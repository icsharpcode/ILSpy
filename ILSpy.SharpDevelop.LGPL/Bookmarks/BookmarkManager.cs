// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using Mono.CSharp;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using System.Linq;

namespace ICSharpCode.ILSpy.Bookmarks
{
  /// <summary>
  /// Bookmark specializations may provide an exported implementation of this interface to support save and load for these bookmarks.
  /// </summary>
  public interface IBookmarkPersistence
  {
    XElement Save(BookmarkBase bookmark);
    BookmarkBase Load(XElement bookmarkNode);
  }

  public interface IBookmarkPersistenceMetadata
  {
    Type SupportedType { get; }
  }

  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class ExportBookmarkPersistenceAttribute : ExportAttribute, IBookmarkPersistenceMetadata
  {
    public ExportBookmarkPersistenceAttribute()
      : base(typeof(IBookmarkPersistence))
    {
    }

    public Type SupportedType { get; set; }
  }

	/// <summary>
	/// Static class that maintains the list of bookmarks and breakpoints.
	/// </summary>
	public static partial class BookmarkManager
	{
		static List<BookmarkBase> bookmarks = new List<BookmarkBase>();
		
		public static List<BookmarkBase> Bookmarks {
			get {
				return bookmarks;
			}
		}
		
		public static List<BookmarkBase> GetBookmarks(string typeName)
		{
			if (typeName == null)
				throw new ArgumentNullException("typeName");
			
			List<BookmarkBase> marks = new List<BookmarkBase>();
			
			foreach (BookmarkBase mark in bookmarks) {
				if (typeName == mark.MemberReference.FullName) {
					marks.Add(mark);
				}
			}
			
			return marks;
		}
		
		public static void AddMark(BookmarkBase bookmark)
		{
			if (bookmark == null) return;
			if (bookmarks.Contains(bookmark)) return;
			if (bookmarks.Exists(b => IsEqualBookmark(b, bookmark))) return;
			bookmarks.Add(bookmark);
			OnAdded(new BookmarkEventArgs(bookmark));
		}
		
		static bool IsEqualBookmark(BookmarkBase a, BookmarkBase b)
		{
			if (a == b)
				return true;
			if (a == null || b == null)
				return false;
			if (a.GetType() != b.GetType())
				return false;
			if (a.MemberReference.FullName != b.MemberReference.FullName)
				return false;
			return a.LineNumber == b.LineNumber;
		}
		
		public static void RemoveMark(BookmarkBase bookmark)
		{
			bookmarks.Remove(bookmark);
			OnRemoved(new BookmarkEventArgs(bookmark));
		}
		
		public static void Clear()
		{
			while (bookmarks.Count > 0) {
				var b = bookmarks[bookmarks.Count - 1];
				bookmarks.RemoveAt(bookmarks.Count - 1);
				OnRemoved(new BookmarkEventArgs(b));
			}
		}
		
		internal static void Initialize()
		{
			
		}
		
		static void OnRemoved(BookmarkEventArgs e)
		{
			if (Removed != null) {
				Removed(null, e);
			}
		}
		
		static void OnAdded(BookmarkEventArgs e)
		{
			if (Added != null) {
				Added(null, e);
			}
		}
		
		public static void ToggleBookmark(string typeName, int line,
		                                  Predicate<BookmarkBase> canToggle,
		                                  Func<AstLocation, BookmarkBase> bookmarkFactory)
		{
			foreach (BookmarkBase bookmark in GetBookmarks(typeName)) {
				if (canToggle(bookmark) && bookmark.LineNumber == line) {
					BookmarkManager.RemoveMark(bookmark);
					return;
				}
			}
			
			// no bookmark at that line: create a new bookmark
			BookmarkManager.AddMark(bookmarkFactory(new AstLocation(line, 0)));
		}
		
		public static event BookmarkEventHandler Removed;
		public static event BookmarkEventHandler Added;

    #region persistence
    [ImportMany(typeof(IBookmarkPersistence))]
    public static IEnumerable<Lazy<IBookmarkPersistence, IBookmarkPersistenceMetadata>> PersistenceEntries { get; set; }

    public static XElement SaveBookmarks()
    {
      XElement list = new XElement("List");
      foreach (var b in BookmarkManager.Bookmarks)
      {
        var persistenceHandler = PersistenceEntries.FirstOrDefault(e => e.Metadata.SupportedType == b.GetType());
        if (null == persistenceHandler)
          continue;
        var bookmarkNode = persistenceHandler.Value.Save(b);
        bookmarkNode.SetAttributeValue("bookmarkType", b.GetType().Name);
        list.Add(bookmarkNode);
      }
      return list;
    }

    public static void LoadBookmarks(XElement bookmarkList)
    {
      foreach (var bookmarkNode in bookmarkList.Elements("Bookmark"))
      {
        var persistenceHandler = 
          PersistenceEntries.FirstOrDefault(e => e.Metadata.SupportedType.Name == (string)bookmarkNode.Attribute("bookmarkType"));
        if (null == persistenceHandler)
          continue;
        BookmarkBase newMark = persistenceHandler.Value.Load(bookmarkNode);
        AddMark(newMark);
      }
    }
    #endregion
  }
}
