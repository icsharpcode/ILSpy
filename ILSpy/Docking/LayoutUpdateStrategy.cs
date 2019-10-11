using System.Linq;
using System.Windows;
using ICSharpCode.ILSpy.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace ICSharpCode.ILSpy.Docking
{
	public class LayoutUpdateStrategy : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			if ((destinationContainer != null) &&
				(destinationContainer.FindParent<LayoutFloatingWindow>() != null))
				return false;

			PanePosition targetPosition = PanePosition.Top;
			switch (anchorableToShow.Content) {
				case AnalyzerPaneModel _:
					targetPosition = PanePosition.Bottom;
					break;
				case AssemblyListPaneModel _:
					targetPosition = PanePosition.Left;
					break;
			}

			return DockingHelper.Dock(layout, anchorableToShow, targetPosition);
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
		}

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}
	}
}
