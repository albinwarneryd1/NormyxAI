namespace Sylvaro.Application.Rag;

public record RagSearchResult(Guid ChunkId, string SourceType, Guid? DocumentId, string ChunkText, float Score, string[] Tags);
