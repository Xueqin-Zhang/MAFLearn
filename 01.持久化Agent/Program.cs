
using _01.持久化Agent;
using _01.持久化Agent.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true)
    .Build();

var openAiProvider = config.GetSection("zhipu").Get<OpenAiProvider>()!;
var dbContext = new ChatHistoryDbContext();

var agent = new OpenAIClient(
        new ApiKeyCredential(openAiProvider.ApiKey),
        new OpenAIClientOptions { Endpoint = new Uri(openAiProvider.EndPoint) }
    )
    .GetChatClient(openAiProvider.ModelId)
    .AsAIAgent(name: "Powerful Assistant", instructions: "你是我的机器人，你可以回答我的消息");


var session = await agent.CreateSessionAsync();
var runOptions = new ChatClientAgentRunOptions
{
    ChatOptions = new Microsoft.Extensions.AI.ChatOptions
    {
        AdditionalProperties = new Microsoft.Extensions.AI.AdditionalPropertiesDictionary
        {
            ["think"] = false
        }
    }
};

string input = "";
while(true)
{
    input = Console.ReadLine() ?? "";
    if(input == "-1")
    {
        Console.WriteLine("结束，保存上下文");
        break;
    }

    Console.Write("\n");
    await foreach (var response in agent.RunStreamingAsync(input, session: session, options: runOptions))
    {
        Console.Write(response);
    }
    Console.Write("\nUser：");
}
var sessionSerialze = await agent.SerializeSessionAsync(session);
var history = new CustomChatHistory(sessionSerialze.ToString());
dbContext.ChatHistories.Add(history);
await dbContext.SaveChangesAsync();