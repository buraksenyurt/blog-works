using System;
using Microsoft.Extensions.VectorData;

namespace BlogPilot.Models
{
    public class BlogPostChunk
    {
        [VectorStoreKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [VectorStoreData]
        public string Title { get; set; } = string.Empty;
        
        [VectorStoreData]
        public string Slug { get; set; } = string.Empty;
        
        [VectorStoreData]
        public string PubDate { get; set; } = string.Empty;
        
        [VectorStoreData]
        public int Year { get; set; }
        
        [VectorStoreData]
        public string Categories { get; set; } = string.Empty;
        
        [VectorStoreData]
        public string Tags { get; set; } = string.Empty;

        [VectorStoreData]
        public string SourcePath { get; set; } = string.Empty;

        [VectorStoreData]
        public string ChunkType { get; set; } = "prose";
        
        [VectorStoreData]
        public string Language { get; set; } = string.Empty;
        
        [VectorStoreData]
        public string Text { get; set; } = string.Empty;

        [VectorStoreVector(768)]
        public ReadOnlyMemory<float> Vector { get; set; }
    }
}
