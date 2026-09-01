// Copyright (c) 2026 Siegfried Pammer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ICSharpCode.ILSpyX.TreeView
{
	// Thread affinity of the tree model.
	//
	// The model is not thread-safe. TreeFlattener.Count and SharpTreeNode.GetNodeByVisibleIndex
	// both read the augmented 'totalListLength' fields, so a structural mutation racing a read can
	// hand out an index for a node that no longer exists; issue #3290 is a NullReferenceException
	// inside GetNodeByVisibleIndex that is unreachable single-threaded. The rule is that a tree
	// displayed by a UI is mutated only from that UI's thread, but ICSharpCode.ILSpyX is
	// host-agnostic and must not name a dispatcher, so ownership is stated explicitly instead:
	// a host calls SetOwner() on the root of a tree it takes over, passing the thread and the way
	// to get onto it.
	//
	// Ownership carries the rule two ways. EnsureLazyChildren uses the invoke delegate to move
	// itself onto the owning thread, so a caller cannot get it wrong; and, in debug builds, every
	// other structural mutation is verified against the owning thread so a call site that bypasses
	// the rule is named instead of silently corrupting the flat list.
	partial class SharpTreeNode
	{
		Thread? owner;
		Action<Action>? ownerInvoke;

		/// <summary>
		/// The nearest node on the model-parent chain that carries an explicit owner, or null when
		/// nothing on the chain has been claimed.
		/// </summary>
		/// <remarks>
		/// Resolving the owner by walking up instead of stamping every node gives the propagation
		/// rules for free:
		/// <list type="bullet">
		/// <item>Ownership covers the whole subtree below the node it was set on, so a single call on
		/// the root of a displayed tree protects everything in it.</item>
		/// <item>Children added later inherit it, with no bookkeeping at insertion time.</item>
		/// <item>A subtree built on a background thread carries no owner and is therefore unchecked
		/// while it is being built. It inherits the owner the instant it is attached below an owned
		/// parent - and that attachment is itself a mutation of the owned tree, so it is checked.</item>
		/// </list>
		/// </remarks>
		SharpTreeNode? OwnerNode {
			get {
				for (SharpTreeNode? node = this; node != null; node = node.modelParent)
				{
					if (node.owner != null)
						return node;
				}
				return null;
			}
		}

		Thread? EffectiveOwner => OwnerNode?.owner;

		/// <summary>
		/// Declares <paramref name="owner"/> as the only thread allowed to structurally mutate this
		/// node and its subtree.
		/// </summary>
		/// <param name="invoke">
		/// Runs an action on <paramref name="owner"/> and blocks until it has completed - the host's
		/// dispatcher invoke. Supplying it lets <see cref="EnsureLazyChildren"/> marshal itself
		/// instead of every caller having to know it must. Null leaves the tree unmarshalled, which
		/// only makes sense for a tree no UI is displaying.
		/// </param>
		/// <remarks>
		/// Re-owning is allowed - handing a tree over is exactly what this exists for - but the
		/// handoff must be performed by the thread that currently owns the tree, because a
		/// background thread taking ownership of a live tree away from the UI is the very race the
		/// affinity check hunts for.
		/// </remarks>
		public void SetOwner(Thread owner, Action<Action>? invoke = null)
		{
			VerifyAccess(nameof(SetOwner));
			this.owner = owner;
			this.ownerInvoke = invoke;
		}

		/// <summary>
		/// Declares the calling thread as the only thread allowed to structurally mutate this node
		/// and its subtree, without a way to marshal onto it.
		/// </summary>
		public void SetOwner() => SetOwner(Thread.CurrentThread);

		/// <summary>
		/// Reports a violation if the calling thread is not the effective owner of this node.
		/// </summary>
		[Conditional("DEBUG")]
		internal void VerifyAccess(string operation)
		{
#if DEBUG
			Thread? expected = EffectiveOwner;
			if (expected == null || expected == Thread.CurrentThread)
				return;
			TreeThreadAffinity.Report(this, operation, expected);
#endif
		}

		/// <summary>
		/// Verifies a pending change to this node's <see cref="Children"/> collection.
		/// </summary>
		[Conditional("DEBUG")]
		internal void VerifyChildrenChange(NotifyCollectionChangedEventArgs e)
		{
#if DEBUG
			VerifyAccess("Children." + e.Action);
			if (e.NewItems == null)
				return;
			// An already-owned subtree being attached below a differently-owned parent would keep its
			// own owner, so the two halves of one displayed tree would demand two different threads.
			// That is a mistake worth naming, but only once: the incoming owner is dropped afterwards
			// so the subtree inherits the parent's, instead of reporting again on every later mutation.
			Thread? parentOwner = EffectiveOwner;
			foreach (SharpTreeNode node in e.NewItems)
			{
				if (node.owner != null && node.owner != parentOwner)
				{
					TreeThreadAffinity.Report(node, "attach of a subtree owned by another thread", node.owner);
					node.owner = null;
					node.ownerInvoke = null;
				}
			}
#endif
		}
	}

	/// <summary>
	/// Collects <see cref="SharpTreeNode"/> thread-affinity violations. Nothing here is reached in
	/// release builds: every caller is <c>[Conditional("DEBUG")]</c>.
	/// </summary>
	public static class TreeThreadAffinity
	{
		/// <summary>
		/// Environment variable selecting the log file. Defaults to ILSpy.TreeAffinity.log in the
		/// temp directory; set it to an empty string to disable file logging.
		/// </summary>
		public const string LogFileVariable = "ILSPY_TREE_AFFINITY_LOG";

		/// <summary>
		/// Environment variable enabling <see cref="FailFast"/> at startup (set it to 1).
		/// </summary>
		public const string FailFastVariable = "ILSPY_TREE_AFFINITY_FAILFAST";

		static readonly ConcurrentDictionary<string, TreeThreadAffinityViolation> violations = new();
		static readonly object logLock = new();

		/// <summary>
		/// When true, a violation throws after being recorded, so a debugger stops at the offending
		/// frame. It is not what makes a violation observable - <see cref="Violations"/> is recorded
		/// either way, because the throw may well be swallowed by a caller that catches Exception.
		/// An exploratory session over a large corpus leaves this off so one bad call site does not
		/// interfere with the rest of the run.
		/// </summary>
		public static bool FailFast { get; set; } = Environment.GetEnvironmentVariable(FailFastVariable) == "1";

		/// <summary>
		/// The file violations are appended to, or null when file logging is disabled.
		/// </summary>
		public static string? LogFilePath { get; set; } = ResolveLogFilePath();

		static string? ResolveLogFilePath()
		{
			string? configured = Environment.GetEnvironmentVariable(LogFileVariable);
			if (configured == null)
				return Path.Combine(Path.GetTempPath(), "ILSpy.TreeAffinity.log");
			return configured.Length == 0 ? null : configured;
		}

		/// <summary>
		/// The distinct violating call sites recorded so far, most frequent first.
		/// </summary>
		public static IReadOnlyList<TreeThreadAffinityViolation> Violations
			=> violations.Values.OrderByDescending(v => v.Count).ToList();

		public static void Clear() => violations.Clear();

		internal static void Report(SharpTreeNode node, string operation, Thread expected)
		{
			// Skip Report and the [Conditional] wrapper that called it, so frame 0 is the mutation.
			string stackTrace = new StackTrace(2, fNeedFileInfo: true).ToString();
			var violation = new TreeThreadAffinityViolation(Describe(node), operation, expected, Thread.CurrentThread, stackTrace);
			Debug.WriteLine(violation.ToString());
			// Deduplicate by call site: a mutation inside a loop must not produce one entry per
			// iteration. The first occurrence is written through immediately so a long-running
			// session can be inspected without stopping the process; repeats only bump the count.
			var recorded = violations.GetOrAdd(stackTrace, violation);
			if (recorded.Hit() == 1)
				AppendToLog(recorded.ToString());
			// Recorded first, thrown second. The throw is only a convenience for stopping a
			// debugger at the offending frame; it is not the record. Tree mutation happens inside
			// callers that catch Exception (the background decompile writes the failure into the
			// text view), so a throw alone would be swallowed and a fail-fast run would come back
			// green while violating. Violations is what a test or a session asserts on.
			if (FailFast)
				throw new InvalidOperationException(violation.ToString());
		}

		static string Describe(SharpTreeNode node)
		{
			string text;
			try
			{
				// ToString() is what the model itself already uses for diagnostics. It can still fail
				// here: this runs on a thread the node did not expect, which is the whole problem.
				text = node.ToString() ?? "<null>";
			}
			catch (Exception ex)
			{
				text = "<ToString() threw " + ex.GetType().Name + ">";
			}
			return node.GetType().FullName + " \"" + text + "\"";
		}

		static void AppendToLog(string text)
		{
			string? path = LogFilePath;
			if (path == null)
				return;
			try
			{
				lock (logLock)
				{
					File.AppendAllText(path, text + Environment.NewLine + Environment.NewLine);
				}
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				// A diagnostic that cannot write its log must not take the session down with it.
			}
		}
	}

	/// <summary>
	/// One distinct call site that mutated a tree from a thread other than its owner.
	/// </summary>
	public sealed class TreeThreadAffinityViolation
	{
		int count;

		internal TreeThreadAffinityViolation(string node, string operation, Thread expected, Thread actual, string stackTrace)
		{
			this.Node = node;
			this.Operation = operation;
			this.ExpectedThread = DescribeThread(expected);
			this.ActualThread = DescribeThread(actual);
			this.StackTrace = stackTrace;
		}

		public string Node { get; }
		public string Operation { get; }
		public string ExpectedThread { get; }
		public string ActualThread { get; }
		public string StackTrace { get; }

		/// <summary>Number of times this call site has been hit.</summary>
		public int Count => count;

		internal int Hit() => Interlocked.Increment(ref count);

		static string DescribeThread(Thread thread)
			=> thread.ManagedThreadId + " \"" + (thread.Name ?? "<unnamed>") + "\"";

		public override string ToString()
		{
			var b = new StringBuilder();
			b.Append("SharpTreeNode thread affinity violation: ").Append(Operation).AppendLine();
			b.Append("  node:     ").AppendLine(Node);
			b.Append("  expected: thread ").AppendLine(ExpectedThread);
			b.Append("  actual:   thread ").AppendLine(ActualThread);
			b.Append(StackTrace);
			return b.ToString();
		}
	}
}
