// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using ICSharpCode.ILSpy.Bookmarks;
using System.Xml.Linq;
using ICSharpCode.ILSpy.Debugger.Bookmarks;
using ICSharpCode.ILSpy.XmlDoc;
using Mono.Cecil;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;

#endregion

namespace ICSharpCode.ILSpy.Debugger.Services
{
  [ExportBookmarkPersistenceAttribute(SupportedType=typeof(BreakpointBookmark))]
  public class BreakpointPersistence : IBookmarkPersistence
  {
    #region IBookmarkPersistence Members

    public XElement Save(BookmarkBase bookmark)
    {
      var b = bookmark as BreakpointBookmark;
      XElement bElement = new XElement("Bookmark", 
        new XAttribute("functionToken", b.FunctionToken), 
        new XAttribute("isProperty", b.FunctionToken != b.MemberReference.MetadataToken.ToInt32()),
        new XAttribute("enabled", b.IsEnabled), 
        new XAttribute("ilRange", b.ILRange),
        new XAttribute("assembly", b.MemberReference.Module.FullyQualifiedName),
        new XAttribute("type", b.MemberReference.DeclaringType.FullName),
        new XAttribute("method", b.MemberReference.Name),
        new XAttribute("line", b.LineNumber)); // line number is just stored as an initial value (it will be adapted if the method is decompiled/shown)
      return bElement;
    }

    public BookmarkBase Load(XElement bookmarkNode)
    {
      string assembly = (string)bookmarkNode.Attribute("assembly");
      string type = (string)bookmarkNode.Attribute("type");
      string method = (string)bookmarkNode.Attribute("method");
      MemberReference mr = BreakpointService.ResolveMethod(assembly, type + "." + method);
      if (mr == null)
        return null;
      ILRange range;
      int functionToken;
      int line;
      if (!ILRange.TryParse((string)bookmarkNode.Attribute("ilRange"), out range)
        || !int.TryParse((string)bookmarkNode.Attribute("functionToken"), out functionToken)
        || !int.TryParse((string)bookmarkNode.Attribute("line"), out line))
        return null;
      var breakpoint = new BreakpointBookmark(mr, new AstLocation(line, 0), functionToken, range, 
                                            BreakpointAction.Break, DebugInformation.Language);
      breakpoint.IsEnabled = (bool)bookmarkNode.Attribute("enabled");
      return breakpoint;
    }

    #endregion
  }

  /// <summary>
  /// Store and load the bookmarks.
  /// </summary>
  /// <remarks>
  /// This task should be handled by BookmarkManager, but it could not reach IlSpySettings and MainWindow.
  /// </remarks>
  static class BreakpointService
  {
    static string activeAssemblyListName = null;

    public static void Initialize()
    {
      BookmarkManager.PersistenceEntries =
        App.CompositionContainer.GetExports<IBookmarkPersistence, IBookmarkPersistenceMetadata>();
      if (MainWindow.Instance.CurrentAssemblyList != null)
      {
        activeAssemblyListName = MainWindow.Instance.CurrentAssemblyList.ListName;
        LoadBookmarks();
      }
      MainWindow.Instance.CurrentAssemblyListChanged += OnCurrentAssemblyListChanged;
      MainWindow.Instance.Closing += delegate
      {
        SaveBookmarks();
        MainWindow.Instance.CurrentAssemblyListChanged -= OnCurrentAssemblyListChanged;
      };

    }

    public static MemberReference ResolveMethod(string assembly, string fullMethodName)
    {
      var foundAssembly = MainWindow.Instance.CurrentAssemblyList.OpenAssembly(assembly);
      if (null == foundAssembly)
        return null;
      if (!foundAssembly.IsLoaded)
        foundAssembly.WaitUntilLoaded();
      // Replace is done because XmlDocKeyProvider stores ".ctor" as "#ctor"            
      MemberReference mr = 
        XmlDocKeyProvider.FindMemberByKey(foundAssembly.AssemblyDefinition.MainModule, "M:" + fullMethodName.Replace("..", ".#"));
      if (mr == null)
        mr = XmlDocKeyProvider.FindMemberByKey(foundAssembly.AssemblyDefinition.MainModule, "P:" + fullMethodName.Replace("..", ".#"));
      return mr;
    }

    static void OnCurrentAssemblyListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action != NotifyCollectionChangedAction.Reset
        || activeAssemblyListName == MainWindow.Instance.CurrentAssemblyList.ListName)
        return;
      if (activeAssemblyListName != null)
        SaveBookmarks();
      BookmarkManager.Clear();
      activeAssemblyListName = MainWindow.Instance.CurrentAssemblyList.ListName;
      LoadBookmarks();
    }

    static void SaveBookmarks()
    {
      ILSpySettings.Update(
        delegate (XElement root) {
          XElement doc = root.Element("Bookmarks");
          if (doc == null)
          {
            doc = new XElement("Bookmarks");
            root.Add(doc);
          }
          XElement listElement = doc.Elements("List").FirstOrDefault(e => (string)e.Attribute("name") == activeAssemblyListName);
          if (listElement != null)
            listElement.ReplaceWith(SaveActiveBookmarks());
          else
            doc.Add(SaveActiveBookmarks());
      });
    }

    static XElement SaveActiveBookmarks()
    {
      XElement list = BookmarkManager.SaveBookmarks();
      list.SetAttributeValue("name", activeAssemblyListName);
      return list;
    }

    static void LoadBookmarks()
    {
      ILSpySettings settings = ILSpySettings.Load();
      XElement doc = settings["Bookmarks"];
      if (doc == null)
      {
        return;
      }
      XElement listElement = doc.Elements("List").FirstOrDefault(e => (string)e.Attribute("name") == activeAssemblyListName);
      if (listElement == null)
        return;
      BookmarkManager.LoadBookmarks(listElement);
    }
  }
}
