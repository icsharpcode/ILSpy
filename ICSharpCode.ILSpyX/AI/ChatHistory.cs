// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class ChatConversation
	{
		public string Id { get; set; } = Guid.NewGuid().ToString("N");
		public AIConversationTarget? Target { get; set; }
		public List<ChatMessage> Messages { get; set; } = new();
		public bool ReadOnly { get; set; }
		[JsonIgnore]
		public bool TargetDeleted { get; set; }

		[JsonIgnore]
		public string DisplayName {
			get {
				if (Target is null)
					return "Legacy conversation (read-only)";
				string name = string.IsNullOrWhiteSpace(Target.ProfileName) ? "Unknown profile" : Target.ProfileName;
				string model = string.IsNullOrWhiteSpace(Target.Model) ? "Unknown model" : Target.Model;
				string state = TargetDeleted ? " (deleted)" : ReadOnly ? " (read-only)" : string.Empty;
				return $"{name} / {model}{state}";
			}
		}
	}

	public sealed class ChatHistory
	{
		static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
		public int SchemaVersion { get; set; } = 2;
		public string AssemblyPath { get; set; } = string.Empty;
		public List<ChatConversation> Conversations { get; set; } = new();
		public string ActiveConversationId { get; set; } = string.Empty;

		[JsonIgnore]
		public ChatConversation ActiveConversation {
			get {
				ChatConversation? conversation = Conversations.FirstOrDefault(c => c.Id == ActiveConversationId) ?? Conversations.FirstOrDefault();
				if (conversation is null)
				{
					conversation = new ChatConversation();
					Conversations.Add(conversation);
				}
				ActiveConversationId = conversation.Id;
				return conversation;
			}
		}

		[JsonIgnore]
		public List<ChatMessage> Messages => ActiveConversation.Messages;

		public ChatConversation GetOrCreate(AIConversationTarget target)
		{
			ArgumentNullException.ThrowIfNull(target);
			ChatConversation? existing = Conversations.FirstOrDefault(c => !c.ReadOnly
				&& c.Target?.BelongsTo(target.ProfileId, target.ProviderType, target.Endpoint, target.Model) == true);
			if (existing is not null)
			{
				ActiveConversationId = existing.Id;
				return existing;
			}
			var created = new ChatConversation { Target = target };
			Conversations.Add(created);
			ActiveConversationId = created.Id;
			return created;
		}

		/// <summary>Creates and selects a fresh writable conversation for the target.</summary>
		public ChatConversation StartNew(AIConversationTarget target)
		{
			ArgumentNullException.ThrowIfNull(target);
			var created = new ChatConversation { Target = target, ReadOnly = false };
			Conversations.Add(created);
			ActiveConversationId = created.Id;
			return created;
		}

		public bool TrySelect(string conversationId)
		{
			ChatConversation? conversation = Conversations.FirstOrDefault(c => string.Equals(c.Id, conversationId, StringComparison.Ordinal));
			if (conversation is null)
				return false;
			ActiveConversationId = conversation.Id;
			return true;
		}

		public static ChatHistory Load(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				return new ChatHistory();
			try
			{
				using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
				return FromJson(document.RootElement);
			}
			catch (IOException) { return new ChatHistory(); }
			catch (JsonException) { return new ChatHistory(); }
		}

		public static async Task<ChatHistory> LoadAsync(string path, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				return new ChatHistory();
			try
			{
				await using var stream = File.OpenRead(path);
				using JsonDocument document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
				return FromJson(document.RootElement);
			}
			catch (IOException) { return new ChatHistory(); }
			catch (JsonException) { return new ChatHistory(); }
		}

		static ChatHistory FromJson(JsonElement root)
		{
			if (root.ValueKind != JsonValueKind.Object)
				return new ChatHistory();
			int schema = root.TryGetProperty("SchemaVersion", out JsonElement version) && version.TryGetInt32(out int parsed) ? parsed : 1;
			if (schema >= 2 && root.TryGetProperty("Conversations", out _))
			{
				ChatHistory? history = JsonSerializer.Deserialize<ChatHistory>(root.GetRawText(), JsonOptions);
				if (history is null)
					return new ChatHistory();
				history.SchemaVersion = 2;
				history.Conversations ??= new List<ChatConversation>();
				history.Conversations = history.Conversations.Where(c => c is not null && !string.IsNullOrWhiteSpace(c.Id)).ToList();
				foreach (ChatConversation conversation in history.Conversations)
				{
					conversation.Messages ??= new List<ChatMessage>();
					if (conversation.Target is null)
						conversation.ReadOnly = true;
				}
				if (!history.TrySelect(history.ActiveConversationId))
					history.ActiveConversationId = history.Conversations.FirstOrDefault()?.Id ?? string.Empty;
				return history;
			}

			var legacy = new ChatHistory { SchemaVersion = 2 };
			string assemblyPath = root.TryGetProperty("AssemblyPath", out JsonElement assembly) ? assembly.GetString() ?? string.Empty : string.Empty;
			legacy.AssemblyPath = assemblyPath;
			var legacyConversation = new ChatConversation { ReadOnly = true, Target = null };
			if (root.TryGetProperty("Messages", out JsonElement messages) && messages.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement message in messages.EnumerateArray())
				{
					try
					{
						ChatMessage? item = JsonSerializer.Deserialize<ChatMessage>(message.GetRawText(), JsonOptions);
						if (item is not null)
							legacyConversation.Messages.Add(item);
					}
					catch (JsonException) { }
				}
			}
			legacy.Conversations.Add(legacyConversation);
			legacy.ActiveConversationId = legacyConversation.Id;
			return legacy;
		}

		public void Save(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return;
			string directory = Path.GetDirectoryName(path) ?? string.Empty;
			if (directory.Length != 0)
				Directory.CreateDirectory(directory);
			string temporary = path + ".tmp";
			File.WriteAllText(temporary, JsonSerializer.Serialize(this, JsonOptions), Encoding.UTF8);
			File.Move(temporary, path, true);
		}

		public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(path))
				return;
			string directory = Path.GetDirectoryName(path) ?? string.Empty;
			if (directory.Length != 0)
				Directory.CreateDirectory(directory);
			string temporary = path + ".tmp";
			await using (var stream = File.Create(temporary))
				await JsonSerializer.SerializeAsync(stream, this, JsonOptions, cancellationToken).ConfigureAwait(false);
			File.Move(temporary, path, true);
		}

		public string ToMarkdown(string? title = null)
		{
			var builder = new StringBuilder();
			builder.Append("# ").AppendLine(title ?? "AI Chat");
			builder.AppendLine();
			foreach (ChatConversation conversation in Conversations)
			{
				if (conversation.Target is { } target)
					builder.Append("_Target: ").Append(target.ProfileName).Append(" / ").Append(target.ProviderType).Append(" / ").Append(target.Model).Append(" / ").Append(target.Endpoint).AppendLine("_");
				foreach (ChatMessage message in conversation.Messages)
				{
					builder.AppendLine("## " + (message.IsUser ? "User" : "Assistant"));
					builder.AppendLine();
					builder.AppendLine(message.Content);
					builder.AppendLine();
				}
			}
			return builder.ToString();
		}
	}
}
