using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class AnalyizeEmailExecutor(AIAgent _agent) : Executor<DetectionReslt, AnalyizeEmail>("AnalyizeEmailExecutor")
{
    public override async ValueTask<AnalyizeEmail> HandleAsync(DetectionReslt message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("Step 邮件分析");
        var response = await _agent.RunAsync<AnalyizeEmail>(message.Reason);
        await context.YieldOutputAsync(response.Result.Reason);
        return response.Result;
    }
}
