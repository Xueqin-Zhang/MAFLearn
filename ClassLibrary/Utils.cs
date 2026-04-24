using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary;

public static class Utils
{
    public static readonly string Host = "https://www-dev.h603f1ec4.nyat.app:28367";
    public static readonly string Model = "qwen3.5:4b";


    public static AIAgent CreateOllamaAiAgent()
    {
        var ollamaClient = new OllamaApiClient(Host, Model);
        return ollamaClient.AsAIAgent();
    }

    public static AIAgent CreateOllamaAiAgent(string name)
    {
        var ollamaClient = new OllamaApiClient(Host, Model);
        return ollamaClient.AsAIAgent(
                name: name
            );
    }

    public static OllamaApiClient CreateOllamaApiClient()
    {
        return new OllamaApiClient(Host, Model);
    }
}
