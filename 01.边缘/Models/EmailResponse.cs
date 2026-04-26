using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace _01.边缘.Models; 
internal class EmailResponse {
    [JsonPropertyName("response")]
    public string Response { get; set; }
}
