using System;

namespace ICSharpCode.ILSpy.Util
{
	public class NavigationHistoryService
	{
		readonly NavigationHistory<NavigationState> history = new();

		public static NavigationHistoryService Instance { get; } = new();

		public bool CanNavigateBack => history.CanNavigateBack;
		public bool CanNavigateForward => history.CanNavigateForward;

		private NavigationHistoryService()
		{
		}

		public void UpdateCurrent(NavigationState navigationState)
		{
			history.UpdateCurrent(navigationState);
		}

		public void Record(NavigationState navigationState)
		{
			history.Record(navigationState);
		}

		public NavigationState GoForward()
		{
			return history.GoForward();
		}

		public NavigationState GoBack()
		{
			return history.GoBack();
		}

		public void Clear()
		{
			history.Clear();
		}

		public void RemoveAll(Predicate<NavigationState> predicate)
		{
			history.RemoveAll(predicate);
		}
	}
}
