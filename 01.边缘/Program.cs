using _01.边缘;
using _01.边缘.Exectors;
using _01.边缘.Models;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

/**
 * 工作流：是用来进行任务编排，处理复杂业务流程。
 */
var openApiClient = new OpenAIClient(
        new ApiKeyCredential("123"),
        new OpenAIClientOptions { Endpoint = new Uri("http://localhost:11434/v1") }
    )
    .GetChatClient("frob/qwen3.5-instruct:4b")
    .AsIChatClient();

var spamDetectionAgent = CustomAgentFactory.CreateSpamDetectionAgent(openApiClient);
var emailAssistantAgent = CustomAgentFactory.GetEmailAssistantAgent(openApiClient);

var spamDetectionExecutor = new SpamDetectionExector(spamDetectionAgent);
var emailAssistantExecutor = new EmailAssistantExecutor(emailAssistantAgent);
var sendEmailExecutor = new SendMailExecutor();
var handleSpamExecutor = new HandleSpamExecutor();

Func<object?, bool> GetCondition(bool expectedResult) =>
        detectionResult => detectionResult is DetectionResult result && result.IsSpam == expectedResult;


// 工作流拼接
var workflow = new WorkflowBuilder(spamDetectionExecutor)
    .AddEdge(spamDetectionExecutor, emailAssistantExecutor, condition: GetCondition(expectedResult: false))
    .AddEdge(emailAssistantExecutor, sendEmailExecutor)
    .AddEdge(spamDetectionExecutor, handleSpamExecutor, condition: GetCondition(expectedResult: true))
    .WithOutputFrom(handleSpamExecutor, sendEmailExecutor)
    .Build();

string emailContent = "你好，周五下午2点有个会议，需要你参加下";
StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, new ChatMessage(ChatRole.User, emailContent));
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is WorkflowOutputEvent outputEvent)
    {
        Console.WriteLine($"{outputEvent}");
    }
    else if (evt is WorkflowErrorEvent workflowError)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(workflowError.Exception?.ToString() ?? "Unknown workflow error occurred.");
        Console.ResetColor();
    }
    else if (evt is ExecutorFailedEvent executorFailed)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"Executor '{executorFailed.ExecutorId}' failed with {(executorFailed.Data == null ? "unknown error" : $"exception {executorFailed.Data}")}.");
        Console.ResetColor();
    }
}
