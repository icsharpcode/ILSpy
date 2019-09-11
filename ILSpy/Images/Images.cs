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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Windows.Shapes;

namespace ICSharpCode.ILSpy
{
	static class Images
	{
		static BitmapImage LoadBitmap(string name)
		{
			BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Images/" + name + ".png"));
			image.Freeze();
			return image;
		}

		static object Load(string icon)
		{
			icon = "Images/" + icon;
			if (icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				return LoadImage(null, icon);
			Uri uri = GetUri(null, icon + ".xaml");
			if (ResourceExists(uri)) {
				return LoadDrawingGroup(null, icon);
			}
			return LoadImage(null, icon + ".png");
		}

		public static readonly BitmapImage Breakpoint = LoadBitmap("Breakpoint");
		public static readonly BitmapImage CurrentLine = LoadBitmap("CurrentLine");

		public static readonly BitmapImage ViewCode = LoadBitmap("ViewCode");
		public static readonly BitmapImage Save = LoadBitmap("SaveFile");
		public static readonly BitmapImage OK = LoadBitmap("OK");

		public static readonly BitmapImage Delete = LoadBitmap("Delete");
		public static readonly BitmapImage Search = LoadBitmap("Search");

		public static readonly object Assembly = Load("Assembly");
		public static readonly object AssemblyWarning = Load("AssemblyWarning");
		public static readonly object AssemblyLoading = Load("FindAssembly");

		public static readonly object Library = Load("Library");
		public static readonly object Namespace = Load("Namespace");

		public static readonly object ReferenceFolder = Load("ReferenceFolder");

		public static readonly BitmapImage SubTypes = LoadBitmap("SubTypes");
		public static readonly BitmapImage SuperTypes = LoadBitmap("SuperTypes");

		public static readonly object FolderOpen = Load("Folder.Open");
		public static readonly object FolderClosed = Load("Folder.Closed");

		public static readonly object Resource = Load("Resource");
		public static readonly object ResourceImage = Load("ResourceImage");
		public static readonly BitmapImage ResourceResourcesFile = LoadBitmap("ResourceResourcesFile");
		public static readonly object ResourceXml = Load("ResourceXml");
		public static readonly object ResourceXsd = Load("ResourceXslt");
		public static readonly object ResourceXslt = Load("ResourceXslt");

		public static readonly object Class = Load("Class");
		public static readonly object Struct = Load("Struct");
		public static readonly object Interface = Load("Interface");
		public static readonly object Delegate = Load("Delegate");
		public static readonly object Enum = Load("Enum");
		public static readonly object StaticClass = Load("StaticClass");


		public static readonly object Field = Load("Field");
		public static readonly object FieldReadOnly = Load("Field");
		public static readonly object Literal = Load("Literal");
		public static readonly object EnumValue = Load("EnumValue");

		public static readonly object Method = Load("Method");
		public static readonly object Constructor = Load("Method");
		public static readonly object VirtualMethod = Load("Method");
		public static readonly object Operator = Load("Operator");
		public static readonly object ExtensionMethod = Load("ExtensionMethod");
		public static readonly object PInvokeMethod = Load("Method");

		public static readonly object Property = Load("Property");
		public static readonly object Indexer = Load("Indexer");

		public static readonly object Event = Load("Event");

		private static readonly object OverlayProtected = Load("OverlayProtected");
		private static readonly object OverlayInternal = Load("OverlayInternal");
		private static readonly object OverlayProtectedInternal = Load("OverlayProtectedInternal");
		private static readonly object OverlayPrivate = Load("OverlayPrivate");
		private static readonly object OverlayPrivateProtected = Load("OverlayPrivateProtected");
		private static readonly object OverlayCompilerControlled = Load("OverlayCompilerControlled");

		private static readonly object OverlayStatic = Load("OverlayStatic");

		public static object GetIcon(object imageOrVector)
		{
			if (imageOrVector is BitmapImage img)
				return img;
			return new Rectangle {
				Width = 16,
				Height = 16,
				Fill = new DrawingBrush((DrawingGroup)imageOrVector)
			};
		}

		public static object Load(object part, string icon)
		{
			if (icon.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
				return LoadImage(part, icon);
			Uri uri = GetUri(part, icon + ".xaml");
			if (ResourceExists(uri)) {
				return LoadVector(part, icon);
			}
			return LoadImage(part, icon + ".png");
		}

		public static BitmapImage LoadImage(object part, string icon)
		{
			Uri uri = GetUri(part, icon);
			BitmapImage image = new BitmapImage(uri);
			image.Freeze();
			return image;
		}

