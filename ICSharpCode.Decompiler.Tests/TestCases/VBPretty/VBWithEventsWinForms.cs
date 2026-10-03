using System;
#if ROSLYN && !OPT
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;
using System.Windows.Forms;

public class VBWithEventsWinForms : Control
{
#if ROSLYN && !OPT
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	[AccessedThroughProperty("Button1")]
#if ROSLYN
	[CompilerGenerated]
#endif
	private Button _Button1;

	private string log;

	internal virtual Button Button1 {
#if ROSLYN
		[CompilerGenerated]
#endif
		get {
			return _Button1;
		}
#if ROSLYN
		[CompilerGenerated]
#endif
		[MethodImpl(MethodImplOptions.Synchronized)]
		set {
			EventHandler obj = Button1_Click;
#if ROSLYN
			Button button = _Button1;
			if (button != null)
			{
				button.Click -= obj;
			}
#else
			if (_Button1 != null)
			{
				_Button1.Click -= obj;
			}
#endif
			_Button1 = value;
#if ROSLYN
			button = _Button1;
			if (button != null)
			{
				button.Click += obj;
			}
#else
			if (_Button1 != null)
			{
				_Button1.Click += obj;
			}
#endif
		}
	}

	public VBWithEventsWinForms()
	{
		Load += VBWithEventsWinForms_Load;
		InitializeComponent();
	}

	private void InitializeComponent()
	{
#if ROSLYN
		this._Button1 = new System.Windows.Forms.Button();
		this._Button1.Text = "Click";
#else
		this.Button1 = new System.Windows.Forms.Button();
		this.Button1.Text = "Click";
#endif
	}

	private void VBWithEventsWinForms_Load(object sender, EventArgs e)
	{
		log += "Load;";
	}

	private void Button1_Click(object sender, EventArgs e)
	{
		log += "Click;";
	}

	public void ReplaceButton(Button button)
	{
		Button1 = button;
	}
}

namespace System.Windows.Forms
{
	public class Button : Control
	{
		public event EventHandler Click;

		public void RaiseClick()
		{
			Click?.Invoke(this, EventArgs.Empty);
		}
	}

	public class Control
	{
		public string Text { get; set; }

		public event EventHandler Load;

		public void RaiseLoad()
		{
			Load?.Invoke(this, EventArgs.Empty);
		}
	}
}
