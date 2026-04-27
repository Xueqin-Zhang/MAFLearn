using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class UnCertainEmailExecutor() : Executor<DetectionReslt>("UnCertainEmailExecutor")
{
    public override async ValueTask HandleAsync(DetectionReslt message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("Step 不确定邮件信息");
        await context.YieldOutputAsync($"原因：{message.Reason}");
    }
}
