// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Windows.Media;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	public class XmlBamlReader : XmlReader, IXmlNamespaceResolver
	{
		BamlBinaryReader reader;
		Dictionary<short, string> assemblyTable = new Dictionary<short, string>();
		Dictionary<short, string> stringTable = new Dictionary<short, string>();
		Dictionary<short, TypeDeclaration> typeTable = new Dictionary<short, TypeDeclaration>();
		Dictionary<short, PropertyDeclaration> propertyTable = new Dictionary<short, PropertyDeclaration>();

		readonly ITypeResolver _resolver;

		BamlRecordType currentType;

		Stack<XmlBamlElement> elements = new Stack<XmlBamlElement>();
		Stack<XmlBamlElement> readingElements = new Stack<XmlBamlElement>();
		NodesCollection nodes = new NodesCollection();
		List<XmlToClrNamespaceMapping> mappings = new List<XmlToClrNamespaceMapping>();
		XmlBamlNode _currentNode;

		readonly KnownInfo KnownInfo;

		int complexPropertyOpened = 0;

		bool intoAttribute = false;
		bool initialized;
		bool _eof;
		
		#region Context
		Stack<ReaderContext> layer = new Stack<ReaderContext>();
		
		class ReaderContext
		{
			public bool IsDeferred { get; set; }
			public bool IsInStaticResource { get; set; }

			public ReaderContext Previous { get; private set; }
			
			public ReaderContext()
			{
				Previous = this;
			}
			
			public ReaderContext(ReaderContext previous)
			{
				Previous = previous;
			}
		}
		
		ReaderContext Current {
			get {
				if (!layer.Any())
					layer.Push(new ReaderContext());
				
				return layer.Peek();
			}
		}
		
		int currentKey;
		List<KeyMapping> keys = new List<KeyMapping>();
		
		void LayerPop()
		{
			layer.Pop();
		}
		
		void LayerPush()
		{
			if (layer.Any())
				layer.Push(new ReaderContext(layer.Peek()));
			else
				layer.Push(new ReaderContext());
		}
		#endregion

		int bytesToSkip;

		static readonly MethodInfo staticConvertCustomBinaryToObjectMethod = Type.GetType("System.Windows.Markup.XamlPathDataSerializer,PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35").GetMethod("StaticConvertCustomBinaryToObject", BindingFlags.Static | BindingFlags.Public);
		readonly TypeDeclaration XamlTypeDeclaration;
		readonly XmlNameTable _nameTable = new NameTable();
		IDictionary<string, string> _rootNamespaces;
		
		public const string XWPFNamespace = XmlToClrNamespaceMapping.XamlNamespace;
		public const string DefaultWPFNamespace = XmlToClrNamespaceMapping.PresentationNamespace;

		public ISet<XmlNamespace> XmlnsDefinitions { get; } = new HashSet<XmlNamespace>();

		public XmlBamlReader(Stream stream, ITypeResolver resolver)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (resolver == null)
				throw new ArgumentNullException(nameof(resolver));

			_resolver = resolver;
			reader = new BamlBinaryReader(stream);

			XamlTypeDeclaration = new TypeDeclaration(Resolver, "", "System.Windows.Markup", 0);
			KnownInfo = new KnownInfo(resolver);
		}

		public override string GetAttribute(string name)
		{
			throw new NotImplementedException();
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			throw new NotImplementedException();
		}

		public override string GetAttribute(int i)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToAttribute(string name)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			throw new NotImplementedException();
		}

		public override bool MoveToFirstAttribute()
		{
			intoAttribute = false;
			if (nodes.Count > 0 && (nodes.Peek() is XmlBamlProperty || nodes.Peek() is XmlBamlSimpleProperty))
			{
				_currentNode = nodes.Dequeue();
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			intoAttribute = false;
			if (nodes.Count > 0 &&  (nodes.Peek() is XmlBamlProperty || nodes.Peek() is XmlBamlSimpleProperty))
			{
				_currentNode = nodes.Dequeue();
				return true;
			}
			return false;
		}

		public override bool MoveToElement()
		{
			while (nodes.Peek() is XmlBamlProperty || nodes.Peek() is XmlBamlSimpleProperty)
			{
				nodes.Dequeue();
			}

			return true;
		}

		public override bool ReadAttributeValue()
		{
			if (!intoAttribute)
			{
				intoAttribute = true;
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			return ReadInternal();
		}

		bool ReadInternal()
		{
			EnsureInit();

			if (SetNextNode())
				return true;

			try
			{
				do
				{
					ReadRecordType();
					if (currentType == BamlRecordType.DocumentEnd)
						break;

					long position = reader.BaseStream.Position;

					bytesToSkip = ComputeBytesToSkip();
					ProcessNext();

					if (bytesToSkip > 0)
						// jump to the end of the record
						reader.BaseStream.Position = position + bytesToSkip;
				}
				//while (currentType != BamlRecordType.DocumentEnd);
				while (nodes.Count == 0 || (currentType != BamlRecordType.ElementEnd) || complexPropertyOpened > 0);

				if (!SetNextNode()) {
					_eof = true;
					return false;
				}
				return true;
			}
			catch (EndOfStreamException)
			{
				_eof = true;
				return false;
			}
		}

		void ReadRecordType()
		{
			byte type = reader.ReadByte();
			if (type < 0)
				currentType = BamlRecordType.DocumentEnd;
			else
				currentType = (BamlRecordType)type;
#if DEBUG
			if (currentType.ToString().EndsWith("End", StringComparison.Ordinal))
				Debug.Unindent();
			Debug.WriteLine(string.Format("{0} (0x{0:x})", currentType));
			if (currentType.ToString().EndsWith("Start", StringComparison.Ordinal))
				Debug.Indent();
#endif
		}

		bool SetNextNode()
		{
			while (nodes.Count > 0)
			{
				_currentNode = nodes.Dequeue();

				if ((_currentNode is XmlBamlProperty)) continue;
				if ((_currentNode is XmlBamlSimpleProperty)) continue;

				if (NodeType == XmlNodeType.EndElement)
				{
					if (readingElements.Count == 1)
						_rootNamespaces = ((IXmlNamespaceResolver)this).GetNamespacesInScope(XmlNamespaceScope.All);
					readingElements.Pop();
				}
				else if (NodeType == XmlNodeType.Element)
					readingElements.Push((XmlBamlElement)_currentNode);

				return true;
			}

			return false;
		}

		void ProcessNext()
		{
			switch (currentType)
			{
				case BamlRecordType.DocumentStart:
					reader.ReadBytes(6);
					break;
				case BamlRecordType.DocumentEnd:
					break;
				case BamlRecordType.ElementStart:
					ReadElementStart();
					break;
				case BamlRecordType.ElementEnd:
					ReadElementEnd();
					break;
				case BamlRecordType.AssemblyInfo:
					ReadAssemblyInfo();
					break;
				case BamlRecordType.StringInfo:
					ReadStringInfo();
					break;
				case BamlRecordType.LineNumberAndPosition:
					reader.ReadInt32();
					reader.ReadInt32();
					break;
				case BamlRecordType.LinePosition:
					reader.ReadInt32();
					break;
				case BamlRecordType.XmlnsProperty:
					ReadXmlnsProperty();
					break;
				case BamlRecordType.ConnectionId:
					ReadConnectionId();
					break;
				case BamlRecordType.DeferableContentStart:
					Current.IsDeferred = true;
					keys = new List<KeyMapping>();
					currentKey = 0;
					reader.ReadInt32();
					break;
				case BamlRecordType.DefAttribute:
					ReadDefAttribute();
					break;
				case BamlRecordType.DefAttributeKeyType:
					ReadDefAttributeKeyType();
					break;
				case BamlRecordType.DefAttributeKeyString:
					ReadDefAttributeKeyString();
					break;
				case BamlRecordType.AttributeInfo:
					ReadAttributeInfo();
					break;
				case BamlRecordType.PropertyListStart:
					ReadPropertyListStart();
					break;
				case BamlRecordType.PropertyListEnd:
					ReadPropertyListEnd();
					break;
				case BamlRecordType.Property:
					ReadProperty();
					break;
				case BamlRecordType.PropertyWithConverter:
					ReadPropertyWithConverter();
					break;
				case BamlRecordType.PropertyWithExtension:
					ReadPropertyWithExtension();
					break;
				case BamlRecordType.PropertyDictionaryStart:
					ReadPropertyDictionaryStart();
					break;
				case BamlRecordType.PropertyCustom:
					ReadPropertyCustom();
					break;
				case BamlRecordType.PropertyDictionaryEnd:
					ReadPropertyDictionaryEnd();
					break;
				case BamlRecordType.PropertyComplexStart:
					ReadPropertyComplexStart();
					break;
				case BamlRecordType.PropertyComplexEnd:
					ReadPropertyComplexEnd();
					break;
				case BamlRecordType.PIMapping:
					ReadPIMapping();
					break;
				case BamlRecordType.TypeInfo:
					ReadTypeInfo();
					break;
				case BamlRecordType.ContentProperty:
					ReadContentProperty();
					break;
				case BamlRecordType.ConstructorParametersStart:
					ReadConstructorParametersStart();
					break;
				case BamlRecordType.ConstructorParametersEnd:
					ReadConstructorParametersEnd();
					break;
				case BamlRecordType.ConstructorParameterType:
					ReadConstructorParameterType();
					break;
				case BamlRecordType.Text:
					ReadText();
					break;
				case BamlRecordType.TextWithConverter:
					ReadTextWithConverter();
					break;
				case BamlRecordType.TextWithId:
					ReadTextWithId();
					break;
				case BamlRecordType.PropertyWithStaticResourceId:
					ReadPropertyWithStaticResourceIdentifier();
					break;
				case BamlRecordType.OptimizedStaticResource:
					ReadOptimizedStaticResource();
					break;
				case BamlRecordType.KeyElementStart:
					ReadKeyElementStart();
					break;
				case BamlRecordType.KeyElementEnd:
					ReadKeyElementEnd();
					break;
				case BamlRecordType.PropertyTypeReference:
					ReadPropertyTypeReference();
					break;
				case BamlRecordType.StaticResourceStart:
					ReadStaticResourceStart();
					break;
				case BamlRecordType.StaticResourceEnd:
					ReadStaticResourceEnd();
					break;
				case BamlRecordType.StaticResourceId:
					ReadStaticResourceId();
					break;
				case BamlRecordType.PresentationOptionsAttribute:
					ReadPresentationOptionsAttribute();
					break;
				case BamlRecordType.TypeSerializerInfo:
					ReadTypeInfo();
					break;
				case BamlRecordType.PropertyArrayStart:
					ReadPropertyArrayStart();
					break;
				case BamlRecordType.PropertyArrayEnd:
					ReadPropertyArrayEnd();
					break;
				default:
					throw new NotImplementedException("UnsupportedNode: " + currentType);
			}
		}

		void ReadConnectionId()
		{
			int id = reader.ReadInt32();
			nodes.Enqueue(new XmlBamlSimpleProperty(XWPFNamespace, "ConnectionId", id.ToString()));
		}
		
		void ReadTextWithId()
		{
			short textId = reader.ReadInt16();
			string text = stringTable[textId];
			nodes.Enqueue(new XmlBamlText(text));
		}

		int ComputeBytesToSkip()
		{
			switch (currentType)
			{
				case BamlRecordType.PropertyWithConverter:
				case BamlRecordType.DefAttributeKeyString:
				case BamlRecordType.PresentationOptionsAttribute:
				case BamlRecordType.Property:
				case BamlRecordType.PropertyCustom:
				case BamlRecordType.Text:
				case BamlRecordType.TextWithId:
				case BamlRecordType.TextWithConverter:
				case BamlRecordType.XmlnsProperty:
				case BamlRecordType.DefAttribute:
				case BamlRecordType.PIMapping:
				case BamlRecordType.AssemblyInfo:
				case BamlRecordType.TypeInfo:
				case BamlRecordType.AttributeInfo:
				case BamlRecordType.StringInfo:
				case BamlRecordType.TypeSerializerInfo:
					return reader.ReadCompressedInt32();
				default:
					return 0;
			}
		}

		void EnsureInit()
		{
			if (!initialized)
			{
				int startChars = reader.ReadInt32();
				String type = new String(new BinaryReader(reader.BaseStream, Encoding.Unicode).ReadChars(startChars >> 1));
				if (type != "MSBAML")
					throw new NotSupportedException("Not a MS BAML");

				int r = reader.ReadInt32();
				int s = reader.ReadInt32();
				int t = reader.ReadInt32();
				if (((r != 0x600000) || (s != 0x600000)) || (t != 0x600000))
					throw new NotSupportedException();

				initialized = true;
			}
		}


		public override void Close()
		{
			reader = null;
		}


		public override string LookupNamespace(string prefix)
		{
			if (readingElements.Count == 0) return null;

			var namespaces = readingElements.Peek().Namespaces;

			for (int i = 0; i < namespaces.Count; i++) {
				if (String.CompareOrdinal(namespaces[i].Prefix, prefix) == 0)
					return namespaces[i].Namespace;
			}

			return null;
		}


		public override void ResolveEntity()
		{
			throw new NotImplementedException();
		}


		public override XmlNodeType NodeType
		{
			get {
				if (intoAttribute) return XmlNodeType.Text;

				return CurrentNode.NodeType;
			}
		}


		public override string LocalName
		{
			get
			{
				if (intoAttribute) return string.Empty;

				String localName = string.Empty;

				XmlBamlNode node = CurrentNode;
				if (node is XmlBamlSimpleProperty) {
					var simpleNode = (XmlBamlSimpleProperty)node;
					localName = simpleNode.LocalName;
				} else if (node is XmlBamlProperty)
				{
					PropertyDeclaration pd = ((XmlBamlProperty)node).PropertyDeclaration;
					localName = FormatPropertyDeclaration(pd, false, true, true);
				}
				else if (node is XmlBamlPropertyElement)
				{
					XmlBamlPropertyElement property = (XmlBamlPropertyElement)node;
					string typeName = property.TypeDeclaration.Name;
					
					if (property.Parent.TypeDeclaration.Type.IsSubclassOf(property.PropertyDeclaration.DeclaringType.Type))
						typeName = property.Parent.TypeDeclaration.Name;
					
					localName = String.Format("{0}.{1}", typeName, property.PropertyDeclaration.Name);
				}
				else if (node is XmlBamlElement)
					localName = ((XmlBamlElement)node).TypeDeclaration.Name;

				localName = NameTable.Add(localName);

				return localName;
			}
		}

		PropertyDeclaration GetPropertyDeclaration(short identifier)
		{
			PropertyDeclaration declaration;
			if (identifier >= 0)
			{
				declaration = propertyTable[identifier];
			}
			else
			{
				declaration = KnownInfo.KnownPropertyTable[-identifier];
			}
			if (declaration == null)
			{
				throw new NotSupportedException();
			}
			return declaration;
		}

		object GetResourceName(short identifier)
		{
			if (identifier >= 0) {
				PropertyDeclaration declaration = propertyTable[identifier];
				return declaration;
			} else {
				identifier = (short)-identifier;
				bool isKey = true;
				if (identifier > 0xe8 && identifier < 0x1d0) {
					isKey = false;
					identifier -= 0xe8;
				} else if (identifier > 0x1d0 && identifier < 0x1d3) {
					identifier -= 0xe7;
				} else if (identifier > 0x1d3 && identifier < 0x1d6) {
					identifier -= 0xea;
					isKey = false;
				}
				ResourceName resource;
				if (!KnownInfo.KnownResourceTable.TryGetValue(identifier, out resource))
//					resource = new ResourceName("???Resource" + identifier + "???");
					throw new ArgumentException("Cannot find resource name " + identifier);
				if (isKey)
					return new ResourceName(resource.Name + "Key");
				return resource;
			}
		}

		void ReadPropertyArrayStart()
		{
			short identifier = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			XmlBamlElement element = elements.Peek();
			XmlBamlPropertyElement property = new XmlBamlPropertyElement(element, PropertyType.Array, pd);
			elements.Push(property);
			nodes.Enqueue(property);
		}

		void ReadPropertyArrayEnd()
		{
			CloseElement();
		}

		void ReadPropertyDictionaryStart()
		{
			short identifier = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			XmlBamlElement element = elements.Peek();
			XmlBamlPropertyElement property = new XmlBamlPropertyElement(element, PropertyType.Dictionary, pd);
			elements.Push(property);
			nodes.Enqueue(property);
		}

		void ReadPropertyDictionaryEnd()
		{
			CloseElement();
		}

		void ReadPropertyCustom()
		{
			short identifier = reader.ReadInt16();
			short serializerTypeId = reader.ReadInt16();
			bool isValueTypeId = (serializerTypeId & 0x4000) == 0x4000;
			if (isValueTypeId)
				serializerTypeId = (short)(serializerTypeId & ~0x4000);

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			string value;
			switch (serializerTypeId)
			{
				case 0x2e8:
					value = new BrushConverter().ConvertToString(SolidColorBrush.DeserializeFrom(reader));
					break;
				case 0x2e9:
					value = new Int32CollectionConverter().ConvertToString(DeserializeInt32CollectionFrom(reader));
					break;
				case 0x89:

					short typeIdentifier = reader.ReadInt16();
					if (isValueTypeId)
					{
						TypeDeclaration typeDeclaration = GetTypeDeclaration(typeIdentifier);
						string name = reader.ReadString();
						value = FormatPropertyDeclaration(new PropertyDeclaration(name, typeDeclaration), true, false, true);
					}
					else
						value = FormatPropertyDeclaration(GetPropertyDeclaration(typeIdentifier), true, false, true);
					break;

				case 0x2ea:
					value = ((IFormattable)staticConvertCustomBinaryToObjectMethod.Invoke(null, new object[] { reader })).ToString("G", CultureInfo.InvariantCulture);
					break;
				case 0x2eb:
				case 0x2f0:
					value = Deserialize3DPoints();
					break;
				case 0x2ec:
					value = DeserializePoints();
					break;
				case 0xc3:
					// Enum
					uint num = reader.ReadUInt32();
					value = num.ToString();
					break;
				case 0x2e:
					int b = reader.ReadByte();
					value = (b == 1) ? Boolean.TrueString : Boolean.FalseString;
					break;
				default:
					return;
			}

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Value, pd);
			property.Value = value;

			nodes.Enqueue(property);
		}

		string DeserializePoints()
		{
			using (StringWriter writer = new StringWriter())
			{
				int num10 = reader.ReadInt32();
				for (int k = 0; k < num10; k++)
				{
					if (k != 0)
						writer.Write(" ");
					for (int m = 0; m < 2; m++)
					{
						if (m != 0)
							writer.Write(",");
						writer.Write(reader.ReadCompressedDouble().ToString());
					}
				}
				return writer.ToString();
			}
		}

		string Deserialize3DPoints()
		{
			using (StringWriter writer = new StringWriter())
			{
				int num14 = reader.ReadInt32();
				for (int i = 0; i < num14; i++)
				{
					if (i != 0)
					{
						writer.Write(" ");
					}
					for (int j = 0; j < 3; j++)
					{
						if (j != 0)
						{
							writer.Write(",");
						}
						writer.Write(reader.ReadCompressedDouble().ToString());
					}
				}
				return writer.ToString();
			}
		}

		static Int32Collection DeserializeInt32CollectionFrom(BinaryReader reader)
		{
			IntegerCollectionType type = (IntegerCollectionType)reader.ReadByte();
			int capacity = reader.ReadInt32();
			if (capacity < 0)
				throw new ArgumentException();

			Int32Collection ints = new Int32Collection(capacity);
			switch (type) {
				case IntegerCollectionType.Byte:
					for (int i = 0; i < capacity; i++)
						ints.Add(reader.ReadByte());
					return ints;
				case IntegerCollectionType.UShort:
					for (int j = 0; j < capacity; j++)
						ints.Add(reader.ReadUInt16());
					return ints;
				case IntegerCollectionType.Integer:
					for (int k = 0; k < capacity; k++)
						ints.Add(reader.ReadInt32());
					return ints;
				case IntegerCollectionType.Consecutive:
					int start = reader.ReadInt32();
					for (int m = start; m < capacity + start; m++)
						ints.Add(m);
					return ints;
			}
			throw new ArgumentException();
		}

		void ReadPropertyWithExtension()
		{
			short identifier = reader.ReadInt16();
			short x = reader.ReadInt16();
			short valueIdentifier = reader.ReadInt16();
			bool isValueType = (x & 0x4000) == 0x4000;
			bool isStaticType = (x & 0x2000) == 0x2000;
			x = (short)(x & 0xfff);

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			short extensionIdentifier = (short)-(x & 0xfff);
			string value = String.Empty;

			switch (x) {
				case 0x25a:
					// StaticExtension
					value = GetStaticExtension(GetResourceName(valueIdentifier));
					break;
				case 0x25b: // StaticResource
				case 0xbd: // DynamicResource
					if (isValueType)
					{
						value = GetTypeExtension(valueIdentifier);
					}
					else if (isStaticType)
					{
						TypeDeclaration extensionDeclaration = GetTypeDeclaration(extensionIdentifier);
						value = GetExtension(extensionDeclaration, GetStaticExtension(GetResourceName(valueIdentifier)));
					}
					else
					{
						TypeDeclaration extensionDeclaration = GetTypeDeclaration(extensionIdentifier);
						value = GetExtension(extensionDeclaration, (string)stringTable[valueIdentifier]);
					}
					break;

				case 0x27a:
					// TemplateBinding
					PropertyDeclaration pdValue = GetPropertyDeclaration(valueIdentifier);
					value = GetTemplateBindingExtension(pdValue);
					break;
				default:
					throw new NotSupportedException("Unknown property with extension");
			}

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Value, pd);
			property.Value = value;

			nodes.Enqueue(property);
		}

		void ReadProperty()
		{
			short identifier = reader.ReadInt16();
			string text = reader.ReadString();

			EnqueueProperty(identifier, EscapeCurlyBraces(text));
		}

		void ReadPropertyWithConverter()
		{
			short identifier = reader.ReadInt16();
			string text = reader.ReadString();
			reader.ReadInt16();

			EnqueueProperty(identifier, EscapeCurlyBraces(text));
		}

		string EscapeCurlyBraces(string text)
		{
			if (!text.StartsWith("{", StringComparison.OrdinalIgnoreCase))
				return text;
			if (text.StartsWith("{}", StringComparison.OrdinalIgnoreCase))
				return text;
			return "{}" + text;
		}
		
		bool HaveSeenNestedElement()
		{
			XmlBamlElement element = elements.Peek();
			int elementIndex = nodes.IndexOf(element);
			for (int i = elementIndex + 1; i < nodes.Count; i++)
			{
				if (nodes[i] is XmlBamlEndElement)
					return true;
			}
			return false;
		}
		
		void EnqueueProperty(short identifier, string text)
		{
			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			XmlBamlElement element = FindXmlBamlElement();
			// if we've already read a nested element for the current element, this property must be a nested element as well
			if (HaveSeenNestedElement())
			{
				XmlBamlPropertyElement property = new XmlBamlPropertyElement(element, PropertyType.Complex, pd);
				
				nodes.Enqueue(property);
				nodes.Enqueue(new XmlBamlText(text));
				nodes.Enqueue(new XmlBamlEndElement(property));
			}
			else
			{
				XmlBamlProperty property = new XmlBamlProperty(element, PropertyType.Value, pd);
				property.Value = text;
				
				nodes.Enqueue(property);
			}
		}

		void ReadAttributeInfo()
		{
			short key = reader.ReadInt16();
			short identifier = reader.ReadInt16();
			reader.ReadByte();
			string name = reader.ReadString();
			TypeDeclaration declaringType = GetTypeDeclaration(identifier);
			PropertyDeclaration property = new PropertyDeclaration(name, declaringType);
			propertyTable.Add(key, property);
		}

		void ReadDefAttributeKeyType()
		{
			short typeIdentifier = reader.ReadInt16();
			reader.ReadByte();
			int position = reader.ReadInt32();
			bool shared = reader.ReadBoolean();
			bool sharedSet = reader.ReadBoolean();
			
			string extension = GetTypeExtension(typeIdentifier);
			
			keys.Add(new KeyMapping(extension) { Shared = shared, SharedSet = sharedSet, Position = position });
		}

		void ReadDefAttribute()
		{
			string text = reader.ReadString();
			short identifier = reader.ReadInt16();

			PropertyDeclaration pd;
			switch (identifier)
			{
				case -2:
					pd = new PropertyDeclaration("Uid", XamlTypeDeclaration);
					break;
				case -1:
					pd = new PropertyDeclaration("Name", XamlTypeDeclaration);
					break;
				default:
					string recordName = stringTable[identifier];
					if (recordName != "Key") throw new NotSupportedException(recordName);
					pd = new PropertyDeclaration(recordName, XamlTypeDeclaration);
					if (keys == null)
						keys = new List<KeyMapping>();
					keys.Add(new KeyMapping(text) { Position = -1 });
					break;
			}

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Key, pd);
			property.Value = text;

			nodes.Enqueue(property);
		}

		void ReadDefAttributeKeyString()
		{
			short stringId = reader.ReadInt16();
			int position = reader.ReadInt32();
			bool shared = reader.ReadBoolean();
			bool sharedSet = reader.ReadBoolean();
			
			string text = stringTable[stringId];
			Debug.Print("KeyString: " + text);
			if (text == null)
				throw new NotSupportedException();

			keys.Add(new KeyMapping(text) { Position = position });
		}
		
		void ReadXmlnsProperty()
		{
			string prefix = reader.ReadString();
			string @namespace = reader.ReadString();
			string[] textArray = new string[(uint)reader.ReadInt16()];
			for (int i = 0; i < textArray.Length; i++) {
				textArray[i] = assemblyTable[reader.ReadInt16()];
			}

			var namespaces = elements.Peek().Namespaces;

			// The XmlnsProperty record corresponds to an assembly
			// We need to add the assembly name to the entry.
			if (@namespace.StartsWith("clr-namespace:", StringComparison.Ordinal) && @namespace.IndexOf("assembly=", StringComparison.Ordinal) < 0) {
				XmlToClrNamespaceMapping mappingToChange = null;
				foreach (XmlToClrNamespaceMapping mapping in mappings) {
					if (String.CompareOrdinal(mapping.XmlNamespace, @namespace) == 0) {
						mappingToChange = mapping;
						break;
					}
				}
				if (mappingToChange == null)
					throw new InvalidOperationException("Cannot find mapping");
				if (mappingToChange.AssemblyId > 0) {
					@namespace = String.Format("{0};assembly={1}", @namespace, mappingToChange.AssemblyName.Replace(" ", ""));
					mappingToChange.XmlNamespace = @namespace;
				}
			}
			var ns = new XmlNamespace(prefix, @namespace);
			XmlnsDefinitions.Add(ns);
			namespaces.Add(ns);
		}

		void ReadElementEnd()
		{
			CloseElement();
			if (Current.IsDeferred)
				keys = null;
			LayerPop();
		}

		void ReadPropertyComplexStart()
		{
			short identifier = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			XmlBamlElement element = FindXmlBamlElement();

			XmlBamlPropertyElement property = new XmlBamlPropertyElement(element, PropertyType.Complex, pd);
			elements.Push(property);
			nodes.Enqueue(property);
			complexPropertyOpened++;
		}

		XmlBamlElement FindXmlBamlElement()
		{
			return elements.Peek();

			//XmlBamlElement element;
			//int x = nodes.Count - 1;
			//do
			//{
			//    element = nodes[x] as XmlBamlElement;
			//    x--;
			//} while (element == null);
			//return element;
		}

		void ReadPropertyListStart()
		{
			short identifier = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			XmlBamlElement element = FindXmlBamlElement();
			XmlBamlPropertyElement property = new XmlBamlPropertyElement(element, PropertyType.List, pd);
			elements.Push(property);
			nodes.Enqueue(property);
		}

		void ReadPropertyListEnd()
		{
			CloseElement();
		}

		void ReadPropertyComplexEnd()
		{
			XmlBamlPropertyElement propertyElement = (XmlBamlPropertyElement) elements.Peek();

			CloseElement();

			complexPropertyOpened--;
			// this property could be a markup extension
			// try to convert it
			int elementIndex = nodes.IndexOf(propertyElement.Parent);
			int start = nodes.IndexOf(propertyElement) + 1;
			IEnumerator<XmlBamlNode> enumerator = nodes.GetEnumerator();
			
			// move enumerator to the start of this property value
			// note whether there are any child elements before this one
			bool anyChildElement = false;
			for (int i = 0; i < start && enumerator.MoveNext(); i++)
			{
				if (i > elementIndex && i < start - 1 && (enumerator.Current is XmlBamlEndElement))
					anyChildElement = true;
			}

			if (!anyChildElement && IsExtension(enumerator) && start < nodes.Count - 1) {
				start--;
				nodes.RemoveAt(start);
				nodes.RemoveLast();

				StringBuilder sb = new StringBuilder();
				FormatElementExtension((XmlBamlElement) nodes[start], sb);

				XmlBamlProperty property =
					new XmlBamlProperty(elements.Peek(), PropertyType.Complex, propertyElement.PropertyDeclaration);
				property.Value = sb.ToString();
				nodes.Add(property);
			}
		}

		void FormatElementExtension(XmlBamlElement element, StringBuilder sb)
		{
			sb.Append("{");
			sb.Append(FormatTypeDeclaration(element.TypeDeclaration));

			int start = nodes.IndexOf(element);
			nodes.RemoveAt(start);

			string sep = " ";
			while (nodes.Count > start)
			{
				XmlBamlNode node = nodes[start];

				if (node is XmlBamlEndElement)
				{
					sb.Append("}");
					nodes.RemoveAt(start);
					break;
				}
				else if (node is XmlBamlPropertyElement)
				{
					nodes.RemoveAt(start);

					sb.Append(sep);
					XmlBamlPropertyElement property = (XmlBamlPropertyElement)node;
					sb.Append(property.PropertyDeclaration.Name);
					sb.Append("=");

					node = nodes[start];
					nodes.RemoveLast();
					FormatElementExtension((XmlBamlElement)node, sb);
				}
				else if (node is XmlBamlElement)
				{
					sb.Append(sep);
					FormatElementExtension((XmlBamlElement)node, sb);
				}
				else if (node is XmlBamlProperty)
				{
					nodes.RemoveAt(start);

					sb.Append(sep);
					XmlBamlProperty property = (XmlBamlProperty)node;
					sb.Append(property.PropertyDeclaration.Name);
					sb.Append("=");
					sb.Append(property.Value);
				}
				else if (node is XmlBamlText)
				{
					nodes.RemoveAt(start);

					sb.Append(sep);
					sb.Append(((XmlBamlText)node).Text);
				}
				sep = ", ";
			}
		}

		bool IsExtension(IEnumerator<XmlBamlNode> enumerator)
		{
			while (enumerator.MoveNext()) {
				var node = enumerator.Current;
				if (node.NodeType == XmlNodeType.Element && !((XmlBamlElement)node).TypeDeclaration.IsExtension)
					return false;
			}

			return true;
		}

		void CloseElement()
		{
			var e = elements.Pop();
			if (!e.IsImplicit)
				nodes.Enqueue(new XmlBamlEndElement(e));
		}

		void ReadElementStart()
		{
			LayerPush();
			short identifier = reader.ReadInt16();
			sbyte flags = reader.ReadSByte();
			if (flags < 0 || flags > 3)
				throw new NotImplementedException();
			Debug.Print("ElementFlags: " + flags);
			
			TypeDeclaration declaration = GetTypeDeclaration(identifier);

			XmlBamlElement element;
			XmlBamlElement parentElement = null;
			if (elements.Count > 0)
			{
				parentElement = elements.Peek();
				element = new XmlBamlElement(parentElement);
				element.Position = reader.BaseStream.Position;

				// Porto l'inizio del padre all'inizio del primo figlio
				if (parentElement.Position == 0 && complexPropertyOpened == 0)
					parentElement.Position = element.Position;
			}
			else
				element = new XmlBamlElement();
			
			// the type is defined in the local assembly, i.e., the main assembly
			// and this is the root element
			TypeDeclaration oldDeclaration = null;
			if (_resolver.IsLocalAssembly(declaration.Assembly) && parentElement == null) {
				oldDeclaration = declaration;
				declaration = GetKnownTypeDeclarationByName(declaration.Type.BaseType.AssemblyQualifiedName);
			}
			element.TypeDeclaration = declaration;
			element.IsImplicit = (flags & 2) == 2;
			elements.Push(element);
			if (!element.IsImplicit)
				nodes.Enqueue(element);
			
			if (oldDeclaration != null) {
				nodes.Enqueue(new XmlBamlSimpleProperty(XWPFNamespace, "Class", oldDeclaration.FullyQualifiedName.Replace('+', '.')));
			}

			if (parentElement != null && complexPropertyOpened == 0 && !Current.IsInStaticResource && Current.Previous.IsDeferred) {
				if (keys != null && keys.Count > currentKey) {
					string key = keys[currentKey].KeyString;
					AddKeyToElement(key);
					currentKey++;
				}
			}
		}

		void AddKeyToElement(string key)
		{
			PropertyDeclaration pd = new PropertyDeclaration("Key", XamlTypeDeclaration);
			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Key, pd);

			property.Value = key;

			nodes.Enqueue(property);
		}

		XmlToClrNamespaceMapping FindByClrNamespaceAndAssemblyId(TypeDeclaration declaration)
		{
			return FindByClrNamespaceAndAssemblyName(declaration.Namespace, declaration.Assembly);
		}
		
		XmlToClrNamespaceMapping FindByClrNamespaceAndAssemblyName(string clrNamespace, string assemblyName)
		{
			if (assemblyName == XamlTypeDeclaration.Assembly) {
				if (clrNamespace == XamlTypeDeclaration.Namespace)
					return new XmlToClrNamespaceMapping(XmlToClrNamespaceMapping.XamlNamespace, -1, XamlTypeDeclaration.Assembly, XamlTypeDeclaration.Namespace);
				return new XmlToClrNamespaceMapping(XmlToClrNamespaceMapping.PresentationNamespace, -1, assemblyName, clrNamespace);
			}

			for (int x = 0; x < mappings.Count; x++) {
				XmlToClrNamespaceMapping xp = mappings[x];
				if (string.Equals(xp.AssemblyName, assemblyName, StringComparison.Ordinal) && string.Equals(xp.ClrNamespace, clrNamespace, StringComparison.Ordinal))
					return xp;
			}

			return new XmlToClrNamespaceMapping(XmlToClrNamespaceMapping.PresentationNamespace, -1, assemblyName, clrNamespace);
		}

		void ReadPIMapping()
		{
			string xmlNamespace = reader.ReadString();
			string clrNamespace = reader.ReadString();
			short assemblyId = reader.ReadInt16();

			mappings.Add(new XmlToClrNamespaceMapping(xmlNamespace, assemblyId, GetAssembly(assemblyId), clrNamespace));
		}

		void ReadContentProperty()
		{
			reader.ReadInt16();

			// Non serve aprire niente, ?il default
		}

		static void ReadConstructorParametersStart()
		{
			//this.constructorParameterTable.Add(this.elements.Peek());
			//PromoteDataToComplexProperty();
		}

		static void ReadConstructorParametersEnd()
		{
			//this.constructorParameterTable.Remove(this.elements.Peek());
			//properties.Pop();
		}

		void ReadConstructorParameterType()
		{
			short identifier = reader.ReadInt16();

			//TypeDeclaration declaration = GetTypeDeclaration(identifier);
			nodes.Enqueue(new XmlBamlText(GetTypeExtension(identifier)));
		}

		void ReadText()
		{
			string text = reader.ReadString();

			nodes.Enqueue(new XmlBamlText(text));
		}

		void ReadKeyElementStart()
		{
			short typeIdentifier = reader.ReadInt16();
			byte valueIdentifier = reader.ReadByte();
			// TODO: handle shared
			//bool shared = (valueIdentifier & 1) != 0;
			//bool sharedSet = (valueIdentifier & 2) != 0;
			int position = reader.ReadInt32();
			reader.ReadBoolean();
			reader.ReadBoolean();

			TypeDeclaration declaration = GetTypeDeclaration(typeIdentifier);

			XmlBamlPropertyElement property = new XmlBamlPropertyElement(elements.Peek(), PropertyType.Key, new PropertyDeclaration("Key", declaration));
			property.Position = position;
			elements.Push(property);
			nodes.Enqueue(property);
			complexPropertyOpened++;
		}

		void ReadKeyElementEnd()
		{
			XmlBamlPropertyElement propertyElement = (XmlBamlPropertyElement)elements.Peek();

			CloseElement();
			complexPropertyOpened--;
			if (complexPropertyOpened == 0) {
				int start = nodes.IndexOf(propertyElement);

				StringBuilder sb = new StringBuilder();
				FormatElementExtension((XmlBamlElement)nodes[start], sb);
				keys.Add(new KeyMapping(sb.ToString()) { Position = -1 });
			}
		}

		void ReadStaticResourceStart()
		{
			Current.IsInStaticResource = true;
			short identifier = reader.ReadInt16();
			byte flags = reader.ReadByte();
			TypeDeclaration declaration = GetTypeDeclaration(identifier);
			var lastKey = keys.LastOrDefault();
			if (lastKey == null)
				throw new InvalidOperationException("No key mapping found for StaticResourceStart!");
			lastKey.StaticResources.Add(declaration);
			XmlBamlElement element;
			if (elements.Any())
				element = new XmlBamlElement(elements.Peek());
			else
				element = new XmlBamlElement();
			element.TypeDeclaration = declaration;
			elements.Push(element);
			nodes.Enqueue(element);
		}

		void ReadStaticResourceEnd()
		{
			CloseElement();
			Current.IsInStaticResource = false;
		}

		void ReadStaticResourceId()
		{
			short identifier = reader.ReadInt16();
			object staticResource = GetStaticResource(identifier);
		}

		void ReadPresentationOptionsAttribute()
		{
			string text = reader.ReadString();
			short valueIdentifier = reader.ReadInt16();

			PropertyDeclaration pd = new PropertyDeclaration(stringTable[valueIdentifier].ToString());

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Value, pd);
			property.Value = text;
		}

		void ReadPropertyTypeReference()
		{
			short identifier = reader.ReadInt16();
			short typeIdentifier = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(identifier);
			string value = GetTypeExtension(typeIdentifier);

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Value, pd);
			property.Value = value;

			nodes.Enqueue(property);
		}

		void ReadOptimizedStaticResource()
		{
			byte flags = reader.ReadByte();
			short typeIdentifier = reader.ReadInt16();
			bool isValueType = (flags & 1) == 1;
			bool isStaticType = (flags & 2) == 2;
			object resource;

			if (isValueType)
				resource = GetTypeExtension(typeIdentifier);
			else if (isStaticType) {
				resource = GetStaticExtension(GetResourceName(typeIdentifier));
			} else {
				resource = stringTable[typeIdentifier];
			}
			
			var lastKey = keys.LastOrDefault();
			if (lastKey == null)
				throw new InvalidOperationException("No key mapping found for OptimizedStaticResource!");
			lastKey.StaticResources.Add(resource);
		}

		string GetTemplateBindingExtension(PropertyDeclaration propertyDeclaration)
		{
			return String.Format("{{TemplateBinding {0}}}", FormatPropertyDeclaration(propertyDeclaration, true, false, false));
		}

		string GetStaticExtension(object resource)
		{
			if (resource == null)
				return null;
			string name;
			if (resource is ResourceName)
				name = ((ResourceName)resource).Name;
			else if (resource is PropertyDeclaration)
				name = FormatPropertyDeclaration(((PropertyDeclaration)resource), true, false, false);
			else
				throw new InvalidOperationException("Invalid resource: " + resource.GetType());

			string prefix = LookupPrefix(XmlToClrNamespaceMapping.XamlNamespace, false);
			if (String.IsNullOrEmpty(prefix))
				return String.Format("{{Static {0}}}", name);
			else
				return String.Format("{{{0}:Static {1}}}", prefix, name);
		}

		string GetExtension(TypeDeclaration declaration, string value)
		{
			return String.Format("{{{0} {1}}}", FormatTypeDeclaration(declaration), value);
		}

		string GetTypeExtension(short typeIdentifier)
		{
			string prefix = LookupPrefix(XmlToClrNamespaceMapping.XamlNamespace, false);
			if (String.IsNullOrEmpty(prefix))
				return String.Format("{{Type {0}}}", FormatTypeDeclaration(GetTypeDeclaration(typeIdentifier)));
			else
				return String.Format("{{{0}:Type {1}}}", prefix, FormatTypeDeclaration(GetTypeDeclaration(typeIdentifier)));
		}

		string FormatTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			XmlToClrNamespaceMapping mapping = FindByClrNamespaceAndAssemblyName(typeDeclaration.Namespace, typeDeclaration.Assembly);
			string prefix = (mapping != null) ? LookupPrefix(mapping.XmlNamespace, false) : null;
			string name = typeDeclaration.Name;
			if (name.EndsWith("Extension", StringComparison.Ordinal))
				name = name.Substring(0, name.Length - 9);
			if (String.IsNullOrEmpty(prefix))
				return name;
			else
				return String.Format("{0}:{1}", prefix, name);
		}

		string FormatPropertyDeclaration(PropertyDeclaration propertyDeclaration, bool withPrefix, bool useReading, bool checkType)
		{
			StringBuilder sb = new StringBuilder();

			TypeDeclaration elementDeclaration = (useReading) ? readingElements.Peek().TypeDeclaration : elements.Peek().TypeDeclaration;

			IDependencyPropertyDescriptor descriptor = null;
			bool areValidTypes = elementDeclaration.Type != null && propertyDeclaration.DeclaringType.Type != null;
			if (areValidTypes)
				descriptor = Resolver.GetDependencyPropertyDescriptor(propertyDeclaration.Name, elementDeclaration.Type, propertyDeclaration.DeclaringType.Type);

			bool isDescendant = (areValidTypes && (propertyDeclaration.DeclaringType.Type.Equals(elementDeclaration.Type) || elementDeclaration.Type.IsSubclassOf(propertyDeclaration.DeclaringType.Type)));
			bool isAttached = (descriptor != null && descriptor.IsAttached);

			if (withPrefix) {
				XmlToClrNamespaceMapping mapping = FindByClrNamespaceAndAssemblyName(propertyDeclaration.DeclaringType.Namespace, propertyDeclaration.DeclaringType.Assembly);
				string prefix = (mapping != null) ? LookupPrefix(mapping.XmlNamespace, false) : null;

				if (!String.IsNullOrEmpty(prefix)) {
					sb.Append(prefix);
					sb.Append(":");
				}
			}
			if ((!isDescendant || isAttached || !checkType) && propertyDeclaration.DeclaringType.Name.Length > 0) {
				sb.Append(propertyDeclaration.DeclaringType.Name);
				sb.Append(".");
			}
			sb.Append(propertyDeclaration.Name);

			return sb.ToString();
		}

		void ReadPropertyWithStaticResourceIdentifier()
		{
			short propertyId = reader.ReadInt16();
			short index = reader.ReadInt16();

			PropertyDeclaration pd = GetPropertyDeclaration(propertyId);
			object staticResource = GetStaticResource(index);

			string prefix = LookupPrefix(XmlToClrNamespaceMapping.PresentationNamespace, false);
			string value = String.Format("{{{0}{1}StaticResource {2}}}", prefix, (String.IsNullOrEmpty(prefix)) ? String.Empty : ":", staticResource);

			XmlBamlProperty property = new XmlBamlProperty(elements.Peek(), PropertyType.Value, pd);
			property.Value = value;

			nodes.Enqueue(property);
		}

		object GetStaticResource(short identifier)
		{
			int keyIndex = Math.Max(0, currentKey - 1);
			while (keyIndex > keys.Count)
				keyIndex--;
			while (keyIndex >= 0 && !keys[keyIndex].HasStaticResource(identifier))
				keyIndex--;
			if (keyIndex >= 0 && identifier < keys[keyIndex].StaticResources.Count)
				return keys[keyIndex].StaticResources[(int)identifier];
//			Debug.WriteLine(string.Format("Cannot find StaticResource: {0}", identifier));
//			return "???" + identifier + "???";
			throw new ArgumentException("Cannot find StaticResource: " + identifier, nameof(identifier));
		}

		void ReadTextWithConverter()
		{
			string text = reader.ReadString();
			reader.ReadInt16();

			nodes.Enqueue(new XmlBamlText(text));
		}

		void ReadTypeInfo()
		{
			short typeId = reader.ReadInt16();
			short assemblyId = reader.ReadInt16();
			string fullName = reader.ReadString();
			assemblyId = (short)(assemblyId & 0xfff);
			TypeDeclaration declaration;
			int bracket = fullName.IndexOf('[');
			int length = bracket > -1 ? fullName.LastIndexOf('.', bracket) : fullName.LastIndexOf('.');
			if (length != -1)
			{
				string name = fullName.Substring(length + 1);
				string namespaceName = fullName.Substring(0, length);
				declaration = new TypeDeclaration(this, Resolver, name, namespaceName, assemblyId);
			}
			else
			{
				declaration = new TypeDeclaration(this, Resolver, fullName, string.Empty, assemblyId);
			}
			typeTable.Add(typeId, declaration);
		}

		void ReadAssemblyInfo()
		{
			short key = reader.ReadInt16();
			string text = reader.ReadString();
			assemblyTable.Add(key, text);
		}

		void ReadStringInfo()
		{
			short key = reader.ReadInt16();
			string text = reader.ReadString();
			stringTable.Add(key, text);
		}

		TypeDeclaration GetTypeDeclaration(short identifier)
		{
			TypeDeclaration declaration;
			if (identifier >= 0)
				declaration = typeTable[identifier];
			else
				declaration = KnownInfo.KnownTypeTable[-identifier];

			if (declaration == null)
				throw new NotSupportedException();

			return declaration;
		}
		
		TypeDeclaration GetKnownTypeDeclarationByName(string assemblyQualifiedName)
		{
			foreach (var type in KnownInfo.KnownTypeTable) {
				if (assemblyQualifiedName == type.AssemblyQualifiedName)
					return type;
			}
			return new ResolverTypeDeclaration(_resolver, assemblyQualifiedName);
		}

		internal string GetAssembly(short identifier)
		{
			return assemblyTable[identifier];
		}

		XmlBamlNode CurrentNode {
			get { return _currentNode; }
		}

		public override string NamespaceURI {
			get {
				if (intoAttribute) return String.Empty;

				TypeDeclaration declaration;
				XmlBamlNode node = CurrentNode;
				if (node is XmlBamlSimpleProperty)
					return ((XmlBamlSimpleProperty)node).NamespaceName;
				else if (node is XmlBamlProperty) {
					declaration = ((XmlBamlProperty)node).PropertyDeclaration.DeclaringType;
					TypeDeclaration elementDeclaration = readingElements.Peek().TypeDeclaration;

					XmlToClrNamespaceMapping propertyMapping = FindByClrNamespaceAndAssemblyId(declaration);
					XmlToClrNamespaceMapping elementMapping = FindByClrNamespaceAndAssemblyId(elementDeclaration);
					
					if (((XmlBamlProperty)node).PropertyDeclaration.Name == "Name" &&
					    _resolver.IsLocalAssembly(((XmlBamlProperty)node).Parent.TypeDeclaration.Assembly))
						return XWPFNamespace;
					
					if (String.CompareOrdinal(propertyMapping.XmlNamespace, elementMapping.XmlNamespace) == 0
					    || (elementDeclaration.Type != null && declaration.Type != null && elementDeclaration.Type.IsSubclassOf(declaration.Type)))
						return String.Empty;
				}
				else if (node is XmlBamlPropertyElement)
				{
					XmlBamlPropertyElement property = (XmlBamlPropertyElement)node;
					declaration = property.TypeDeclaration;
					if (property.Parent.TypeDeclaration.Type.IsSubclassOf(property.PropertyDeclaration.DeclaringType.Type))
						declaration = property.Parent.TypeDeclaration;
				}
				else if (node is XmlBamlElement)
					declaration = ((XmlBamlElement)node).TypeDeclaration;
				else
					return String.Empty;

				XmlToClrNamespaceMapping mapping = FindByClrNamespaceAndAssemblyId(declaration);

				return mapping.XmlNamespace;
			}
		}


		public override string Prefix
		{
			get {
				if (!intoAttribute)
					return ((IXmlNamespaceResolver)this).LookupPrefix(NamespaceURI) ?? String.Empty;
				return String.Empty;
			}
		}


		public override bool HasValue
		{
			get { return Value != null; }
		}

		/// <summary>
		/// Returns object used to resolve types
		/// </summary>
		public ITypeResolver Resolver
		{
			get { return _resolver; }
		}

		public override string Value
		{
			get {
				XmlBamlNode node = CurrentNode;
				if (node is XmlBamlSimpleProperty)
					return ((XmlBamlSimpleProperty)node).Value;
				else if (node is XmlBamlProperty)
					return ((XmlBamlProperty)node).Value.ToString();
				else if (node is XmlBamlText)
					return ((XmlBamlText)node).Text;
				else if (node is XmlBamlElement)
					return String.Empty;

				return String.Empty;
			}
		}

		public IDictionary<string, string> RootNamespaces => _rootNamespaces;

		public override int Depth => readingElements.Count;

		public override string BaseURI => string.Empty;

		public override bool IsEmptyElement => false;

		public override bool EOF => _eof;

		public override int AttributeCount {
			get { throw new NotImplementedException(); }
		}

		public override ReadState ReadState {
			get {
				if (!initialized)
					return ReadState.Initial;
				if (reader == null)
					return ReadState.Closed;
				if (EOF)
					return ReadState.EndOfFile;
				return ReadState.Interactive;
			}
		}

		public override XmlNameTable NameTable => _nameTable;

		#region IXmlNamespaceResolver Members

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			var namespaces = readingElements.Peek().Namespaces;
			Dictionary<String, String> list = new Dictionary<string, string>();
			foreach (XmlNamespace ns in namespaces)
			{
				list.Add(ns.Prefix, ns.Namespace);
			}

			return list;
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return LookupPrefix(namespaceName, true);
		}

		string LookupPrefix(string namespaceName, bool useReading)
		{
			Stack<XmlBamlElement> elements;
			if (useReading)
				elements = readingElements;
			else
				elements = this.elements;

			if (elements.Count == 0) return null;
			return LookupPrefix(namespaceName, elements.Peek().Namespaces);
		}

		static string LookupPrefix(string namespaceName, IList<XmlNamespace> namespaces)
		{
			for (int x = 0; x < namespaces.Count; x++)
			{
				if (String.CompareOrdinal(namespaces[x].Namespace, namespaceName) == 0)
					return namespaces[x].Prefix;
			}

			return null;
		}

		#endregion

		internal enum IntegerCollectionType : byte
		{
			Byte = 2,
			Consecutive = 1,
			Integer = 4,
			Unknown = 0,
			UShort = 3
		}
	}
}