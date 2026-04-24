
using MAFRagDemo;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp;
#pragma warning disable SKEXP0130

var ollamaSharClient = new OllamaApiClient("https://www-dev.h603f1ec4.nyat.app:28367", "qwen3.5:4b");

// 知识库声明信息11
var db = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db.sqlite");

var store = new InMemoryVectorStore(new()
{
    EmbeddingGenerator = new OllamaApiClient("https://www-dev.h603f1ec4.nyat.app:28367", "bge-m3:latest")
});
var collection = store.GetCollection<Guid, Model>("base");
await collection.EnsureCollectionDeletedAsync();
await collection.EnsureCollectionExistsAsync();

var rrt = new OllamaApiClient("https://www-dev.h603f1ec4.nyat.app:28367", "bge-m3:latest");

// 插入数据
var markdow = await File.ReadAllLinesAsync("AML.md");
var chunks = new List<Model>();
for (int i = 0; i < 10; i++)
{
    var chunk = new Model()
    {
        Key = Guid.NewGuid(),
        Link = Path.GetFileName("AML.md"),
        Text = markdow[i]
    };
    chunk.Embedding = await rrt.GenerateAsync<string, Embedding<float>>(markdow[i], new EmbeddingGenerationOptions { Dimensions = 1024 });

    chunks.Add(chunk);
}
await collection.UpsertAsync(chunks);
Func<string, CancellationToken, Task<IEnumerable<TextSearchProvider.TextSearchResult>>> SearchAdapter =
    async (text, token) =>
    {
        List<TextSearchProvider.TextSearchResult> results = [];
        var vector = await rrt.GenerateAsync<string, Embedding<float>>(text, new EmbeddingGenerationOptions { Dimensions = 1024 });
       
        await foreach (var reslt in collection.SearchAsync(vector, 5, cancellationToken: token))
        {
            results.Add(new TextSearchProvider.TextSearchResult()
            {
                SourceName = reslt.Record.Text,
                SourceLink = reslt.Record.Link,
                Text = reslt.Record.Text ?? string.Empty,
                RawRepresentation = reslt
            });
        }
        return results;
    };


var textSearchOptions = new TextSearchProviderOptions()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
    RecentMessageMemoryLimit = 5
};

var agent = ollamaSharClient.AsAIAgent(
    new ChatClientAgentOptions()
    {
        AIContextProviders = [new TextSearchProvider(SearchAdapter, textSearchOptions)]
    });

var session = await agent.CreateSessionAsync();
var runOptions = new AgentRunOptions()
{
    AdditionalProperties = new AdditionalPropertiesDictionary()
    {
        ["think"] = false
    }
};

while (true)
{
    Console.Write("User：");
    var input = Console.ReadLine();

    var chatMessage = new ChatMessage(ChatRole.User, [
        new TextContent(input)
    ]);

    Console.WriteLine("Agent：");
    await foreach (var result in agent.RunStreamingAsync(chatMessage, session: session,options: runOptions))
    {
        Console.Write(result);
    }
    Console.Write("\n");
}

