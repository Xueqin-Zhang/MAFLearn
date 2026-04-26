using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace _01.持久化Agent.Models;

[Table(name: "custom_chat_history")]
internal class CustomChatHistory {
    public CustomChatHistory(string context)
    {
        this.Id = Guid.NewGuid().ToString("N");
        this.Context = context;
        this.CreatedOn = DateTime.Now;
    }

    public string Id { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }

    
}
