// Copyright (c) 2026 Masroor

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Annotations
{
	public sealed record RenameAnnotation(string Token, string NewName);

	/// <summary>Thread-safe sidecar storage for display-only symbol renames.</summary>
	public sealed class RenameAnnotationManager
	{
		static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) {
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		readonly object gate = new();
		readonly Dictionary<string, string> renames = new(StringComparer.OrdinalIgnoreCase);

		public RenameAnnotationManager(string assemblyPath)
		{
			if (string.IsNullOrWhiteSpace(assemblyPath))
				throw new ArgumentException("Assembly path cannot be empty.", nameof(assemblyPath));
			AssemblyPath = Path.GetFullPath(assemblyPath);
			SidecarPath = AssemblyPath + ".ilspy-annotations.json";
			AssemblyHash = ComputeAssemblyHash(AssemblyPath);
		}

		public string AssemblyPath { get; }
		public string SidecarPath { get; }
		public string AssemblyHash { get; }

		public IReadOnlyList<RenameAnnotation> Annotations
		{
			get { lock (gate) return renames.Select(pair => new RenameAnnotation(pair.Key, pair.Value)).ToArray(); }
		}

		public string? GetRename(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			if (!BelongsToAssembly(entity) || entity.MetadataToken.IsNil)
				return null;
			return GetRename(FormatToken(entity.MetadataToken));
		}

		public string? GetRename(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
				return null;
			lock (gate)
				return renames.TryGetValue(NormalizeToken(token), out string? value) ? value : null;
		}

		public void SetRename(IEntity entity, string newName)
		{
			ArgumentNullException.ThrowIfNull(entity);
			if (!BelongsToAssembly(entity))
				throw new ArgumentException("The entity does not belong to this assembly.", nameof(entity));
			SetRename(FormatToken(entity.MetadataToken), newName);
		}

		public void SetRename(string token, string newName)
		{
			if (string.IsNullOrWhiteSpace(token))
				throw new ArgumentException("Metadata token cannot be empty.", nameof(token));
			if (string.IsNullOrWhiteSpace(newName) || !IsValidIdentifier(newName))
				throw new ArgumentException("Rename must be a valid non-empty C# identifier.", nameof(newName));
			lock (gate)
				renames[NormalizeToken(token)] = newName.Trim();
		}

		public bool RemoveRename(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			if (!BelongsToAssembly(entity) || entity.MetadataToken.IsNil)
				return false;
			lock (gate)
				return renames.Remove(FormatToken(entity.MetadataToken));
		}

		public void Load()
		{
			if (!File.Exists(SidecarPath))
				return;
			try
			{
				LoadJson(File.ReadAllText(SidecarPath));
			}
			catch (IOException) { }
			catch (JsonException) { }
			catch (UnauthorizedAccessException) { }
		}

		public void Save()
		{
			lock (gate)
			{
				string json = JsonSerializer.Serialize(new AnnotationDocument {
					AssemblyHash = AssemblyHash,
					Renames = renames.Select(pair => new RenameAnnotation(pair.Key, pair.Value)).ToList()
				}, JsonOptions);
				string directory = Path.GetDirectoryName(SidecarPath) ?? ".";
				Directory.CreateDirectory(directory);
				string temporaryPath = SidecarPath + ".tmp-" + Guid.NewGuid().ToString("N");
				try
				{
					using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					using (var writer = new StreamWriter(stream))
					{
						writer.Write(json);
						writer.Flush();
						stream.Flush(true);
					}
					File.Move(temporaryPath, SidecarPath, true);
				}
				finally
				{
					if (File.Exists(temporaryPath))
						File.Delete(temporaryPath);
				}
			}
		}

		public string ToJson()
		{
			lock (gate)
			{
				return JsonSerializer.Serialize(new AnnotationDocument {
					AssemblyHash = AssemblyHash,
					Renames = renames.Select(pair => new RenameAnnotation(pair.Key, pair.Value)).ToList()
				}, JsonOptions);
			}
		}

		public void LoadJson(string json)
		{
			if (string.IsNullOrWhiteSpace(json))
				return;
			AnnotationDocument? document = JsonSerializer.Deserialize<AnnotationDocument>(json, JsonOptions);
			if (document is null || !string.Equals(document.AssemblyHash, AssemblyHash, StringComparison.OrdinalIgnoreCase))
				return;
			lock (gate)
			{
				renames.Clear();
				foreach (RenameAnnotation? annotation in document.Renames ?? Enumerable.Empty<RenameAnnotation>())
				{
					if (annotation is not null && IsValidToken(annotation.Token) && IsValidIdentifier(annotation.NewName))
						renames[NormalizeToken(annotation.Token)] = annotation.NewName.Trim();
				}
			}
		}

		public static string FormatToken(EntityHandle token)
		{
			if (token.IsNil)
				throw new ArgumentException("Metadata token cannot be nil.", nameof(token));
			return $"0x{MetadataTokens.GetToken(token):X8}";
		}

		static string NormalizeToken(string token)
		{
			if (!int.TryParse(token.Trim().Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase), System.Globalization.NumberStyles.HexNumber, null, out int value))
				throw new ArgumentException("Metadata token must be hexadecimal.", nameof(token));
			return $"0x{value:X8}";
		}

		static bool IsValidToken(string token)
		{
			try { _ = NormalizeToken(token); return true; }
			catch (ArgumentException) { return false; }
		}

		static bool IsValidIdentifier(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return false;
			if (!SyntaxFacts.IsIdentifierStartCharacter(value[0]))
				return false;
			for (int i = 1; i < value.Length; i++)
				if (!SyntaxFacts.IsIdentifierPartCharacter(value[i]))
					return false;
			return !ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpOutputVisitor.IsKeyword(value);
		}

		bool BelongsToAssembly(IEntity entity)
			=> entity.ParentModule?.MetadataFile?.FileName is { } path
				&& string.Equals(Path.GetFullPath(path), AssemblyPath, StringComparison.OrdinalIgnoreCase);

		static string ComputeAssemblyHash(string path)
		{
			if (!File.Exists(path))
				return string.Empty;
			using FileStream stream = File.OpenRead(path);
			return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
		}

		sealed class AnnotationDocument
		{
			[JsonPropertyName("assemblyHash")]
			public string AssemblyHash { get; set; } = string.Empty;
			[JsonPropertyName("renames")]
			public List<RenameAnnotation>? Renames { get; set; }
		}
	}

	// Kept local to avoid pulling a compiler package into ILSpyX.
	static class SyntaxFacts
	{
		public static bool IsIdentifierStartCharacter(char c) => c == '_' || char.IsLetter(c);
		public static bool IsIdentifierPartCharacter(char c) => c == '_' || char.IsLetterOrDigit(c);
	}
}
