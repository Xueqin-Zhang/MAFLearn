using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace _01.边缘.Models; 
internal class Email {
    [JsonPropertyName("email_id")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("email_content")]
    public string EmailContent { get; set; } = string.Empty;
}
