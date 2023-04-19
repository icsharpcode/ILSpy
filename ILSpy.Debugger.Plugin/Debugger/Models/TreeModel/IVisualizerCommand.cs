namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal interface IVisualizerCommand
    {
        bool CanExecute { get; }

        void Execute();
    }
}
