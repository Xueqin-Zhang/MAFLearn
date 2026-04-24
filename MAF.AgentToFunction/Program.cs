using ClassLibrary;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel;

/**
 * 在MAF中，Agent可以看作一个函数在另一个Agent调用。
 */

[Description("Get current weather for a location")]
static async Task<string> GetCurrentWeatherAsync(string location)
{
    return $"The weather is {location} is cloudy with a hight of 31℃";
}


var ollamaSharpClient = new OllamaApiClient(Utils.Host, Utils.Model);
var weatherAgent = ollamaSharpClient.AsAIAgent(
        instructions: "你是一个天气助力，可以用来查询天气信息",
        name: "GetWeatherInfo",
        description: "获取地区天气信息",
        tools: [AIFunctionFactory.Create(GetCurrentWeatherAsync)]
    );

var mainAgent = (new OllamaApiClient(Utils.Host, Utils.Model))
    .AsAIAgent(
        instructions: "你是个人助力",
        tools: [weatherAgent.AsAIFunction()]
    );
Console.WriteLine(await mainAgent.RunAsync("获取北京的天气"));