﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ILSpy.ReadyToRun.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ILSpy.ReadyToRun.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disassembly Format.
        /// </summary>
        public static string DisassemblyFormat {
            get {
                return ResourceManager.GetString("DisassemblyFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ReadyToRun.
        /// </summary>
        public static string ReadyToRun {
            get {
                return ResourceManager.GetString("ReadyToRun", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show Debug Info.
        /// </summary>
        public static string ShowDebugInfo {
            get {
                return ResourceManager.GetString("ShowDebugInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Show Unwind Info.
        /// </summary>
        public static string ShowStackUnwindInfo {
            get {
                return ResourceManager.GetString("ShowStackUnwindInfo", resourceCulture);
            }
        }

		/// <summary>
		///   Looks up a localized string similar to Show GC Info.
		/// </summary>
		public static string ShowGCInfo {
			get {
				return ResourceManager.GetString("ShowGCInfo", resourceCulture);
			}
		}
	}
}