using System;
using System.Collections.Generic;
using System.Linq;

namespace QueuetyServer;

public class ServerQueue
{
    private readonly ServerSettings _settings;

    public ServerQueue(ServerSettings settings)
    {
        _settings = settings;
    }

    private readonly Queue<Message> _messages = new();
    private readonly List<MessageBatch> _batches = new();
    private readonly Dictionary<string, Message> _subject = new();

    public void Enqueue(Message message)
    {
        _subject[message.Subject] = message;
        _messages.Enqueue(message);
    }

    private Message Dequeue()
    {
        var message = _messages.Dequeue();
        _subject.Remove(message.Subject);
        return message;
    }

    private MessageBatch? GetExpiredBatch()
    {
        var first = _batches.FirstOrDefault();
        return first?.HasExpired ?? false ? first : null;
    }
    
    public MessageBatch GetBatch(int batchSize)
    {
        var expiredBatch = GetExpiredBatch();
        if (expiredBatch != null)
        {
            expiredBatch.Expiry = CalculateExpiry();
            return expiredBatch;
        }

        var result = new List<Message>(batchSize);
        while (_messages.Count > 0 && result.Count < batchSize)
        {
            var message = Dequeue();
            result.Add(message);
        }

        return result.Count == 0 
            ? MessageBatch.EmptyBatch 
            : AddBatch(result.ToArray());
    }

    private MessageBatch AddBatch(Message[] result)
    {
        var batch = new MessageBatch
        {
            BatchId = Guid.NewGuid().ToString(),
            Messages = result,
            Expiry = CalculateExpiry()
        };
        
        _batches.Add(batch);
        
        return batch;
    }

    private DateTime CalculateExpiry()
    {
        var timespan = TimeSpan.FromSeconds(_settings.BatchExpirySeconds);
        var expiry = SystemTime.UtcNow().Add(timespan);
        return expiry;
    }

    public void CommitBatch(string batchId)
    {
        _batches.RemoveAll(x => x.BatchId == batchId);
    }

    public void DeleteMessages()
    {
        _messages.Clear();
    }

    public void AddMessage(Message message)
    {
        if (_subject.TryGetValue(message.Subject, out var matchingMessage))
        {
            matchingMessage.UpdateMessage(message);
            return;
        }
        
        Enqueue(message);
    }
}