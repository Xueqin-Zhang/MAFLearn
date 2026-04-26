using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace _02.Edge边缘作业.Models; 
internal class DetectionReslt {

    [JsonPropertyName("spam_decision")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SpamDecision SpamDecision { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonIgnore]
    public string EmailId { get; set; } = string.Empty;
}
