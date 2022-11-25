using System;
using System.Collections.Generic;

namespace QueuetyServer;

public class MessageBatch
{
    public static readonly MessageBatch EmptyBatch = new();
    
    public string BatchId { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = new();
    public DateTime Expiry { get; set; } = SystemTime.UtcNow();
    
    public bool HasExpired => SystemTime.UtcNow() > Expiry;
}