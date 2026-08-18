// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class ChatMessage : INotifyPropertyChanged
	{
		string role = "user";
		string content = string.Empty;
		DateTimeOffset timestampUtc = DateTimeOffset.UtcNow;
		public string Role { get => role; set => Set(ref role, value ?? string.Empty); }
		public string Content { get => content; set => Set(ref content, value ?? string.Empty); }
		public DateTimeOffset TimestampUtc { get => timestampUtc; set => Set(ref timestampUtc, value); }
		public event PropertyChangedEventHandler? PropertyChanged;
		void Set<T>(ref T field, T value, [CallerMemberName] string? name = null) { if (!Equals(field, value)) { field = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); } }

		[JsonIgnore]
		public bool IsUser => string.Equals(Role, "user", StringComparison.Ordinal);
		[JsonIgnore]
		public bool IsAssistant => string.Equals(Role, "assistant", StringComparison.Ordinal);
	}
}
