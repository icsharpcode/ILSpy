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

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.Decompiler.Util
{
	static class FileUtility
	{
		/// <summary>
		/// Gets the normalized version of fileName.
		/// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
		/// </summary>
		public static string NormalizePath(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return fileName;

			int i;

			bool isWeb = false;
			for (i = 0; i < fileName.Length; i++)
			{
				if (fileName[i] == '/' || fileName[i] == '\\')
					break;
				if (fileName[i] == ':')
				{
					if (i > 1)
						isWeb = true;
					break;
				}
			}

			char outputSeparator = '/';
			bool isRelative;
			bool isAbsoluteUnixPath = false;

			StringBuilder result = new StringBuilder();
			if (isWeb == false && IsUNCPath(fileName))
			{
				// UNC path
				i = 2;
				outputSeparator = '\\';
				result.Append(outputSeparator);
				isRelative = false;
			}
			else
			{
				i = 0;
				isAbsoluteUnixPath = fileName[0] == '/';
				isRelative = !isWeb && !isAbsoluteUnixPath && (fileName.Length < 2 || fileName[1] != ':');
				if (fileName.Length >= 2 && fileName[1] == ':')
				{
					outputSeparator = '\\';
				}
			}
			int levelsBack = 0;
			int segmentStartPos = i;
			for (; i <= fileName.Length; i++)
			{
				if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\')
				{
					int segmentLength = i - segmentStartPos;
					switch (segmentLength)
					{
						case 0:
							// ignore empty segment (if not in web mode)
							if (isWeb)
							{
								result.Append(outputSeparator);
							}
							break;
						case 1:
							// ignore /./ segment, but append other one-letter segments
							if (fileName[segmentStartPos] != '.')
							{
								if (result.Length > 0)
									result.Append(outputSeparator);
								result.Append(fileName[segmentStartPos]);
							}
							break;
						case 2:
							if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.')
							{
								// remove previous segment
								int j;
								for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--)
								{
								}
								if (j > 0)
								{
									result.Length = j;
								}
								else if (isAbsoluteUnixPath)
								{
									result.Length = 0;
								}
								else if (isRelative)
								{
									if (result.Length == 0)
										levelsBack++;
									else
										result.Length = 0;
								}
								break;
							}
							else
							{
								// append normal segment
								goto default;
							}
						default:
							if (result.Length > 0)
								result.Append(outputSeparator);
							result.Append(fileName, segmentStartPos, segmentLength);
							break;
					}
					segmentStartPos = i + 1; // remember start position for next segment
				}
			}
			if (isWeb == false)
			{
				if (isRelative)
				{
					for (int j = 0; j < levelsBack; j++)
					{
						result.Insert(0, ".." + outputSeparator);
					}
				}
				if (result.Length > 0 && result[result.Length - 1] == outputSeparator)
				{
					result.Length -= 1;
				}
				if (isAbsoluteUnixPath)
				{
					result.Insert(0, '/');
				}
				if (result.Length == 2 && result[1] == ':')
				{
					result.Append(outputSeparator);
				}
				if (result.Length == 0)
					return ".";
			}
			return result.ToString();
		}

		static bool IsUNCPath(string fileName)
		{
			return fileName.Length > 2
				&& (fileName[0] == '\\' || fileName[0] == '/')
				&& (fileName[1] == '\\' || fileName[1] == '/');
		}

		public static bool IsEqualFileName(string fileName1, string fileName2)
		{
			return string.Equals(NormalizePath(fileName1),
								 NormalizePath(fileName2),
								 StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			if (baseDirectory == null || testDirectory == null)
				return false;
			baseDirectory = NormalizePath(baseDirectory);
			if (baseDirectory == "." || baseDirectory == "")
				return !Path.IsPathRooted(testDirectory);
			baseDirectory = AddTrailingSeparator(baseDirectory);
			testDirectory = AddTrailingSeparator(NormalizePath(testDirectory));

			return testDirectory.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase);
		}

		static string AddTrailingSeparator(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;
			if (input[input.Length - 1] == Path.DirectorySeparatorChar || input[input.Length - 1] == Path.AltDirectorySeparatorChar)
				return input;
			else
				return input + GetSeparatorForPath(input).ToString();
		}

		static char GetSeparatorForPath(string input)
		{
			if (input.Length > 2 && input[1] == ':' || IsUNCPath(input))
				return '\\';
			return '/';
		}

		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			if (string.IsNullOrEmpty(baseDirectoryPath))
			{
				return absPath;
			}

			baseDirectoryPath = NormalizePath(baseDirectoryPath);
			absPath = NormalizePath(absPath);

			string[] bPath = baseDirectoryPath != "." ? baseDirectoryPath.TrimEnd(separators).Split(separators) : new string[0];
			string[] aPath = absPath != "." ? absPath.TrimEnd(separators).Split(separators) : new string[0];
			int indx = 0;
			for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
			{
				if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
					break;
			}

			if (indx == 0 && (Path.IsPathRooted(baseDirectoryPath) || Path.IsPathRooted(absPath)))
			{
				return absPath;
			}

			if (indx == bPath.Length && indx == aPath.Length)
			{
				return ".";
			}
			StringBuilder erg = new StringBuilder();
			for (int i = indx; i < bPath.Length; ++i)
			{
				erg.Append("..");
				erg.Append(Path.DirectorySeparatorChar);
			}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
			if (erg[erg.Length - 1] == Path.DirectorySeparatorChar)
				erg.Length -= 1;
			return erg.ToString();
		}

		public static string TrimPath(string path, int max_chars)
		{
			const char ellipsis = '\u2026'; // HORIZONTAL ELLIPSIS
			const int ellipsisLength = 2;

			if (path == null || path.Length <= max_chars)
				return path;
			char sep = Path.DirectorySeparatorChar;
			if (path.IndexOf(Path.AltDirectorySeparatorChar) >= 0 && path.IndexOf(Path.DirectorySeparatorChar) < 0)
			{
				sep = Path.AltDirectorySeparatorChar;
			}
			string[] parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			int len = ellipsisLength; // For initial ellipsis
			int index = parts.Length;
			// From the end of the path, fit as many parts as possible:
			while (index > 1 && len + parts[index - 1].Length < max_chars)
			{
				len += parts[index - 1].Length + 1;
				index--;
			}

			StringBuilder result = new StringBuilder();
			result.Append(ellipsis);
			// If there's 5 chars left, partially fit another part:
			if (index > 1 && len + 5 <= max_chars)
			{
				if (index == 2 && parts[0].Length <= ellipsisLength)
				{
					// If the partial part is part #1,
					// and part #0 is as short as the ellipsis
					// (e.g. just a drive letter), use part #0
					// instead of the ellipsis.
					result.Clear();
					result.Append(parts[0]);
				}
				result.Append(sep);
				result.Append(parts[index - 1], 0, max_chars - len - 3);
				result.Append(ellipsis);
			}
			while (index < parts.Length)
			{
				result.Append(sep);
				result.Append(parts[index]);
				index++;
			}
			return result.ToString();
		}
	}
}
