// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;

using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Analyzers
{
	sealed class AISecurityFindingTreeNode : AnalyzerTreeNode
	{
		public AISecurityFindingTreeNode(AISecurityFinding finding)
		{
			Finding = finding;
		}

		public AISecurityFinding Finding { get; }
		public override object Text => Finding.Line > 0
			? $"{Finding.Severity}: {Finding.Issue} (line {Finding.Line})"
			: $"{Finding.Severity}: {Finding.Issue}";
		public override object Icon => Finding.Severity switch {
			"Critical" => Images.SecurityCritical,
			"High" => Images.SecurityHigh,
			"Medium" => Images.SecurityMedium,
			"Low" => Images.SecurityLow,
			_ => Images.SecurityMedium,
		};
		public override object? ToolTip => Finding.Type;

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = true;
			MessageBus.Send(this, new NavigateToReferenceEventArgs(Finding.Target));
		}

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies)
		{
			foreach (LoadedAssembly assembly in removedAssemblies)
				if (Finding.Target.ParentModule?.MetadataFile == assembly.GetMetadataFileOrNull())
					return false;
			return true;
		}
	}
}
