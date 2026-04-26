using _01.持久化Agent.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace _01.持久化Agent; 
internal class ChatHistoryDbContext : DbContext {

    public DbSet<CustomChatHistory> ChatHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(@"D:\编程\个人\Demo\MAFLearn\01.持久化Agent\ChatHistoryDb.db");
    }
}
