

using _02.Edge边缘作业;
using _02.Edge边缘作业.Executors;
using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI.Workflows;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;

/**
 * 目标：实现一个邮件系统AI判定
 *      1：判断邮件类型：正常、垃圾邮件、未知。
 *      2：若是未知邮件，进行归档，由人工自行处理。
 *      3：若是正常邮件：助手回复、分析以及存档。
 */
var openAiClient = new OpenAIClient(
        new ApiKeyCredential("1"),
        new OpenAIClientOptions { Endpoint = new Uri("http://localhost:11434/v1") })
    .GetChatClient("frob/qwen3.5-instruct:4b");


var detectAiAgent = AssistantFactory.CreateDectionAiAgent(openAiClient);


var detectExecutor = new SpamDetectionExecutor(detectAiAgent);

var workflow = new WorkflowBuilder(detectExecutor)
    .WithOutputFrom(detectExecutor)
    .Build();

// 执行
var run = await InProcessExecution.RunStreamingAsync(workflow, "恭喜你中奖1个亿，点击链接即可领取！");

await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

await foreach (var evt in run.WatchStreamAsync())
{
    if(evt is WorkflowOutputEvent outputEvent)
    {
        if(outputEvent.Data is string)
        {
            Console.WriteLine($"{outputEvent}");
        }
        
        if(outputEvent.Data is DetectionReslt data)
        {
            Console.WriteLine(data.Reason);
        }
        
    }
}