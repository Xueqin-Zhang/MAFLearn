using Microsoft.Agents.AI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业; 
internal static class AssistantFactory {

    public static AIAgent CreateDectionAiAgent(ChatClient chatClient) => chatClient.AsAIAgent(
            instructions: "你是一个邮件处理助手，专门用来进行邮件分类处理，包含 垃圾邮件（Spam）、正常邮件（UnSpam）、不确定（UnCertain），请使用中文回答。",
            description: "请使用中文回答。"
        );
}
