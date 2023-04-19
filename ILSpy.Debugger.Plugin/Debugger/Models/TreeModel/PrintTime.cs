using System;
using System.Diagnostics;

namespace ICSharpCode.ILSpy.Debugger.Models.TreeModel
{
    public class PrintTime : IDisposable
    {
        public PrintTime(string text)
        {
            this.text = text;
            this.stopwatch.Start();
        }

        public void Dispose()
        {
            this.stopwatch.Stop();
        }

        private string text;

        private Stopwatch stopwatch = new Stopwatch();
    }
}
