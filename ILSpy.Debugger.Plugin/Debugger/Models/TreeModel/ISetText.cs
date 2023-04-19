namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    internal interface ISetText
    {
        bool CanSetText { get; }

        bool SetText(string text);
    }
}
