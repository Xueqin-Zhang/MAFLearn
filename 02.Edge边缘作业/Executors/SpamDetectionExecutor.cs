using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class SpamDetectionExecutor : Executor<string, DetectionReslt> {

    private readonly AIAgent _assistantAiAgent;

    public SpamDetectionExecutor(AIAgent assistantAiAgent) : base("SpamDetectionExecutor")
    {
        _assistantAiAgent = assistantAiAgent;
    }

    public override async ValueTask<DetectionReslt> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("开始邮件类型判断");
        var newEnail = new
        {
            EmailId = Guid.NewGuid().ToString("N"),
            EmailContent = message
        };

        await context.QueueStateUpdateAsync(newEnail.EmailId, newEnail, scopeName: "State", cancellationToken);

        var result = await _assistantAiAgent.RunAsync<DetectionReslt>(message);
        result.Result.EmailId = newEnail.EmailId;
        return result.Result;
    }
}
