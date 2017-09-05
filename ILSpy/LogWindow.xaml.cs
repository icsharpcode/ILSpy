using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace ICSharpCode.ILSpy
{
	partial class LogWindow : UserControl, IPane
	{
		static LogWindow instance;

		public static LogWindow Instance {
			get {
				if (instance == null) {
					App.Current.VerifyAccess();
					instance = new LogWindow();
				}
				return instance;
			}
		}

		readonly AvalonEditEventListener listener;

		private LogWindow()
		{
			InitializeComponent();
			this.listener = new AvalonEditEventListener(this.log);
		}

		public void Show()
		{
			if (!IsVisible) {
				MainWindow.Instance.ShowInBottomPane("Output", this);
			}
		}

		public void Closed()
		{

		}

		class AvalonEditEventListener : EventListener
		{
			private TextEditor editor;

			public AvalonEditEventListener(TextEditor editor)
			{
				this.editor = editor ?? throw new ArgumentNullException(nameof(editor));
			}

			protected override void OnEventSourceCreated(EventSource eventSource)
			{
				if (eventSource.Name == "DecompilerEventSource")
					EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All);
			}

			protected override void OnEventWritten(EventWrittenEventArgs eventData)
			{
				editor.Dispatcher.InvokeAsync(() => WriteToLog(eventData));
			}

			private void WriteToLog(EventWrittenEventArgs eventData)
			{
				editor.AppendText(eventData.Payload[0] + Environment.NewLine);
				editor.ScrollToEnd();
			}
		}
	}
}
