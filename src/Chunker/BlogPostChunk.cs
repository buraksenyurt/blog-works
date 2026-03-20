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
    /// <summary>ISO-8601 date string from frontmatter, e.g. "2024-10-31 06:28:00 +0300"</summary>
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
    /// <summary>
    /// Primary normalized programming language derived from categories/tags
    /// (e.g. "csharp", "rust", "ruby", "golang", "python").
    /// Empty string when the post is not language-specific.
    /// </summary>
    [VectorStoreData]
    public string ProgrammingLanguage { get; set; } = string.Empty;
    /// <summary>Workspace-relative path to the source markdown file.</summary>
    [VectorStoreData]
    public string SourcePath { get; set; } = string.Empty;

    // ── Chunk ─────────────────────────────────────────────────
    /// <summary>"prose" | "code" | "summary"</summary>
    [VectorStoreData]
    public string ChunkType { get; set; } = "prose";
    /// <summary>Programming language label for "code" chunks (e.g. "csharp", "rust", "python").</summary>
    [VectorStoreData]
    public string Language { get; set; } = string.Empty;
    /// <summary>
    /// The nearest section heading this chunk belongs to.
    /// Empty string for content before the first heading or for "summary" chunks.
    /// </summary>
    [VectorStoreData]
    public string SectionHeading { get; set; } = string.Empty;
    /// <summary>
    /// Full text stored in the vector store (metadata prefix + optional section heading + chunk body).
    /// The LLM receives this as context. Does NOT include the "search_document:" embedding task prefix.
    /// </summary>
    [VectorStoreData]
    public string Text { get; set; } = string.Empty;

    [VectorStoreVector(768)]
    public ReadOnlyMemory<float> Vector { get; set; }
}
