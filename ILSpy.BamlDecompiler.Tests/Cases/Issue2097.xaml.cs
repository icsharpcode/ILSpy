using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ILSpy.BamlDecompiler.Tests.Cases
{
	/// <summary>
	/// Interaction logic for Issue2097.xaml
	/// </summary>
	public partial class Issue2097 : Window
	{
		public Issue2097()
		{
			InitializeComponent();
		}
	}

	class Issue2097Temp
	{
		public static string Test = "Hello";
	}
}
