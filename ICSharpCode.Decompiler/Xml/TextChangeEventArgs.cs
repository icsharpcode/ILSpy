using System;

namespace ICSharpCode.Decompiler.Xml
{
	/// <summary>
	/// Describes a change of the document text.
	/// This class is thread-safe.
	/// </summary>
	[Serializable]
	public class TextChangeEventArgs : EventArgs
	{
		readonly int offset;
		readonly ITextSource removedText;
		readonly ITextSource insertedText;

		/// <summary>
		/// The offset at which the change occurs.
		/// </summary>
		public int Offset {
			get { return offset; }
		}

		/// <summary>
		/// The text that was removed.
		/// </summary>
		public ITextSource RemovedText {
			get { return removedText; }
		}

		/// <summary>
		/// The number of characters removed.
		/// </summary>
		public int RemovalLength {
			get { return removedText.TextLength; }
		}

		/// <summary>
		/// The text that was inserted.
		/// </summary>
		public ITextSource InsertedText {
			get { return insertedText; }
		}

		/// <summary>
		/// The number of characters inserted.
		/// </summary>
		public int InsertionLength {
			get { return insertedText.TextLength; }
		}

		/// <summary>
		/// Creates a new TextChangeEventArgs object.
		/// </summary>
		public TextChangeEventArgs(int offset, string removedText, string insertedText)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
			this.offset = offset;
			this.removedText = removedText != null ? new StringTextSource(removedText) : StringTextSource.Empty;
			this.insertedText = insertedText != null ? new StringTextSource(insertedText) : StringTextSource.Empty;
		}

		/// <summary>
		/// Creates a new TextChangeEventArgs object.
		/// </summary>
		public TextChangeEventArgs(int offset, ITextSource removedText, ITextSource insertedText)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", offset, "offset must not be negative");
			this.offset = offset;
			this.removedText = removedText ?? StringTextSource.Empty;
			this.insertedText = insertedText ?? StringTextSource.Empty;
		}

		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public virtual int GetNewOffset(int offset, AnchorMovementType movementType = AnchorMovementType.Default)
		{
			if (offset >= this.Offset && offset <= this.Offset + this.RemovalLength) {
				if (movementType == AnchorMovementType.BeforeInsertion)
					return this.Offset;
				else
					return this.Offset + this.InsertionLength;
			} else if (offset > this.Offset) {
				return offset + this.InsertionLength - this.RemovalLength;
			} else {
				return offset;
			}
		}

		/// <summary>
		/// Creates TextChangeEventArgs for the reverse change.
		/// </summary>
		public virtual TextChangeEventArgs Invert()
		{
			return new TextChangeEventArgs(offset, insertedText, removedText);
		}
	}
}
