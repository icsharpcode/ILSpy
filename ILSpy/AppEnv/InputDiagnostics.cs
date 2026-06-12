// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection;
using System.Text;

using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Diagnostics for chasing the asynchronous DBus failure on Linux: an unobserved
	/// <c>Tmds.DBus.Protocol.DBusException</c> (e.g. <c>org.freedesktop.DBus.Error.ServiceUnknown</c>)
	/// surfaces on the finalizer thread, temporally disconnected from the mouse/keyboard interaction
	/// that made Avalonia issue the DBus call (AT-SPI accessibility, portals, clipboard, IME, …).
	/// <para>
	/// <see cref="Attach"/> records every input event into a rolling ring buffer (and the
	/// <see cref="AppLog.Category.DBusDebug"/> log), so when the exception finally fires we can dump
	/// the recent interaction trail next to the unwrapped exception and read off the trigger.
	/// </para>
	/// Everything here is gated on <see cref="AppLog.Category.DBusDebug"/> (opt in with
	/// <c>ILSPY_LOG=DBUSDEBUG</c>); with the category off, <see cref="Attach"/> installs no handlers
	/// and the app pays nothing.
	/// </summary>
	public static class InputDiagnostics
	{
		const int RingCapacity = 256;
		const long MoveThrottleMs = 50;

		static readonly object ringLock = new();
		static readonly Queue<string> ring = new(RingCapacity + 1);
		static long lastMoveTick;

		/// <summary>
		/// Installs tunnel/bubble handlers for every mouse and keyboard routed event on
		/// <paramref name="root"/> (typically the main window). No-op unless the
		/// <see cref="AppLog.Category.DBusDebug"/> category is enabled at the time of the call, so it
		/// is a launch-time flag: set <c>ILSPY_LOG=DBUSDEBUG</c> before starting the process.
		/// </summary>
		public static void Attach(InputElement root)
		{
			ArgumentNullException.ThrowIfNull(root);
			if (!AppLog.IsEnabled(AppLog.Category.DBusDebug))
				return;

			// Tunnel + handledEventsToo for the gesture events so we log them on the way DOWN,
			// before any control handles them (and possibly triggers the DBus call) -- the log line
			// then reliably precedes the DBus error in time. Focus/enter/exit have no tunnel route,
			// so register those on Bubble.
			root.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.KeyUpEvent, OnKeyUp, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheel, RoutingStrategies.Tunnel, handledEventsToo: true);
			root.AddHandler(InputElement.PointerEnteredEvent, OnPointerEntered, RoutingStrategies.Bubble, handledEventsToo: true);
			root.AddHandler(InputElement.PointerExitedEvent, OnPointerExited, RoutingStrategies.Bubble, handledEventsToo: true);
			root.AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
			root.AddHandler(InputElement.LostFocusEvent, OnLostFocus, RoutingStrategies.Bubble, handledEventsToo: true);

			Record($"input diagnostics attached to {Src(root)}");
		}

		/// <summary>
		/// Appends one timestamped line to the ring buffer and writes it under
		/// <see cref="AppLog.Category.DBusDebug"/>. Public so non-input subsystems can drop markers
		/// onto the same trail when chasing the DBus issue.
		/// </summary>
		public static void Record(string message)
		{
			var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
			lock (ringLock)
			{
				ring.Enqueue(line);
				while (ring.Count > RingCapacity)
					ring.Dequeue();
			}
			AppLog.Write(AppLog.Category.DBusDebug, message);
		}

		/// <summary>Newest-last snapshot of the recent interaction trail, one event per line.</summary>
		public static string DumpRecent()
		{
			lock (ringLock)
			{
				return ring.Count == 0 ? "(no input recorded)" : string.Join(Environment.NewLine, ring);
			}
		}

		#region Input handlers

		static void OnKeyDown(object? sender, KeyEventArgs e)
			=> Record($"KeyDown    {e.Key} mods={e.KeyModifiers} src={Src(e.Source)} handled={e.Handled}");

		static void OnKeyUp(object? sender, KeyEventArgs e)
			=> Record($"KeyUp      {e.Key} mods={e.KeyModifiers} src={Src(e.Source)}");

		static void OnTextInput(object? sender, TextInputEventArgs e)
			=> Record($"TextInput  '{e.Text}' src={Src(e.Source)}");

		static void OnPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			var point = e.GetCurrentPoint(sender as Visual);
			Record($"PtrPressed {point.Properties.PointerUpdateKind} @{Pos(point.Position)} clicks={e.ClickCount} src={Src(e.Source)}");
		}

		static void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			var point = e.GetCurrentPoint(sender as Visual);
			Record($"PtrReleased {point.Properties.PointerUpdateKind} @{Pos(point.Position)} src={Src(e.Source)}");
		}

		static void OnPointerMoved(object? sender, PointerEventArgs e)
		{
			// Moves can fire hundreds of times a second; throttle so the trail stays readable while
			// still bracketing the moment a DBus call fired.
			var now = Environment.TickCount64;
			if (now - lastMoveTick < MoveThrottleMs)
				return;
			lastMoveTick = now;
			Record($"PtrMoved   @{Pos(e.GetPosition(sender as Visual))} src={Src(e.Source)}");
		}

		static void OnPointerWheel(object? sender, PointerWheelEventArgs e)
			=> Record($"PtrWheel   delta={e.Delta} src={Src(e.Source)}");

		static void OnPointerEntered(object? sender, PointerEventArgs e)
			=> Record($"PtrEntered src={Src(e.Source)}");

		static void OnPointerExited(object? sender, PointerEventArgs e)
			=> Record($"PtrExited  src={Src(e.Source)}");

		static void OnGotFocus(object? sender, RoutedEventArgs e)
			=> Record($"GotFocus   src={Src(e.Source)}");

		static void OnLostFocus(object? sender, RoutedEventArgs e)
			=> Record($"LostFocus  src={Src(e.Source)}");

		#endregion

		static string Src(object? source)
		{
			if (source is StyledElement element)
				return string.IsNullOrEmpty(element.Name) ? element.GetType().Name : $"{element.GetType().Name}#{element.Name}";
			return source?.GetType().Name ?? "null";
		}

		static string Pos(Point p) => $"{p.X:0},{p.Y:0}";

		#region DBus exception unwrapping

		/// <summary>
		/// True if <paramref name="exception"/> (flattened, including inner exceptions) carries a
		/// <c>Tmds.DBus.Protocol.DBusException</c>.
		/// </summary>
		public static bool ContainsDBusException(Exception? exception)
		{
			foreach (var root in Roots(exception))
				for (var ex = root; ex != null; ex = ex.InnerException)
					if (IsDBusException(ex))
						return true;
			return false;
		}

		/// <summary>
		/// Fully unwraps <paramref name="exception"/> -- flattens any <see cref="AggregateException"/>,
		/// walks every inner-exception chain, and for each <c>DBusException</c> reflects the
		/// <c>ErrorName</c> / <c>ErrorMessage</c> the bare <c>ToString()</c> hides -- into a readable
		/// multi-line report. We reflect because Tmds.DBus is a transitive dependency we don't
		/// reference directly.
		/// </summary>
		public static string DescribeException(Exception? exception)
		{
			if (exception == null)
				return "(null exception)";

			var sb = new StringBuilder();
			sb.AppendLine($"{exception.GetType().FullName}: {exception.Message}");
			var roots = Roots(exception);
			for (int i = 0; i < roots.Count; i++)
			{
				sb.AppendLine($"--- branch {i + 1}/{roots.Count} ---");
				int depth = 0;
				for (var ex = roots[i]; ex != null; ex = ex.InnerException, depth++)
				{
					var indent = new string(' ', 2 + depth * 2);
					sb.AppendLine($"{indent}{ex.GetType().FullName}: {ex.Message}");
					AppendDBusDetails(sb, ex, indent + "  ");
				}
				if (roots[i].StackTrace is { Length: > 0 } stack)
				{
					sb.AppendLine("  stack:");
					sb.AppendLine(stack);
				}
			}
			return sb.ToString();
		}

		static void AppendDBusDetails(StringBuilder sb, Exception ex, string indent)
		{
			if (!IsDBusException(ex))
				return;
			var type = ex.GetType();
			foreach (var property in new[] { "ErrorName", "ErrorMessage" })
			{
				var value = type.GetProperty(property, BindingFlags.Public | BindingFlags.Instance)?.GetValue(ex);
				if (value != null)
					sb.AppendLine($"{indent}{property} = {value}");
			}
		}

		static bool IsDBusException(Exception ex)
			=> ex.GetType().FullName == "Tmds.DBus.Protocol.DBusException"
				|| ex.GetType().Name == "DBusException";

		static IReadOnlyList<Exception> Roots(Exception? exception)
		{
			if (exception is AggregateException aggregate)
				return aggregate.Flatten().InnerExceptions;
			return exception == null ? Array.Empty<Exception>() : new[] { exception };
		}

		#endregion
	}
}
