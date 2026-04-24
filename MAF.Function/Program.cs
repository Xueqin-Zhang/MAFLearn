
using MAF.Function;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

var ollamaClient = new OllamaApiClient("https://www-dev.h603f1ec4.nyat.app:28367", "qwen3.5:4b");

var approvalRequireTool = new ApprovalRequiredAIFunction(AIFunctionFactory.Create(WeatherServicePlugin.GetCurrentWeatherAsync));

var agent = ollamaClient.AsAIAgent(
        instructions: "你是一个帮助我的助理",
        tools: [AIFunctionFactory.Create(WeatherServicePlugin.GetCurrentWeatherAsync)]
    );

var session = await agent.CreateSessionAsync();
while (true)
{
    Console.Write("User：");
    var input = Console.ReadLine();
    var runOptions = new ChatClientAgentRunOptions
    {
        AdditionalProperties = new AdditionalPropertiesDictionary
        {
            ["think"] = false
        }
    };
    var response = await agent.RunAsync(input!, session: session, options: runOptions);
    var functionApprovalRequests = response.Messages.SelectMany(x => x.Contents)
        .OfType<ToolApprovalRequestContent>()
        .ToList();
    if(functionApprovalRequests.Count > 0)
    {
        List<ChatMessage> userInputResponse = functionApprovalRequests
            .ConvertAll(func => {
                Console.WriteLine($"想要调用你的函数，请回复Y批准，Name：{((FunctionCallContent)func.ToolCall).Name}");
                return new ChatMessage(ChatRole.User, [func.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);
            });
        response = await agent.RunAsync(userInputResponse, session: session, options: runOptions);
        functionApprovalRequests = response.Messages.SelectMany(m => m.Contents).OfType<ToolApprovalRequestContent>().ToList();
    }

    Console.WriteLine($"\nAgent：{response}");
}