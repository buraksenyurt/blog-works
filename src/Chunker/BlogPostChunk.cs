using Microsoft.Extensions.VectorData;

public class BlogPostChunk
{
    [VectorStoreKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    // ── Provenance ────────────────────────────────────────────
    [VectorStoreData]
    public string Title { get; set; } = string.Empty;
    [VectorStoreData]
    public string Slug { get; set; } = string.Empty;
    /// <summary>ISO-8601 date string from frontmatter, e.g. "2003-12-23 12:00:00"</summary>
    [VectorStoreData]
    public string PubDate { get; set; } = string.Empty;
    /// <summary>Publication year — used for range / equality filters on the client side.</summary>
    [VectorStoreData]
    public int Year { get; set; }
    /// <summary>Comma-separated list from frontmatter categories block.</summary>
    [VectorStoreData]
    public string Categories { get; set; } = string.Empty;
    /// <summary>Comma-separated list from frontmatter tags block.</summary>
    [VectorStoreData]
    public string Tags { get; set; } = string.Empty;

    // ── Chunk ─────────────────────────────────────────────────
    [VectorStoreData]
    public string ChunkType { get; set; } = "prose";   // "prose" | "code"
    [VectorStoreData]
    public string Language { get; set; } = string.Empty;
    /// <summary>
    /// Full text stored in the vector store (metadata prefix + chunk body).
    /// The LLM receives this as-is; metadata prefix provides provenance context.
    /// </summary>
    [VectorStoreData]
    public string Text { get; set; } = string.Empty;

    [VectorStoreVector(768)]
    public ReadOnlyMemory<float> Vector { get; set; }
}
