using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace MAFRagDemo;

public class Model
{
    [VectorStoreKey] public Guid Key { get; set; }
    [VectorStoreData] public string? Text { get; set; }

    [VectorStoreData] public string? Link { get; set; }

    [VectorStoreVector(1024)] public Embedding<float>? Embedding { get; set; }
}