using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ICSharpCode.Decompiler.Metadata
{
	using SRMDocument = System.Reflection.Metadata.Document;

	/// <summary>
	/// A sequence point read from a PDB file or produced by the decompiler.
	/// </summary>
	public struct SequencePoint
	{
		/// <summary>
		/// IL start offset.
		/// </summary>
		public int Offset { get; set; }

		/// <summary>
		/// IL end offset.
		/// </summary>
		/// <remarks>
		/// This does not get stored in debug information;
		/// it is used internally to create hidden sequence points
		/// for the IL fragments not covered by any sequence point.
		/// </remarks>
		public int EndOffset { get; set; }

		public int StartLine { get; set; }
		public int StartColumn { get; set; }
		public int EndLine { get; set; }
		public int EndColumn { get; set; }

		public bool IsHidden {
			get { return StartLine == 0xfeefee && StartLine == EndLine; }
		}

		public Document Document { get; set; }

		internal void SetHidden()
		{
			StartLine = EndLine = 0xfeefee;
		}
	}

	public struct Document : IEquatable<Document>
	{
		public PEFile Module { get; }
		public DocumentHandle Handle { get; }
		public bool IsNil => Handle.IsNil;

		public Document(PEFile module, DocumentHandle handle) : this()
		{
			this.Module = module ?? throw new ArgumentNullException(nameof(module));
			this.Handle = handle;
		}

		SRMDocument This() => Module.GetMetadataReader().GetDocument(Handle);

		public bool Equals(Document other)
		{
			return Module == other.Module && Handle == other.Handle;
		}

		public override bool Equals(object obj)
		{
			if (obj is Document md)
				return Equals(md);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Module.GetHashCode() + 982451653 * MetadataTokens.GetToken(Handle));
		}

		public static bool operator ==(Document lhs, Document rhs) => lhs.Equals(rhs);
		public static bool operator !=(Document lhs, Document rhs) => !lhs.Equals(rhs);

		public string Url {
			get {
				if (Handle.IsNil)
					return null;
				var h = This().Name;
				if (h.IsNil) return null;
				return Module.GetMetadataReader().GetString(h);
			}
		}
	}
}
