using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class SpamEmailExecutor() : Executor<DetectionReslt>("SpamEmailExecutor")
{
    public override async ValueTask HandleAsync(DetectionReslt message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("Step 垃圾邮件处理");
        await context.YieldOutputAsync("-------------------------------------------------------");
        await context.YieldOutputAsync(message.Reason);
    }
}
