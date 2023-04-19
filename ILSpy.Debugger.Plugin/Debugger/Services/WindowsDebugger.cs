using Debugger;
using global::Debugger.Interop;
using global::Debugger.Interop.CorPublish;
using global::Debugger.MetaData;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Debugger.Bookmarks;
using ICSharpCode.ILSpy.Debugger.Models.TreeModel;
using ICSharpCode.ILSpy.Debugger.Tooltips;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Visitors;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Services
{
    [Export(typeof(IDebugger))]
    public class WindowsDebugger : IDebugger, IDisposable
    {
        public event EventHandler<ProcessEventArgs> ProcessSelected;

        public NDebugger DebuggerCore
        {
            get
            {
                return this.debugger;
            }
        }

        public global::Debugger.Process DebuggedProcess
        {
            get
            {
                return this.debuggedProcess;
            }
        }

        public static global::Debugger.Process CurrentProcess
        {
            get
            {
                WindowsDebugger windowsDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
                if (windowsDebugger != null && windowsDebugger.DebuggedProcess != null)
                {
                    return windowsDebugger.DebuggedProcess;
                }
                return null;
            }
        }

        public bool BreakAtBeginning { get; set; }

        protected virtual void OnProcessSelected(ProcessEventArgs e)
        {
            if (this.ProcessSelected != null)
            {
                this.ProcessSelected(this, e);
            }
        }

        public bool ServiceInitialized
        {
            get
            {
                return this.debugger != null;
            }
        }

        public bool IsDebugging
        {
            get
            {
                return this.ServiceInitialized && this.debuggedProcess != null;
            }
        }

        public bool IsAttached
        {
            get
            {
                return this.ServiceInitialized && this.attached;
            }
        }

        public bool IsProcessRunning
        {
            get
            {
                return this.IsDebugging && this.debuggedProcess.IsRunning;
            }
        }

        public void Start(ProcessStartInfo processStartInfo)
        {
            if (this.IsDebugging)
            {
                MessageBox.Show(this.errorDebugging);
                return;
            }
            if (!this.ServiceInitialized)
            {
                this.InitializeService();
            }
            string programVersion = this.debugger.GetProgramVersion(processStartInfo.FileName);
            if (programVersion.StartsWith("v1.0"))
            {
                MessageBox.Show("Net10NotSupported");
                return;
            }
            if (programVersion.StartsWith("v1.1"))
            {
                MessageBox.Show("Net1.1NotSupported");
                return;
            }
            if (this.debugger.IsKernelDebuggerEnabled)
            {
                MessageBox.Show("KernelDebuggerEnabled");
                return;
            }
            this.attached = false;
            if (this.DebugStarting != null)
            {
                this.DebugStarting(this, EventArgs.Empty);
            }
            try
            {
                global::Debugger.Process.DebugMode = DebugModeFlag.Debug;
                global::Debugger.Process process = this.debugger.Start(processStartInfo.FileName, processStartInfo.WorkingDirectory, processStartInfo.Arguments);
                this.SelectProcess(process);
            }
            catch (System.Exception ex)
            {
                if (!(ex is COMException) && !(ex is BadImageFormatException) && !(ex is UnauthorizedAccessException))
                {
                    throw;
                }
                string text = "CannotStartProcess";
                text = text + " " + ex.Message;
                if (ex is COMException && ((COMException)ex).ErrorCode == -2147024846)
                {
                    text = text + Environment.NewLine + Environment.NewLine;
                    text += "64-bit debugging is not supported.  Please set Project -> Project Options... -> Compiling -> Target CPU to 32bit.";
                }
                MessageBox.Show(text);
                if (this.DebugStopped != null)
                {
                    this.DebugStopped(this, EventArgs.Empty);
                }
            }
        }

        public void Attach(System.Diagnostics.Process existingProcess)
        {
            if (existingProcess == null)
            {
                return;
            }
            if (this.IsDebugging)
            {
                MessageBox.Show(this.errorDebugging);
                return;
            }
            if (!this.ServiceInitialized)
            {
                this.InitializeService();
            }
            string programVersion = this.debugger.GetProgramVersion(existingProcess.MainModule.FileName);
            if (programVersion.StartsWith("v1.0"))
            {
                MessageBox.Show("Net10NotSupported");
                return;
            }
            if (this.DebugStarting != null)
            {
                this.DebugStarting(this, EventArgs.Empty);
            }
            try
            {
                global::Debugger.Process.DebugMode = DebugModeFlag.Debug;
                global::Debugger.Process process = this.debugger.Attach(existingProcess);
                this.attached = true;
                this.SelectProcess(process);
            }
            catch (System.Exception ex)
            {
                if (!(ex is COMException) && !(ex is UnauthorizedAccessException))
                {
                    throw;
                }
                string str = "CannotAttachToProcess";
                MessageBox.Show(str + " " + ex.Message);
                if (this.DebugStopped != null)
                {
                    this.DebugStopped(this, EventArgs.Empty);
                }
            }
        }

        public void Detach()
        {
            if (this.debuggedProcess == null)
            {
                return;
            }
            this.debugger.Detach();
        }

        public void StartWithoutDebugging(ProcessStartInfo processStartInfo)
        {
            System.Diagnostics.Process.Start(processStartInfo);
        }

        public void Stop()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "Stop");
                return;
            }
            if (this.IsAttached)
            {
                this.Detach();
                return;
            }
            this.debuggedProcess.Terminate();
        }

        public void Break()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "Break");
                return;
            }
            if (!this.IsProcessRunning)
            {
                MessageBox.Show(this.errorProcessPaused, "Break");
                return;
            }
            this.debuggedProcess.Break();
        }

        public void Continue()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "Continue");
                return;
            }
            if (this.IsProcessRunning)
            {
                MessageBox.Show(this.errorProcessRunning, "Continue");
                return;
            }
            this.debuggedProcess.AsyncContinue();
        }

        private SourceCodeMapping GetCurrentCodeMapping(out global::Debugger.StackFrame frame, out bool isMatch)
        {
            isMatch = false;
            frame = this.debuggedProcess.SelectedThread.MostRecentStackFrame;
            int metadataToken = frame.MethodInfo.MetadataToken;
            if (DebugInformation.CodeMappings == null || !DebugInformation.CodeMappings.ContainsKey(metadataToken))
            {
                return null;
            }
            return DebugInformation.CodeMappings[metadataToken].GetInstructionByTokenAndOffset(frame.IP, out isMatch);
        }

        private global::Debugger.StackFrame GetStackFrame()
        {
            global::Debugger.StackFrame mostRecentStackFrame;
            bool isMatch;
            SourceCodeMapping currentCodeMapping = this.GetCurrentCodeMapping(out mostRecentStackFrame, out isMatch);
            if (currentCodeMapping == null)
            {
                mostRecentStackFrame = this.debuggedProcess.SelectedThread.MostRecentStackFrame;
                mostRecentStackFrame.ILRanges = new int[]
                {
                    0,
                    1
                };
            }
            else
            {
                mostRecentStackFrame.SourceCodeLine = currentCodeMapping.StartLocation.Line;
                mostRecentStackFrame.ILRanges = currentCodeMapping.ToArray(isMatch);
            }
            return mostRecentStackFrame;
        }

        public void StepInto()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "StepInto");
                return;
            }
            if (this.debuggedProcess.SelectedThread == null || this.debuggedProcess.SelectedThread.MostRecentStackFrame == null || this.debuggedProcess.IsRunning)
            {
                MessageBox.Show(this.errorCannotStepNoActiveFunction, "StepInto");
                return;
            }
            global::Debugger.StackFrame stackFrame = this.GetStackFrame();
            if (stackFrame != null)
            {
                stackFrame.AsyncStepInto();
            }
        }

        public void StepOver()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "StepOver");
                return;
            }
            if (this.debuggedProcess.SelectedThread == null || this.debuggedProcess.SelectedThread.MostRecentStackFrame == null || this.debuggedProcess.IsRunning)
            {
                MessageBox.Show(this.errorCannotStepNoActiveFunction, "StepOver");
                return;
            }
            global::Debugger.StackFrame stackFrame = this.GetStackFrame();
            if (stackFrame != null)
            {
                stackFrame.AsyncStepOver();
            }
        }

        public void StepOut()
        {
            if (!this.IsDebugging)
            {
                MessageBox.Show(this.errorNotDebugging, "StepOut");
                return;
            }
            if (this.debuggedProcess.SelectedThread == null || this.debuggedProcess.SelectedThread.MostRecentStackFrame == null || this.debuggedProcess.IsRunning)
            {
                MessageBox.Show(this.errorCannotStepNoActiveFunction, "StepOut");
                return;
            }
            global::Debugger.StackFrame stackFrame = this.GetStackFrame();
            if (stackFrame != null)
            {
                stackFrame.AsyncStepOut();
            }
        }

        public event EventHandler DebugStarting;

        public event EventHandler DebugStarted;

        public event EventHandler DebugStopped;

        public event EventHandler IsProcessRunningChanged;

        protected virtual void OnIsProcessRunningChanged(EventArgs e)
        {
            if (this.IsProcessRunningChanged != null)
            {
                this.IsProcessRunningChanged(this, e);
            }
        }

        public Value GetValueFromName(string variableName)
        {
            if (!this.CanEvaluate)
            {
                return null;
            }
            return ExpressionEvaluator.Evaluate(variableName, SupportedLanguage.CSharp, this.debuggedProcess.SelectedThread.MostRecentStackFrame);
        }

        public ICSharpCode.NRefactory.CSharp.Expression GetExpression(string variableName)
        {
            if (!this.CanEvaluate)
            {
                throw new GetValueException("Cannot evaluate now - debugged process is either null or running or has no selected stack frame");
            }
            return ExpressionEvaluator.ParseExpression(variableName, SupportedLanguage.CSharp);
        }

        public bool IsManaged(int processId)
        {
            this.corPublish = new CorpubPublishClass();
            TrackedComObjects.Track(this.corPublish);
            ICorPublishProcess process = this.corPublish.GetProcess((uint)processId);
            return process != null && process.IsManaged() != 0;
        }

        public string GetValueAsString(string variableName)
        {
            string result;
            try
            {
                Value valueFromName = this.GetValueFromName(variableName);
                if (valueFromName == null)
                {
                    result = null;
                }
                else
                {
                    result = valueFromName.AsString(int.MaxValue);
                }
            }
            catch (GetValueException)
            {
                result = null;
            }
            return result;
        }

        public bool CanEvaluate
        {
            get
            {
                return this.debuggedProcess != null && !this.debuggedProcess.IsRunning && this.debuggedProcess.SelectedThread != null && this.debuggedProcess.SelectedThread.MostRecentStackFrame != null;
            }
        }

        public object GetTooltipControl(TextLocation logicalPosition, string variableName)
        {
            object result;
            try
            {
                ICSharpCode.NRefactory.CSharp.Expression expression = this.GetExpression(variableName);
                if (expression == null)
                {
                    result = null;
                }
                else
                {
                    string imageName;
                    ImageSource imageForLocalVariable = ExpressionNode.GetImageForLocalVariable(out imageName);
                    result = new DebuggerTooltipControl(logicalPosition, new ExpressionNode(imageForLocalVariable, variableName, expression)
                    {
                        ImageName = imageName
                    });
                }
            }
            catch (GetValueException)
            {
                result = null;
            }
            return result;
        }

        internal ITreeNode GetNode(string variable, string currentImageName = null)
        {
            ITreeNode result;
            try
            {
                ICSharpCode.NRefactory.CSharp.Expression expression = this.GetExpression(variable);
                string imageName;
                ImageSource image;
                if (string.IsNullOrEmpty(currentImageName))
                {
                    image = ExpressionNode.GetImageForLocalVariable(out imageName);
                }
                else
                {
                    image = ImageService.GetImage(currentImageName);
                    imageName = currentImageName;
                }
                result = new ExpressionNode(image, variable, expression)
                {
                    ImageName = imageName
                };
            }
            catch (GetValueException)
            {
                result = null;
            }
            return result;
        }

        public bool CanSetInstructionPointer(string filename, int line, int column)
        {
            if (this.debuggedProcess != null && this.debuggedProcess.IsPaused && this.debuggedProcess.SelectedThread != null && this.debuggedProcess.SelectedThread.MostRecentStackFrame != null)
            {
                SourcecodeSegment sourcecodeSegment = this.debuggedProcess.SelectedThread.MostRecentStackFrame.CanSetIP(filename, line, column);
                return sourcecodeSegment != null;
            }
            return false;
        }

        public bool SetInstructionPointer(string filename, int line, int column)
        {
            if (this.CanSetInstructionPointer(filename, line, column))
            {
                SourcecodeSegment sourcecodeSegment = this.debuggedProcess.SelectedThread.MostRecentStackFrame.SetIP(filename, line, column);
                return sourcecodeSegment != null;
            }
            return false;
        }

        public void Dispose()
        {
            this.Stop();
        }

        public event EventHandler Initialize;

        public void InitializeService()
        {
            this.debugger = new NDebugger();
            this.debugger.DebuggerTraceMessage += this.debugger_TraceMessage;
            this.debugger.Processes.Added += this.debugger_ProcessStarted;
            this.debugger.Processes.Removed += this.debugger_ProcessExited;
            DebuggerService.BreakPointAdded += delegate (object sender, BreakpointBookmarkEventArgs e)
            {
                this.AddBreakpoint(e.BreakpointBookmark);
            };
            foreach (BreakpointBookmark bookmark in DebuggerService.Breakpoints)
            {
                this.AddBreakpoint(bookmark);
            }
            if (this.Initialize != null)
            {
                this.Initialize(this, null);
            }
        }

        private bool Compare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void AddBreakpoint(BreakpointBookmark bookmark)
        {
            Breakpoint breakpoint = null;
            breakpoint = new ILBreakpoint(this.debugger, bookmark.MemberReference.DeclaringType.FullName, bookmark.LineNumber, bookmark.FunctionToken, bookmark.ILRange.From, bookmark.IsEnabled);
            this.debugger.Breakpoints.Add(breakpoint);
            bookmark.IsEnabledChanged += delegate (object A_1, EventArgs A_2)
            {
                breakpoint.Enabled = bookmark.IsEnabled;
            };
            breakpoint.Set += delegate (object A_0, BreakpointEventArgs A_1)
            {
            };
            EventHandler<CollectionItemEventArgs<global::Debugger.Process>> bp_debugger_ProcessStarted = delegate (object sender, CollectionItemEventArgs<global::Debugger.Process> e)
            {
                breakpoint.Line = bookmark.LineNumber;
            };
            EventHandler<CollectionItemEventArgs<global::Debugger.Process>> bp_debugger_ProcessExited = delegate (object sender, CollectionItemEventArgs<global::Debugger.Process> e)
            {
            };
            EventHandler<BreakpointEventArgs> bp_debugger_BreakpointHit = delegate (object sender, BreakpointEventArgs e)
            {
                switch (bookmark.Action)
                {
                    default:
                        return;
                }
            };
            BookmarkEventHandler bp_bookmarkManager_Removed = null;
            bp_bookmarkManager_Removed = delegate (object sender, BookmarkEventArgs e)
            {
                if (bookmark == e.Bookmark)
                {
                    this.debugger.Breakpoints.Remove(breakpoint);
                    this.debugger.Processes.Added -= bp_debugger_ProcessStarted;
                    this.debugger.Processes.Removed -= bp_debugger_ProcessExited;
                    breakpoint.Hit -= bp_debugger_BreakpointHit;
                    BookmarkManager.Removed -= bp_bookmarkManager_Removed;
                }
            };
            this.debugger.Processes.Added += bp_debugger_ProcessStarted;
            this.debugger.Processes.Removed += bp_debugger_ProcessExited;
            breakpoint.Hit += bp_debugger_BreakpointHit;
            BookmarkManager.Removed += bp_bookmarkManager_Removed;
        }

        private bool Evaluate(string code, string language)
        {
            bool result;
            try
            {
                SupportedLanguage language2 = (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), language, true);
                Value value = ExpressionEvaluator.Evaluate(code, language2, this.debuggedProcess.SelectedThread.MostRecentStackFrame);
                if (value != null && value.Type.IsPrimitive && value.PrimitiveValue is bool)
                {
                    result = (bool)value.PrimitiveValue;
                }
                else
                {
                    result = false;
                }
            }
            catch (GetValueException ex)
            {
                string.Concat(new string[]
                {
                    "Error while evaluating breakpoint condition ",
                    code,
                    ":\n",
                    ex.Message,
                    "\n"
                });
                result = true;
            }
            return result;
        }

        private void LogMessage(object sender, MessageEventArgs e)
        {
        }

        private void debugger_TraceMessage(object sender, MessageEventArgs e)
        {
        }

        private void debugger_ProcessStarted(object sender, CollectionItemEventArgs<global::Debugger.Process> e)
        {
            if (this.debugger.Processes.Count == 1 && this.DebugStarted != null)
            {
                this.DebugStarted(this, EventArgs.Empty);
            }
            e.Item.LogMessage += this.LogMessage;
        }

        private void debugger_ProcessExited(object sender, CollectionItemEventArgs<global::Debugger.Process> e)
        {
            if (this.debugger.Processes.Count == 0)
            {
                if (this.DebugStopped != null)
                {
                    this.DebugStopped(this, e);
                }
                this.SelectProcess(null);
                return;
            }
            this.SelectProcess(this.debugger.Processes[0]);
        }

        public void SelectProcess(global::Debugger.Process process)
        {
            if (this.debuggedProcess != null)
            {
                this.debuggedProcess.Paused -= this.debuggedProcess_DebuggingPaused;
                this.debuggedProcess.ExceptionThrown -= this.debuggedProcess_ExceptionThrown;
                this.debuggedProcess.Resumed -= this.debuggedProcess_DebuggingResumed;
                this.debuggedProcess.ModulesAdded -= this.debuggedProcess_ModulesAdded;
            }
            this.debuggedProcess = process;
            if (this.debuggedProcess != null)
            {
                this.debuggedProcess.Paused += this.debuggedProcess_DebuggingPaused;
                this.debuggedProcess.ExceptionThrown += this.debuggedProcess_ExceptionThrown;
                this.debuggedProcess.Resumed += this.debuggedProcess_DebuggingResumed;
                this.debuggedProcess.ModulesAdded += this.debuggedProcess_ModulesAdded;
                this.debuggedProcess.BreakAtBeginning = this.BreakAtBeginning;
            }
            this.BreakAtBeginning = false;
            this.OnProcessSelected(new ProcessEventArgs(process));
        }

        private void debuggedProcess_ModulesAdded(object sender, ModuleEventArgs e)
        {
            List<string> namesOfDefinedTypes = e.Module.GetNamesOfDefinedTypes();
            BreakpointBookmark bookmark;
            foreach (BreakpointBookmark bookmark2 in DebuggerService.Breakpoints)
            {
                bookmark = bookmark2;
                Breakpoint breakpoint = this.debugger.Breakpoints.FirstOrDefault((Breakpoint b) => b.Line == bookmark.LineNumber && (b as ILBreakpoint).MetadataToken == bookmark.MemberReference.MetadataToken.ToInt32());
                if (breakpoint != null && namesOfDefinedTypes.Contains(breakpoint.TypeName))
                {
                    breakpoint.SetBreakpoint(e.Module);
                }
            }
        }

        private void debuggedProcess_DebuggingPaused(object sender, ProcessEventArgs e)
        {
            this.JumpToCurrentLine();
            this.OnIsProcessRunningChanged(EventArgs.Empty);
        }

        private void debuggedProcess_DebuggingResumed(object sender, ProcessEventArgs e)
        {
            this.OnIsProcessRunningChanged(EventArgs.Empty);
            DebuggerService.RemoveCurrentLineMarker();
        }

        private void debuggedProcess_ExceptionThrown(object sender, global::Debugger.ExceptionEventArgs e)
        {
            if (!e.IsUnhandled)
            {
                e.Process.AsyncContinue();
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (e.Process.SelectedThread.InterceptCurrentException())
            {
                stringBuilder.AppendLine(e.Exception.ToString());
                string stackTrace;
                try
                {
                    stackTrace = e.Exception.GetStackTrace("--- End of inner exception stack trace ---");
                }
                catch (GetValueException)
                {
                    stackTrace = e.Process.SelectedThread.GetStackTrace("at {0} in {1}:line {2}", "at {0}");
                }
                stringBuilder.Append(stackTrace);
            }
            else
            {
                stringBuilder.AppendLine("CannotInterceptException");
                stringBuilder.AppendLine(e.Exception.ToString());
                stringBuilder.Append(e.Process.SelectedThread.GetStackTrace("at {0} in {1}:line {2}", "at {0}"));
            }
            string caption = e.IsUnhandled ? "Unhandled" : "Handled";
            string str = string.Format("Message {0} {1}", e.Exception.Type, e.Exception.Message);
            MessageBox.Show(str + stringBuilder.ToString(), caption);
        }

        public void JumpToCurrentLine()
        {
            if (this.debuggedProcess != null && this.debuggedProcess.SelectedThread != null)
            {
                MainWindow.Instance.Activate();
                global::Debugger.StackFrame mostRecentStackFrame = this.debuggedProcess.SelectedThread.MostRecentStackFrame;
                if (mostRecentStackFrame == null)
                {
                    return;
                }
                int metadataToken = mostRecentStackFrame.MethodInfo.MetadataToken;
                int ip = mostRecentStackFrame.IP;
                MemberReference memberReference;
                int num;
                if (DebugInformation.CodeMappings != null && DebugInformation.CodeMappings.ContainsKey(metadataToken) && DebugInformation.CodeMappings[metadataToken].GetInstructionByTokenAndOffset(ip, out memberReference, out num))
                {
                    DebugInformation.DebugStepInformation = null;
                    DebuggerService.RemoveCurrentLineMarker();
                    DebuggerService.JumpToCurrentLine(memberReference, num, 0, num, 0, ip);
                    return;
                }
                this.StepIntoUnknownFrame(mostRecentStackFrame);
            }
        }

        private void StepIntoUnknownFrame(global::Debugger.StackFrame frame)
        {
            string value = frame.MethodInfo.DebugModule.Process.DebuggeeVersion.Substring(1, 3);
            DebugType debugType = (DebugType)frame.MethodInfo.DeclaringType;
            int metadataToken = frame.MethodInfo.MetadataToken;
            int ip = frame.IP;
            string fullNameWithoutGenericArguments = debugType.FullNameWithoutGenericArguments;
            DebugInformation.LoadedAssemblies = from a in MainWindow.Instance.CurrentAssemblyList.GetAssemblies()
                                                select a.AssemblyDefinition;
            if (DebugInformation.LoadedAssemblies == null)
            {
                throw new NullReferenceException("No DebugData assemblies!");
            }
            TypeDefinition typeDefinition = null;
            TypeDefinition typeDefinition2 = null;
            foreach (AssemblyDefinition assemblyDefinition in DebugInformation.LoadedAssemblies)
            {
                if (assemblyDefinition != null && ((!assemblyDefinition.FullName.StartsWith("System") && !assemblyDefinition.FullName.StartsWith("Microsoft") && !assemblyDefinition.FullName.StartsWith("mscorlib")) || assemblyDefinition.Name.Version.ToString().StartsWith(value)))
                {
                    foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    {
                        TypeDefinition type = moduleDefinition.GetType(fullNameWithoutGenericArguments);
                        if (type != null)
                        {
                            if (type.DeclaringType == null)
                            {
                                typeDefinition = type;
                                break;
                            }
                            typeDefinition2 = type;
                            typeDefinition = type.DeclaringType;
                            break;
                        }
                    }
                    if (typeDefinition != null)
                    {
                        break;
                    }
                }
            }
            if (typeDefinition != null)
            {
                TypeDefinition type2 = typeDefinition2 ?? typeDefinition;
                DebugInformation.DebugStepInformation = Tuple.Create<int, int, MemberReference>(metadataToken, ip, type2.GetMemberByToken(metadataToken));
            }
        }

        public void ShowAttachDialog()
        {
            throw new NotImplementedException();
        }

        private bool attached;

        private NDebugger debugger;

        private ICorPublish corPublish;

        private global::Debugger.Process debuggedProcess;

        private string errorDebugging = "Error.Debugging";

        private string errorNotDebugging = "Error.NotDebugging";

        private string errorProcessRunning = "Error.ProcessRunning";

        private string errorProcessPaused = "Error.ProcessPaused";

        private string errorCannotStepNoActiveFunction = "Threads.CannotStepNoActiveFunction";

        private enum StopAttachedProcessDialogResult
        {
            Detach,
            Terminate,
            Cancel
        }
    }
}
