using _02.Edge边缘作业.Models;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业.Executors;

[YieldsOutput(typeof(string))]
internal class EmailFileSaveExecutor() : Executor<DetectionReslt>("EmailFileSaveExecutor")
{
    public override async ValueTask HandleAsync(DetectionReslt message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync("Step 邮件归档");
    }
}
