using _01.边缘.Models;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace _01.边缘.Exectors; 
internal sealed partial class SpamDetectionExector : Executor<ChatMessage, DetectionResult> {
    private readonly AIAgent _spamDetectionAgent;

    /// <summary>
    /// Creates a new instance of the <see cref="SpamDetectionExecutor"/> class.
    /// </summary>
    /// <param name="spamDetectionAgent">The AI agent used for spam detection</param>
    public SpamDetectionExector(AIAgent spamDetectionAgent) : base("SpamDetectionExecutor")
    {
        this._spamDetectionAgent = spamDetectionAgent;
    }

    public override async ValueTask<DetectionResult> HandleAsync(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        // Generate a random email ID and store the email content to the shared state
        var newEmail = new Email
        {
            EmailId = Guid.NewGuid().ToString("N"),
            EmailContent = message.Text
        };
        await context.QueueStateUpdateAsync(newEmail.EmailId, newEmail, scopeName: EmailStateConstants.EmailStateScope, cancellationToken);

        // Invoke the agent
        var response = await this._spamDetectionAgent.RunAsync(message, cancellationToken: cancellationToken);
        var detectionResult = JsonSerializer.Deserialize<DetectionResult>(response.Text);

        detectionResult!.EmailId = newEmail.EmailId;

        return detectionResult;
    }
}
