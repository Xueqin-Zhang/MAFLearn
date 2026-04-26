using _01.边缘.Models;
using Microsoft.Agents.AI.Workflows;
using System;
using System.Collections.Generic;
using System.Text;

namespace _01.边缘.Exectors;

[YieldsOutput(typeof(string))]
internal sealed  class SendMailExecutor() : Executor<EmailResponse>("SendEmailExecutor") {
    public override async ValueTask HandleAsync(EmailResponse message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync($"邮件发送：{message.Response}");
    }
}
