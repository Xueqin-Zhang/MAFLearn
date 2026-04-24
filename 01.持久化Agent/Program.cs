
using _01.持久化Agent;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true)
    .Build();

var openAiProvider = config.GetSection("zhipu").Get<OpenAiProvider>()!;

var agent = new OpenAIClient(
        new ApiKeyCredential(openAiProvider.ApiKey),
        new OpenAIClientOptions { Endpoint = new Uri(openAiProvider.EndPoint) }
    )
    .GetChatClient(openAiProvider.ModelId)
    .AsAIAgent(name: "Powerful Assistant", instructions: "你是我的机器人，你可以回答我的消息");

Console.WriteLine(await agent.RunAsync("你好啊"));