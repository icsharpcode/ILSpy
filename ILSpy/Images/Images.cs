// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSharpCode.ILSpy
{
	static class Images
	{
		static ImageSource Load(string icon)
		{
			var image = new DrawingImage(LoadDrawingGroup(null, "Images/" + icon));
			if (image.CanFreeze)
			{
				image.Freeze();
			}

			return image;
		}

		public static readonly ImageSource ILSpyIcon = new BitmapImage(new Uri("pack://application:,,,/ILSpy;component/images/ILSpy.ico"));

		public static readonly ImageSource ViewCode = Load("ViewCode");
		public static readonly ImageSource Save = Load("Save");
		public static readonly ImageSource OK = Load("OK");

		public static readonly ImageSource Delete = Load("Delete");
		public static readonly ImageSource Search = Load("Search");

		public static readonly ImageSource Assembly = Load("Assembly");
		public static readonly ImageSource AssemblyWarning = Load("AssemblyWarning");
		public static readonly ImageSource FindAssembly = Load("FindAssembly");

		public static readonly ImageSource Library = Load("Library");
		public static readonly ImageSource Namespace = Load("Namespace");

		public static readonly ImageSource ReferenceFolder = Load("ReferenceFolder");
		public static readonly ImageSource NuGet = Load(null, "Images/NuGet.png");

		public static readonly ImageSource SubTypes = Load("SubTypes");
		public static readonly ImageSource SuperTypes = Load("SuperTypes");

		public static readonly ImageSource FolderOpen = Load("Folder.Open");
		public static readonly ImageSource FolderClosed = Load("Folder.Closed");

		public static readonly ImageSource Resource = Load("Resource");
		public static readonly ImageSource ResourceImage = Load("ResourceImage");
		public static readonly ImageSource ResourceResourcesFile = Load("ResourceResourcesFile");
		public static readonly ImageSource ResourceXml = Load("ResourceXml");
		public static readonly ImageSource ResourceXsd = Load("ResourceXslt");
		public static readonly ImageSource ResourceXslt = Load("ResourceXslt");

		public static readonly ImageSource Class = Load("Class");
		public static readonly ImageSource Struct = Load("Struct");
		public static readonly ImageSource Interface = Load("Interface");
		public static readonly ImageSource Delegate = Load("Delegate");
		public static readonly ImageSource Enum = Load("Enum");

		public static readonly ImageSource Field = Load("Field");
		public static readonly ImageSource FieldReadOnly = Load("FieldReadOnly");
		public static readonly ImageSource Literal = Load("Literal");
		public static readonly ImageSource EnumValue = Load("EnumValue");

		public static readonly ImageSource Method = Load("Method");
		public static readonly ImageSource Constructor = Load("Constructor");
		public static readonly ImageSource VirtualMethod = Load("VirtualMethod");
		public static readonly ImageSource Operator = Load("Operator");
		public static readonly ImageSource ExtensionMethod = Load("ExtensionMethod");
		public static readonly ImageSource PInvokeMethod = Load("PInvokeMethod");

		public static readonly ImageSource Property = Load("Property");
		public static readonly ImageSource Indexer = Load("Indexer");

		public static readonly ImageSource Event = Load("Event");

		private static readonly ImageSource OverlayProtected = Load("OverlayProtected");
		private static readonly ImageSource OverlayInternal = Load("OverlayInternal");
		private static readonly ImageSource OverlayProtectedInternal = Load("OverlayProtectedInternal");
		private static readonly ImageSource OverlayPrivate = Load("OverlayPrivate");
		private static readonly ImageSource OverlayPrivateProtected = Load("OverlayPrivateProtected");
		private static readonly ImageSource OverlayCompilerControlled = Load("OverlayCompilerControlled");

		private static readonly ImageSource OverlayStatic = Load("OverlayStatic");

		public static ImageSource Load(object part, string icon)
		{
			if (icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				return LoadImage(part, icon);
			Uri uri = GetUri(part, icon + ".xaml");
			if (ResourceExists(uri))
			{
				var image = new DrawingImage(LoadDrawingGroup(part, icon));
				if (image.CanFreeze)
				{
					image.Freeze();
				}
				return image;
			}
			return LoadImage(part, icon + ".png");
		}

		static BitmapImage LoadImage(object part, string icon)
		{
			Uri uri = GetUri(part, icon);
			BitmapImage image = new BitmapImage(uri);
			if (image.CanFreeze)
			{
				image.Freeze();
			}
			return image;
		}

		public static Drawing LoadDrawingGroup(object part, string icon)
		{
			return (Drawing)Application.LoadComponent(GetUri(part, icon + ".xaml", absolute: false));
		}

		private static Uri GetUri(object part, string icon, bool absolute = true)
		{
			Uri uri;
			var assembly = part?.GetType().Assembly;
			string prefix;
			UriKind kind;
			if (absolute)
			{
				prefix = "pack://application:,,,/";
				kind = UriKind.Absolute;
			}
			else
			{
				prefix = "/";
				kind = UriKind.Relative;
			}
			if (part == null || assembly == typeof(Images).Assembly)
			{
				uri = new Uri(prefix + icon, kind);
			}
			else
			{
				var name = assembly.GetName();
				uri = new Uri(prefix + name.Name + ";v" + name.Version + ";component/" + icon, kind);
			}

			return uri;
		}

		private static bool ResourceExists(Uri uri)
		{
			try
			{
				Application.GetResourceStream(uri);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
		}

		private static readonly TypeIconCache typeIconCache = new TypeIconCache();
		private static readonly MemberIconCache memberIconCache = new MemberIconCache();

		public static ImageSource GetIcon(TypeIcon icon, AccessOverlayIcon overlay, bool isStatic = false)
		{
			lock (typeIconCache)
				return typeIconCache.GetIcon(icon, overlay, isStatic);
		}

		public static ImageSource GetIcon(MemberIcon icon, AccessOverlayIcon overlay, bool isStatic)
		{
			lock (memberIconCache)
				return memberIconCache.GetIcon(icon, overlay, isStatic);
		}

		#region icon caches & overlay management

		private class TypeIconCache : IconCache<TypeIcon>
		{
			public TypeIconCache()
			{
				PreloadPublicIconToCache(TypeIcon.Class, Images.Class);
				PreloadPublicIconToCache(TypeIcon.Enum, Images.Enum);
				PreloadPublicIconToCache(TypeIcon.Struct, Images.Struct);
				PreloadPublicIconToCache(TypeIcon.Interface, Images.Interface);
				PreloadPublicIconToCache(TypeIcon.Delegate, Images.Delegate);
			}

			protected override ImageSource GetBaseImage(TypeIcon icon)
			{
				ImageSource baseImage;
				switch (icon)
				{
					case TypeIcon.Class:
						baseImage = Images.Class;
						break;
					case TypeIcon.Enum:
						baseImage = Images.Enum;
						break;
					case TypeIcon.Struct:
						baseImage = Images.Struct;
						break;
					case TypeIcon.Interface:
						baseImage = Images.Interface;
						break;
					case TypeIcon.Delegate:
						baseImage = Images.Delegate;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(icon), $"TypeIcon.{icon} is not supported!");
				}

				return baseImage;
			}
		}

		private class MemberIconCache : IconCache<MemberIcon>
		{
			public MemberIconCache()
			{
				PreloadPublicIconToCache(MemberIcon.Field, Images.Field);
				PreloadPublicIconToCache(MemberIcon.FieldReadOnly, Images.FieldReadOnly);
				PreloadPublicIconToCache(MemberIcon.Literal, Images.Literal);
				PreloadPublicIconToCache(MemberIcon.EnumValue, Images.EnumValue);
				PreloadPublicIconToCache(MemberIcon.Property, Images.Property);
				PreloadPublicIconToCache(MemberIcon.Indexer, Images.Indexer);
				PreloadPublicIconToCache(MemberIcon.Method, Images.Method);
				PreloadPublicIconToCache(MemberIcon.Constructor, Images.Constructor);
				PreloadPublicIconToCache(MemberIcon.VirtualMethod, Images.VirtualMethod);
				PreloadPublicIconToCache(MemberIcon.Operator, Images.Operator);
				PreloadPublicIconToCache(MemberIcon.ExtensionMethod, Images.ExtensionMethod);
				PreloadPublicIconToCache(MemberIcon.PInvokeMethod, Images.PInvokeMethod);
				PreloadPublicIconToCache(MemberIcon.Event, Images.Event);
			}

			protected override ImageSource GetBaseImage(MemberIcon icon)
			{
				ImageSource baseImage;
				switch (icon)
				{
					case MemberIcon.Field:
						baseImage = Images.Field;
						break;
					case MemberIcon.FieldReadOnly:
						baseImage = Images.FieldReadOnly;
						break;
					case MemberIcon.Literal:
						baseImage = Images.Literal;
						break;
					case MemberIcon.EnumValue:
						baseImage = Images.EnumValue;
						break;
					case MemberIcon.Property:
						baseImage = Images.Property;
						break;
					case MemberIcon.Indexer:
						baseImage = Images.Indexer;
						break;
					case MemberIcon.Method:
						baseImage = Images.Method;
						break;
					case MemberIcon.Constructor:
						baseImage = Images.Constructor;
						break;
					case MemberIcon.VirtualMethod:
						baseImage = Images.VirtualMethod;
						break;
					case MemberIcon.Operator:
						baseImage = Images.Operator;
						break;
					case MemberIcon.ExtensionMethod:
						baseImage = Images.ExtensionMethod;
						break;
					case MemberIcon.PInvokeMethod:
						baseImage = Images.PInvokeMethod;
						break;
					case MemberIcon.Event:
						baseImage = Images.Event;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(icon), $"MemberIcon.{icon} is not supported!");
				}

				return baseImage;
			}
		}

		private abstract class IconCache<T>
		{
			private readonly Dictionary<(T, AccessOverlayIcon, bool), ImageSource> cache = new Dictionary<(T, AccessOverlayIcon, bool), ImageSource>();

			protected void PreloadPublicIconToCache(T icon, ImageSource image)
			{
				var iconKey = (icon, AccessOverlayIcon.Public, false);
				cache.Add(iconKey, image);
			}

			public ImageSource GetIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				var iconKey = (icon, overlay, isStatic);
				if (cache.ContainsKey(iconKey))
				{
					return cache[iconKey];
				}
				else
				{
					ImageSource result = BuildMemberIcon(icon, overlay, isStatic);
					cache.Add(iconKey, result);
					return result;
				}
			}

			private ImageSource BuildMemberIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				ImageSource baseImage = GetBaseImage(icon);
				ImageSource overlayImage = GetOverlayImage(overlay);

				return CreateOverlayImage(baseImage, overlayImage, isStatic);
			}

			protected abstract ImageSource GetBaseImage(T icon);

			private static ImageSource GetOverlayImage(AccessOverlayIcon overlay)
			{
				ImageSource overlayImage;
				switch (overlay)
				{
					case AccessOverlayIcon.Public:
						overlayImage = null;
						break;
					case AccessOverlayIcon.Protected:
						overlayImage = Images.OverlayProtected;
						break;
					case AccessOverlayIcon.Internal:
						overlayImage = Images.OverlayInternal;
						break;
					case AccessOverlayIcon.ProtectedInternal:
						overlayImage = Images.OverlayProtectedInternal;
						break;
					case AccessOverlayIcon.Private:
						overlayImage = Images.OverlayPrivate;
						break;
					case AccessOverlayIcon.PrivateProtected:
						overlayImage = Images.OverlayPrivateProtected;
						break;
					case AccessOverlayIcon.CompilerControlled:
						overlayImage = Images.OverlayCompilerControlled;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(overlay), $"AccessOverlayIcon.{overlay} is not supported!");
				}
				return overlayImage;
			}

			private static readonly Rect iconRect = new Rect(0, 0, 16, 16);

			private static ImageSource CreateOverlayImage(ImageSource baseImage, ImageSource overlay, bool isStatic)
			{
				var group = new DrawingGroup();

				Drawing baseDrawing = new ImageDrawing(baseImage, iconRect);

				if (overlay != null)
				{
					var nestedGroup = new DrawingGroup { Transform = new ScaleTransform(0.8, 0.8) };
					nestedGroup.Children.Add(baseDrawing);
					group.Children.Add(nestedGroup);
					group.Children.Add(new ImageDrawing(overlay, iconRect));
				}
				else
				{
					group.Children.Add(baseDrawing);
				}

				if (isStatic)
				{
					group.Children.Add(new ImageDrawing(Images.OverlayStatic, iconRect));
				}

				var image = new DrawingImage(group);
				if (image.CanFreeze)
				{
					image.Freeze();
				}
				return image;
			}
		}

		#endregion
	}
}
