using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace _01.边缘.Models; 
internal class DetectionResult {
    [JsonPropertyName("is_spam")]
    public bool IsSpam { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonIgnore]
    public string EmailId { get; set; } = string.Empty;
}
