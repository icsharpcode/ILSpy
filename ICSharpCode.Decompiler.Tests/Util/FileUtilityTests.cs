// Copyright (c) 2020 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	[TestFixture]
	public class FileUtilityTests
	{
		#region NormalizePath
		[Test]
		public void NormalizePath()
		{
			Assert.That(FileUtility.NormalizePath(@"c:\temp\project\..\test.txt"), Is.EqualTo(@"c:\temp\test.txt"));
			Assert.That(FileUtility.NormalizePath(@"c:\temp\project\.\..\test.txt"), Is.EqualTo(@"c:\temp\test.txt"));
			Assert.That(FileUtility.NormalizePath(@"c:\temp\\test.txt"), Is.EqualTo(@"c:\temp\test.txt")); // normalize double backslash
			Assert.That(FileUtility.NormalizePath(@"c:\temp\."), Is.EqualTo(@"c:\temp"));
			Assert.That(FileUtility.NormalizePath(@"c:\temp\subdir\.."), Is.EqualTo(@"c:\temp"));
		}

		[Test]
		public void NormalizePath_DriveRoot()
		{
			Assert.That(FileUtility.NormalizePath(@"C:\"), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:/"), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:"), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:/."), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:/.."), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:/./"), Is.EqualTo(@"C:\"));
			Assert.That(FileUtility.NormalizePath(@"C:/..\"), Is.EqualTo(@"C:\"));
		}

		[Test]
		public void NormalizePath_UNC()
		{
			Assert.That(FileUtility.NormalizePath(@"\\server\share"), Is.EqualTo(@"\\server\share"));
			Assert.That(FileUtility.NormalizePath(@"\\server\share\"), Is.EqualTo(@"\\server\share"));
			Assert.That(FileUtility.NormalizePath(@"//server/share/"), Is.EqualTo(@"\\server\share"));
			Assert.That(FileUtility.NormalizePath(@"//server/share/dir/..\otherdir"), Is.EqualTo(@"\\server\share\otherdir"));
		}

		[Test]
		public void NormalizePath_Web()
		{
			Assert.That(FileUtility.NormalizePath(@"http://danielgrunwald.de/path/"), Is.EqualTo(@"http://danielgrunwald.de/path/"));
			Assert.That(FileUtility.NormalizePath(@"browser://http://danielgrunwald.de/wrongpath/../path/"), Is.EqualTo(@"browser://http://danielgrunwald.de/path/"));
		}

		[Test]
		public void NormalizePath_Relative()
		{
			Assert.That(FileUtility.NormalizePath(@"..\a\..\b"), Is.EqualTo(@"../b"));
			Assert.That(FileUtility.NormalizePath(@"."), Is.EqualTo(@"."));
			Assert.That(FileUtility.NormalizePath(@"a\.."), Is.EqualTo(@"."));
		}

		[Test]
		public void NormalizePath_UnixStyle()
		{
			Assert.That(FileUtility.NormalizePath("/"), Is.EqualTo("/"));
			Assert.That(FileUtility.NormalizePath("/a/b"), Is.EqualTo("/a/b"));
			Assert.That(FileUtility.NormalizePath("/c/../a/./b"), Is.EqualTo("/a/b"));
			Assert.That(FileUtility.NormalizePath("/c/../../a/./b"), Is.EqualTo("/a/b"));
		}
		#endregion

		[Test]
		public void TestIsBaseDirectory()
		{
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A\b\hello"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a\"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a\", @"C:\a"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a", @"C:\a\"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\A", @"C:\a"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a", @"C:\A"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\a\x\fWufhweoe", @"C:\a\x\fwuFHweoe\a\b\hello"));

			Assert.That(FileUtility.IsBaseDirectory(@"C:\b\..\A", @"C:\a"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\HELLO\..\B\..\a", @"C:\b\..\a"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\.\B\..\.\.\a", @"C:\.\.\.\.\.\.\.\a"));

			Assert.That(!FileUtility.IsBaseDirectory(@"C:\b", @"C:\a\b\hello"));
			Assert.That(!FileUtility.IsBaseDirectory(@"C:\a\b\hello", @"C:\b"));
			Assert.That(!FileUtility.IsBaseDirectory(@"C:\a\x\fwufhweoe", @"C:\a\x\fwuFHweoex\a\b\hello"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\", @"C:\"));
			Assert.That(FileUtility.IsBaseDirectory(@"C:\", @"C:\a\b\hello"));
			Assert.That(!FileUtility.IsBaseDirectory(@"C:\", @"D:\a\b\hello"));
		}


		[Test]
		public void TestIsBaseDirectoryRelative()
		{
			Assert.That(FileUtility.IsBaseDirectory(@".", @"a\b"));
			Assert.That(FileUtility.IsBaseDirectory(@".", @"a"));
			Assert.That(!FileUtility.IsBaseDirectory(@".", @"c:\"));
			Assert.That(!FileUtility.IsBaseDirectory(@".", @"/"));
		}

		[Test]
		public void TestIsBaseDirectoryUnixStyle()
		{
			Assert.That(FileUtility.IsBaseDirectory(@"/", @"/"));
			Assert.That(FileUtility.IsBaseDirectory(@"/", @"/a"));
			Assert.That(FileUtility.IsBaseDirectory(@"/", @"/a/subdir"));
		}

		[Test]
		public void TestIsBaseDirectoryUNC()
		{
			Assert.That(FileUtility.IsBaseDirectory(@"\\server\share", @"\\server\share\dir\subdir"));
			Assert.That(FileUtility.IsBaseDirectory(@"\\server\share", @"\\server\share\dir\subdir"));
			Assert.That(!FileUtility.IsBaseDirectory(@"\\server2\share", @"\\server\share\dir\subdir"));
		}

		[Test]
		public void TestGetRelativePath()
		{
			Assert.That(FileUtility.GetRelativePath(@"C:\hello\.\..\a", @"C:\.\a\blub"), Is.EqualTo(@"blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\.\.\.\.\hello", @"C:\.\blub\.\..\.\a\.\blub"), Is.EqualTo(@"..\a\blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\.\.\.\.\hello\", @"C:\.\blub\.\..\.\a\.\blub"), Is.EqualTo(@"..\a\blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\hello", @"C:\.\hello"), Is.EqualTo(@"."));
			Assert.That(FileUtility.GetRelativePath(@"C:\", @"C:\"), Is.EqualTo(@"."));
			Assert.That(FileUtility.GetRelativePath(@"C:\", @"C:\blub"), Is.EqualTo(@"blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\", @"D:\"), Is.EqualTo(@"D:\"));
			Assert.That(FileUtility.GetRelativePath(@"C:\abc", @"D:\def"), Is.EqualTo(@"D:\def"));

			// casing troubles
			Assert.That(FileUtility.GetRelativePath(@"C:\hello\.\..\A", @"C:\.\a\blub"), Is.EqualTo(@"blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\.\.\.\.\HELlo", @"C:\.\blub\.\..\.\a\.\blub"), Is.EqualTo(@"..\a\blub"));
			Assert.That(FileUtility.GetRelativePath(@"C:\.\.\.\.\heLLo\A\..", @"C:\.\blub\.\..\.\a\.\blub"), Is.EqualTo(@"..\a\blub"));
		}

		[Test]
		public void RelativeGetRelativePath()
		{
			// Relative path
			Assert.That(FileUtility.GetRelativePath(@".", @"a"), Is.EqualTo(@"a"));
			Assert.That(FileUtility.GetRelativePath(@"a", @"."), Is.EqualTo(@".."));
			Assert.That(FileUtility.GetRelativePath(@"a", @"b"), Is.EqualTo(@"..\b"));
			Assert.That(FileUtility.GetRelativePath(@"a", @".."), Is.EqualTo(@"..\.."));

			// Getting a path from an absolute path to a relative path isn't really possible;
			// so we just keep the existing relative path (don't introduce incorrect '..\').
			Assert.That(FileUtility.GetRelativePath(@"C:\abc", @"def"), Is.EqualTo(@"def"));
		}

		[Test]
		public void GetRelativePath_Unix()
		{
			Assert.That(FileUtility.GetRelativePath("/", "/a"), Is.EqualTo(@"a"));
			Assert.That(FileUtility.GetRelativePath("/", "/a/b"), Is.EqualTo(@"a\b"));
			Assert.That(FileUtility.GetRelativePath("/a", "/a/b"), Is.EqualTo(@"b"));
		}

		[Test]
		public void TestIsEqualFile()
		{
			Assert.That(FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO WOrld.exe"));
			Assert.That(FileUtility.IsEqualFileName(@"C:\bla\..\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\a\MY.FILE.IS.THIS"));

			Assert.That(!FileUtility.IsEqualFileName(@"C:\.\Hello World.Exe", @"C:\HELLO_WOrld.exe"));
			Assert.That(!FileUtility.IsEqualFileName(@"C:\a\my.file.is.this", @"C:\gg\..\.\.\.\.\a\..\b\MY.FILE.IS.THIS"));
		}
	}
}