using _01.边缘.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace _01.边缘;

internal class CustomAgentFactory {

    public static ChatClientAgent CreateSpamDetectionAgent(IChatClient chatClient) =>
        new ChatClientAgent(chatClient, new ChatClientAgentOptions { 
            ChatOptions = new ChatOptions
            {
                Instructions = "你是一个用于识别垃圾邮件的检测助手。请严格返回 JSON，字段只包含 is_spam 和 reason。 在 reason中首先指出原文信息，之后描述此信息为什么给标记为垃圾邮件 注意：请使用中文回复",
                ResponseFormat = ChatResponseFormat.ForJsonSchema(AIJsonUtilities.CreateJsonSchema(typeof(DetectionResult)))
            }
        });

    public static ChatClientAgent GetEmailAssistantAgent(IChatClient chatClient) =>
        new ChatClientAgent(chatClient, new ChatClientAgentOptions
        {
            ChatOptions = new ChatOptions
            {
                Instructions = "你是一个邮件助手，帮助用户撰写专业的电子邮件回复。请严格返回 JSON，字段只包含 response。注意：请使用中文回复",
                ResponseFormat = ChatResponseFormat.ForJsonSchema(AIJsonUtilities.CreateJsonSchema(typeof(EmailResponse)))
            }
        });
}
