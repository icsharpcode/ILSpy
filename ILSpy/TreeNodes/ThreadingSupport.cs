// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Adds threading support to nodes
	/// </summary>
	class ThreadingSupport
	{
		readonly Stopwatch stopwatch = new Stopwatch();
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		Task<List<SharpTreeNode>> loadChildrenTask;

		public bool IsRunning {
			get { return loadChildrenTask != null && !loadChildrenTask.IsCompleted; }
		}

		public long EllapsedMilliseconds => stopwatch.ElapsedMilliseconds;

		public void Cancel()
		{
			cancellationTokenSource.Cancel();
			loadChildrenTask = null;
			cancellationTokenSource = new CancellationTokenSource();
			stopwatch.Reset();
		}

		/// <summary>
		/// Starts loading the children of the specified node.
		/// </summary>
		public void LoadChildren(SharpTreeNode node, Func<CancellationToken, IEnumerable<SharpTreeNode>> fetchChildren)
		{
			stopwatch.Restart();
			node.Children.Add(new LoadingTreeNode());

			CancellationToken ct = cancellationTokenSource.Token;

			var fetchChildrenEnumerable = fetchChildren(ct);
			Task<List<SharpTreeNode>> thisTask = null;
			thisTask = new Task<List<SharpTreeNode>>(
				delegate {
					List<SharpTreeNode> result = new List<SharpTreeNode>();
					foreach (SharpTreeNode child in fetchChildrenEnumerable)
					{
						ct.ThrowIfCancellationRequested();
						result.Add(child);
						App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<SharpTreeNode>(
							delegate (SharpTreeNode newChild) {
								// don't access "child" here,
								// the background thread might already be running the next loop iteration
								if (loadChildrenTask == thisTask)
								{
									node.Children.Insert(node.Children.Count - 1, newChild);
								}
							}), child);
					}
					return result;
				}, ct);
			loadChildrenTask = thisTask;
			thisTask.Start();
			thisTask.ContinueWith(
				delegate (Task continuation) {
					App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
						delegate {
							if (loadChildrenTask == thisTask)
							{
								stopwatch.Stop();
								node.Children.RemoveAt(node.Children.Count - 1); // remove 'Loading...'
								node.RaisePropertyChanged(nameof(node.Text));

								if (continuation.Exception != null)
								{
									foreach (Exception ex in continuation.Exception.InnerExceptions)
									{
										node.Children.Add(new ErrorTreeNode(ex.ToString()));
									}
								}
							}
						}));
				});

			// Give the task a bit time to complete before we return to WPF - this keeps "Loading..."
			// from showing up for very short waits.
			thisTask.Wait(TimeSpan.FromMilliseconds(200));
		}

		public void Decompile(Language language, ITextOutput output, DecompilationOptions options, Action ensureLazyChildren)
		{
			var loadChildrenTask = this.loadChildrenTask;
			if (loadChildrenTask == null)
			{
				App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, ensureLazyChildren);
				loadChildrenTask = this.loadChildrenTask;
			}
			if (loadChildrenTask != null)
			{
				foreach (ILSpyTreeNode child in loadChildrenTask.Result.Cast<ILSpyTreeNode>())
				{
					child.Decompile(language, output, options);
				}
			}
		}

		sealed class LoadingTreeNode : ILSpyTreeNode
		{
			public override object Text {
				get { return Resources.Loading; }
			}

			public override FilterResult Filter(FilterSettings settings)
			{
				return FilterResult.Match;
			}

			public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			{
			}
		}

		sealed class ErrorTreeNode : ILSpyTreeNode
		{
			readonly string text;

			public override object Text {
				get { return text; }
			}

			public ErrorTreeNode(string text)
			{
				this.text = text;
			}

			public override FilterResult Filter(FilterSettings settings)
			{
				return FilterResult.Match;
			}

			public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			{
			}
		}

		[ExportContextMenuEntry(Header = nameof(Resources.CopyErrorMessage))]
		sealed class CopyErrorMessageContextMenu : IContextMenuEntry
		{
			public bool IsVisible(TextViewContext context)
			{
				if (context.SelectedTreeNodes != null && context.SelectedTreeNodes.All(n => n is ErrorTreeNode))
					return true;
				return false;
			}

			public bool IsEnabled(TextViewContext context)
			{
				return true;
			}

			public void Execute(TextViewContext context)
			{
				StringBuilder builder = new StringBuilder();
				if (context.SelectedTreeNodes != null)
				{
					foreach (var node in context.SelectedTreeNodes.OfType<ErrorTreeNode>())
					{
						builder.AppendLine(node.Text.ToString());
					}
				}
				Clipboard.SetText(builder.ToString());
			}
		}
	}
}
