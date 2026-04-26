using _01.边缘.Models;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _01.边缘.Exectors;

/// <summary>
/// Executor that handles spam messages.
/// </summary>
[YieldsOutput(typeof(string))]
internal sealed class HandleSpamExecutor() : Executor<DetectionResult>("HandleSpamExecutor") {
    /// <summary>
    /// Simulate the handling of a spam message.
    /// </summary>
    public override async ValueTask HandleAsync(DetectionResult message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        if (message.IsSpam)
        {
            await context.YieldOutputAsync($"邮件标记 Spam: {message.Reason}", cancellationToken);
        }
        else
        {
            throw new InvalidOperationException("This executor should only handle spam messages.");
        }
    }
}