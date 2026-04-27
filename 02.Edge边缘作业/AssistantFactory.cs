using Microsoft.Agents.AI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace _02.Edge边缘作业; 
internal static class AssistantFactory {

    public static AIAgent CreateDectionAiAgent(ChatClient chatClient) => chatClient.AsAIAgent(
            instructions: """
                你是专业邮件处理 AI 助手，负责对收到的邮件内容、发件信息、行文目的进行精准分类判定，仅划分三类：
        正常邮件（UnSpam）：正规沟通、工作往来、亲友通知、官方通知、业务对接、合法合理有效信息，内容正经无诱导、无广告骚扰、无违规引流，为日常有效沟通类邮件。
        垃圾邮件（Spam）：广告推销、低俗引流、刷单兼职、诈骗链接、红包福利、低价售卖、骚扰推广、博彩 / 理财诱导、重复群发营销、恶意骚扰类内容，全部判定为垃圾邮件。
        不确定邮件（UnCertain）：内容存在模糊，需要人工审查。
        严格按照以上明确边界规则逐一判断，只用中文输出分类结果 + 简短判定理由。。
        """,
            description: "请使用中文回答。"
        );


    public static AIAgent CreateEmailAnalyizeAiAgent(ChatClient chatClient) => chatClient.AsAIAgent(
            instructions: "你是一个邮件分析助手，专门用来分析邮件，以及邮件用途。请使用中文回答。",
            description: "请使用中文回答。"
        );

    public static AIAgent CreateEmailReplayAiAgent(ChatClient chatClient) => chatClient.AsAIAgent(
           instructions: "你是一个邮件回复助手，根据邮件内容做出相应的答复。请使用中文回答。",
           description: "请使用中文回答。"
       );
}
