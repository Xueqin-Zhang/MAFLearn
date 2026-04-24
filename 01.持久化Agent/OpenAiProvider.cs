using System;
using System.Collections.Generic;
using System.Text;

namespace _01.持久化Agent;

internal class OpenAiProvider
{
    public string EndPoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ModelId { get; set; } = string.Empty;
}
