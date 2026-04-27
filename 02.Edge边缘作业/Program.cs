

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
        new OpenAIClientOptions { Endpoint = new Uri("https://www-dev.h603f1ec4.nyat.app:28367/v1") })
    .GetChatClient("frob/qwen3.5-instruct:4b");


var detectAiAgent = AssistantFactory.CreateDectionAiAgent(openAiClient);
var analyizeAiAgent = AssistantFactory.CreateEmailAnalyizeAiAgent(openAiClient);
var emailReplayeAiAgent = AssistantFactory.CreateEmailReplayAiAgent(openAiClient);

var detectExecutor = new SpamDetectionExecutor(detectAiAgent);
var emailAssistantExecutor = new EmailAssistantExecutor(emailReplayeAiAgent);
var emailFileSaveExecutor = new EmailFileSaveExecutor();
var analyizeEmailExecutor = new AnalyizeEmailExecutor(analyizeAiAgent);
var spamEmailExecutor = new SpamEmailExecutor();
var unCertainEmailExecutor = new UnCertainEmailExecutor();

Func<DetectionReslt?, int, IEnumerable<int>> GetSelector()
{
    return (result, targetCount) =>
    {
        if(result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }
        if(result.SpamDecision == SpamDecision.Spam)
        {
            return [0];
        }
        
        if(result.SpamDecision == SpamDecision.NotSpam)
        {
            return [1, 2, 3,];
        }

        if(result.SpamDecision == SpamDecision.Uncertain)
        {
            return [4];
        }

        throw new ArgumentOutOfRangeException(nameof(result));
    };
}

var workflow = new WorkflowBuilder(detectExecutor)
    .AddFanOutEdge(
        detectExecutor, 
        targets: [
            spamEmailExecutor, 
            emailAssistantExecutor,
            emailFileSaveExecutor,
            analyizeEmailExecutor,
            unCertainEmailExecutor
        ],
        targetSelector: GetSelector())
    .WithOutputFrom(detectExecutor, spamEmailExecutor, emailAssistantExecutor, emailFileSaveExecutor, analyizeEmailExecutor, unCertainEmailExecutor)
    .Build();

// 执行

var input = """
    发件人：龙飞公司人力资源部 hr@company.com邮件内容：您好！现通知你本周周五下午 15:00 前往三楼会议室参加季度工作复盘会议，请提前整理好个人工作周报、项目进度报表。会议全程需佩戴工牌，请勿迟到缺席，如有特殊情况请提前私信人事报备。祝工作顺利！
    """;

var run = await InProcessExecution.RunStreamingAsync(workflow, input);

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