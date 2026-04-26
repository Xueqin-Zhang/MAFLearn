using ClassLibrary;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System.Text;

var chatClient = AgentFactory.CreatOllamaApiClient();

var triageAgent = AgentFactory.CreateOllamaAiAgent(
    "你是决定那个Agent使用在回答一些作业问题",
    "triage_agent");
var historyTutorAgent = AgentFactory.CreateOllamaAiAgent(
    "您可提供历史查询方面的协助。清晰地解释重要事件及其背景。请仅就历史问题作出回应。",
    "history_tutor");
var mathTutorAgent = AgentFactory.CreateOllamaAiAgent(
    "您会帮助解决数学问题。请在每一步骤中说明你的推理过程，并附上示例。请仅就数学问题作答。",
    "math_tutor");

var workflow = AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent)
    .WithHandoffs(triageAgent, [historyTutorAgent, mathTutorAgent])
    .WithHandoffs([mathTutorAgent, historyTutorAgent], triageAgent)
    .Build();


var agent = workflow.AsAIAgent();
var session = await agent.CreateSessionAsync();

//while (true)
//{
//    Console.Write("User：");
//    string input = Console.ReadLine() ?? "";

//    Console.Write("Agent：");
//    await foreach (var response in agent.RunStreamingAsync(input, session: session, options: AgentFactory.RunOptions))
//    {
//        Console.Write(response);
//    }
//    Console.Write("\n");
//}

// 监听执行
StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, "你好，介绍洛阳的历史");

await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
Console.WriteLine("----------------------------------------------");
Console.WriteLine("Process Tracking");
Console.WriteLine("----------------------------------------------");
Console.WriteLine();
var result = new List<ChatMessage>();
var stageOutput = new StringBuilder();
int stemNumber = 1;
await foreach (var evt in run.WatchStreamAsync())
{
    if(evt is ExecutorCompletedEvent completedEvent)
    {
        if(stageOutput.Length > 0)
        {
            Console.WriteLine($"Step {stemNumber}：{completedEvent.ExecutorId}");
            Console.WriteLine($"Output：{stageOutput.ToString()}\n");
            stemNumber++;
            stageOutput.Clear();
        }
    }else if(evt is WorkflowEvent endEvent)
    {
        result = (List<ChatMessage>)endEvent.Data!;
        break;
    }
}
foreach (var message in result)
{
    Console.WriteLine($"Agent：{message.Text}");
}