// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class ChatHistory
	{
		static readonly JsonSerializerOptions JsonOptions = new()
		{
			WriteIndented = true
		};

		public string AssemblyPath { get; set; } = string.Empty;
		public List<ChatMessage> Messages { get; set; } = new();

		public static ChatHistory Load(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				return new ChatHistory();

			try
			{
				string json = File.ReadAllText(path, Encoding.UTF8);
				return JsonSerializer.Deserialize<ChatHistory>(json, JsonOptions) ?? new ChatHistory();
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
				return await JsonSerializer.DeserializeAsync<ChatHistory>(stream, JsonOptions, cancellationToken).ConfigureAwait(false) ?? new ChatHistory();
			}
			catch (IOException) { return new ChatHistory(); }
			catch (JsonException) { return new ChatHistory(); }
		}

		public void Save(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return;

			string directory = Path.GetDirectoryName(path) ?? string.Empty;
			if (directory.Length != 0)
				Directory.CreateDirectory(directory);

			File.WriteAllText(path, JsonSerializer.Serialize(this, JsonOptions), Encoding.UTF8);
		}

		public async Task SaveAsync(string path, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(path))
				return;

			string directory = Path.GetDirectoryName(path) ?? string.Empty;
			if (directory.Length != 0)
				Directory.CreateDirectory(directory);

			await using var stream = File.Create(path);
			await JsonSerializer.SerializeAsync(stream, this, JsonOptions, cancellationToken).ConfigureAwait(false);
		}

		public string ToMarkdown(string? title = null)
		{
			var builder = new StringBuilder();
			builder.Append("# ").AppendLine(title ?? "AI Chat");
			builder.AppendLine();
			foreach (var message in Messages)
			{
				builder.AppendLine("## " + (message.IsUser ? "User" : "Assistant"));
				builder.AppendLine();
				builder.AppendLine(message.Content);
				builder.AppendLine();
			}
			return builder.ToString();
		}
	}
}
