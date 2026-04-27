using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class EmailAssistantExecutor(
      AIAgent _emailAiAgent
    ) : Executor<DetectionReslt, EmailResult>("EmailAssistantExecutor")
{

    public override async ValueTask<EmailResult> HandleAsync(DetectionReslt message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("Step：邮件回复助手");
        if (message.SpamDecision != SpamDecision.NotSpam)
        {
            throw new InvalidOperationException("状态异常");
        }

        var result = await _emailAiAgent.RunAsync<EmailResult>(message.Reason);
        await context.YieldOutputAsync(result.Result.Replay);
        return result.Result;
    }
}
