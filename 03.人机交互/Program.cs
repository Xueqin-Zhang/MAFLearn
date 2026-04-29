using HitlDemo;
using HitlDemo.Executors;
using Microsoft.Agents.AI.Workflows;

var numberRequestPort = RequestPort.Create<NumberSignal, int>("GuessNumber");
var judgeExecutor = new JudgeExecutor(42);

var workflow = new WorkflowBuilder(numberRequestPort)
    .AddEdge(numberRequestPort, judgeExecutor)
    .AddEdge(judgeExecutor, numberRequestPort)
    .WithOutputFrom(judgeExecutor)
    .Build();

StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, NumberSignal.Init);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));




Console.WriteLine("Guess number demo started.");

await foreach (var evt in run.WatchStreamAsync())
{
    switch (evt)
    {
        case RequestInfoEvent requestInfoEvent:
            while (true)
            {
                Console.WriteLine($"当前请求：{requestInfoEvent.Request.PortInfo.PortId}");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var guess))
                {
                    await run.SendResponseAsync(requestInfoEvent.Request.CreateResponse(guess));
                    break;
                }
                Console.Write("Invalid input. Enter an integer: ");
            }
            break;

        case WorkflowOutputEvent outputEvent:
            Console.WriteLine(outputEvent.Data);
            return;

        case WorkflowErrorEvent workflowError:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(workflowError.Exception?.ToString() ?? "Workflow execution failed.");
            Console.ResetColor();
            return;
    }
}
