using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ClassLibrary; 
public static class AgentFactory {

    public static readonly string Host = "https://www-dev.h603f1ec4.nyat.app:28367";
    public static readonly string Model = "qwen3.5:4b";

    public static ChatClientAgentRunOptions RunOptions = new ChatClientAgentRunOptions()
    {
        ChatOptions = new ChatOptions
        {
            AdditionalProperties = new AdditionalPropertiesDictionary()
            {
                ["think"] = false
            }
        }
    };

    public static ChatClientAgent CreateOllamaAiAgent(string name, string instructions, string? model = "")
    {
        model = model ?? Model;
        var ollamaClient = new OllamaApiClient(Host, Model);
        return ollamaClient.AsAIAgent(
                name: name,
                instructions: instructions
            );
    }

    public static ChatClientAgent CreateOllamaAiAgent(string instructions, string name)
    {
        var ollamaClient = new OllamaApiClient(Host, Model);
        return ollamaClient.AsAIAgent(
                name: name,
                instructions: instructions
            );
    }


    public static OllamaApiClient CreatOllamaApiClient()
    {
        return new OllamaApiClient(Host, Model);
    }
}