		public static Viewbox LoadVector(object part, string icon)
		{
			return (Viewbox)Application.LoadComponent(GetUri(part, icon + ".xaml", absolute: false));
		}

		public static DrawingGroup LoadDrawingGroup(object part, string icon)
		{
			return (DrawingGroup)Application.LoadComponent(GetUri(part, icon + ".xaml", absolute: false));
		}

		private static Uri GetUri(object part, string icon, bool absolute = true)
		{
			Uri uri;
			var assembly = part?.GetType().Assembly;
			string prefix;
			UriKind kind;
			if (absolute) {
				prefix = "pack://application:,,,/";
				kind = UriKind.Absolute;
			} else {
				prefix = "/";
				kind = UriKind.Relative;
			}
			if (part == null || assembly == typeof(Images).Assembly) {
				uri = new Uri(prefix + icon, kind);
			} else {
				var name = assembly.GetName();
				uri = new Uri(prefix + name.Name + ";v" + name.Version + ";component/" + icon, kind);
			}

			return uri;
		}

		private static bool ResourceExists(Uri uri)
		{
			try {
				Application.GetResourceStream(uri);
				return true;
			} catch (IOException) {
				return false;
			}
		}

		private static readonly TypeIconCache typeIconCache = new TypeIconCache();
		private static readonly MemberIconCache memberIconCache = new MemberIconCache();

		public static object GetIcon(TypeIcon icon, AccessOverlayIcon overlay, bool isStatic = false)
		{
			lock (typeIconCache)
				return typeIconCache.GetIcon(icon, overlay, isStatic);
		}

		public static object GetIcon(MemberIcon icon, AccessOverlayIcon overlay, bool isStatic)
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

			protected override object GetBaseImage(TypeIcon icon)
			{
				object baseImage;
				switch (icon) {
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

			protected override object GetBaseImage(MemberIcon icon)
			{
				object baseImage;
				switch (icon) {
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
						baseImage = Images.Literal;
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
			private readonly Dictionary<(T, AccessOverlayIcon, bool), object> cache = new Dictionary<(T, AccessOverlayIcon, bool), object>();

			protected void PreloadPublicIconToCache(T icon, object image)
			{
				var iconKey = (icon, AccessOverlayIcon.Public, false);
				if (image is ImageSource img)
					cache.Add(iconKey, img);
				else
					cache.Add(iconKey, new DrawingImage((DrawingGroup)image));
			}

			public object GetIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				var iconKey = (icon, overlay, isStatic);
				if (cache.ContainsKey(iconKey)) {
					return cache[iconKey];
				} else {
					object result = BuildMemberIcon(icon, overlay, isStatic);
					cache.Add(iconKey, result);
					return result;
				}
			}

			private object BuildMemberIcon(T icon, AccessOverlayIcon overlay, bool isStatic)
			{
				object baseImage = GetBaseImage(icon);
				object overlayImage = GetOverlayImage(overlay);

				return CreateOverlayImage(baseImage, overlayImage, isStatic);
			}

			protected abstract object GetBaseImage(T icon);

			private static object GetOverlayImage(AccessOverlayIcon overlay)
			{
				object overlayImage;
				switch (overlay) {
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

			private static ImageSource CreateOverlayImage(object baseImage, object overlay, bool isStatic)
			{
				var group = new DrawingGroup();

				Drawing baseDrawing;
				if (baseImage is ImageSource img) {
					baseDrawing = new ImageDrawing(img, iconRect);
				} else {
					baseDrawing = (DrawingGroup)baseImage;
				}

				if (overlay != null) {
					var nestedGroup = new DrawingGroup { Transform = new ScaleTransform(0.8, 0.8) };
					nestedGroup.Children.Add(baseDrawing);
					group.Children.Add(nestedGroup);

					if (overlay is ImageSource overlayImage)
						group.Children.Add(new ImageDrawing(overlayImage, iconRect));
					else
						group.Children.Add((DrawingGroup)overlay);
				} else {
					group.Children.Add(baseDrawing);
				}

				if (isStatic) {
					if (Images.OverlayStatic is ImageSource staticImg)
						group.Children.Add(new ImageDrawing(staticImg, iconRect));
					else
						group.Children.Add((DrawingGroup)Images.OverlayStatic);
				}

				var image = new DrawingImage(group);
				image.Freeze();
				return image;
			}
		}

		#endregion
	}
}
