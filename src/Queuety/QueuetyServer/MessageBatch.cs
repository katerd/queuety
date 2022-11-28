using System;
using System.Collections.Generic;

namespace QueuetyServer;

public class MessageBatch
{
    public static readonly MessageBatch EmptyBatch = new();
    
    public string BatchId { get; set; } = string.Empty;
    public Message[]? Messages { get; set; }
    public DateTime Expiry { get; set; }
    
    public bool HasExpired => SystemTime.UtcNow() > Expiry;
}