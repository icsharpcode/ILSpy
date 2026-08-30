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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ICSharpCode.ILSpy.Tests;

public static class WindowExtensions
{
	/// <summary>
	/// Presses and releases <paramref name="button"/> on the visual <paramref name="resolveTarget"/>
	/// returns, once the window's hit test at the click point answers with that visual (or a
	/// descendant of it). Returns the window-relative point that was clicked.
	/// </summary>
	/// <remarks>
	/// Synthesized input is routed by hit testing the rendered scene, so a press only reaches the
	/// intended control when the scene agrees with the visual tree: a closed popup's light-dismiss
	/// overlay keeps answering hit tests until the next frame, a virtualized row may be re-realised
	/// as a different container, and layout may still be moving. Resolving the target on every poll
	/// and waiting for the hit test is that precondition, whatever number of frames it takes.
	/// <paramref name="pointInTarget"/> picks the point inside the target's bounds; the centre by
	/// default.
	/// </remarks>
	public static async Task<Point> ClickAsync(
		this Window window,
		Func<Visual?> resolveTarget,
		MouseButton button = MouseButton.Left,
		RawInputModifiers modifiers = RawInputModifiers.None,
		Func<Visual, Point>? pointInTarget = null,
		[CallerArgumentExpression(nameof(resolveTarget))] string? description = null)
	{
		ArgumentNullException.ThrowIfNull(window);
		ArgumentNullException.ThrowIfNull(resolveTarget);
		pointInTarget ??= static t => new Point(t.Bounds.Width / 2, t.Bounds.Height / 2);
		Visual? target = null;
		Point? point = null;
		object? lastHit = null;
		try
		{
			await Waiters.WaitForAsync(() => {
				target = resolveTarget();
				point = target == null ? null : target.TranslatePoint(pointInTarget(target), window);
				if (point == null)
					return false;
				lastHit = window.InputHitTest(point.Value);
				return lastHit is Visual hit && (ReferenceEquals(hit, target) || hit.GetVisualAncestors().Contains(target));
			}, description: $"{description} to answer the hit test at its click point");
		}
		catch (TimeoutException ex)
		{
			throw new TimeoutException(
				$"{ex.Message} (target: {target?.GetType().Name ?? "not resolved"}, point: {point?.ToString() ?? "n/a"}, hit: {lastHit?.GetType().Name ?? "nothing"})", ex);
		}
		window.MouseDown(point!.Value, button, modifiers);
		window.MouseUp(point.Value, button, modifiers);
		return point.Value;
	}

	/// <summary>Snapshots the window with Skia, writes a temp PNG, and opens it in the OS
	/// image viewer. No-op when <c>UseHeadlessDrawing</c> is true (CI default).</summary>
	public static void CaptureAndShow(this Window window, [CallerMemberName] string? label = null)
	{
		ArgumentNullException.ThrowIfNull(window);
		Dispatcher.UIThread.RunJobs();

		var frame = window.CaptureRenderedFrame();
		if (frame is null)
			return;

		var fileName = $"ILSpy.Tests-{Sanitize(label)}-{DateTime.Now:HHmmss-fff}.png";
		var path = Path.Combine(Path.GetTempPath(), fileName);
		frame.Save(path, PngBitmapEncoderOptions.Default);

		try
		{
			Process.Start(new ProcessStartInfo {
				FileName = path,
				UseShellExecute = true,
			});
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"CaptureAndShow: failed to launch viewer for {path}: {ex.Message}");
		}
	}

	static string Sanitize(string? label)
	{
		if (string.IsNullOrWhiteSpace(label))
			return "frame";
		return new string(label.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
	}
}
