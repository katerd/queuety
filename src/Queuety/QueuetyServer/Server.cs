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

    public readonly Queue<Message> Messages = new();
    private readonly List<MessageBatch> _batches = new();

    public void Enqueue(Message message)
    {
        Messages.Enqueue(message);
    }

    private Message Dequeue()
    {
        return Messages.Dequeue();
    }

    public MessageBatch GetBatch(int batchSize)
    {
        var expiredBatch = _batches.FirstOrDefault(x => x.HasExpired);
        if (expiredBatch != null)
        {
            expiredBatch.Expiry = CalculateExpiry();
            return expiredBatch; 
        }
        
        var result = new List<Message>(batchSize);
        while (Messages.Count > 0 && result.Count < batchSize)
        {
            var message = Dequeue();
            result.Add(message);
        }

        if (result.Count == 0)
            return MessageBatch.EmptyBatch;

        var messageBatch = new MessageBatch
        {
            BatchId = Guid.NewGuid().ToString(),
            Messages = result
        };
        
        _batches.Add(messageBatch);
        
        return messageBatch;
    }

    private DateTime CalculateExpiry()
    {
        var expiry = SystemTime.UtcNow().Add(TimeSpan.FromSeconds(_settings.BatchExpirySeconds));
        return expiry;
    }
}

public class Server
{
    private readonly ServerSettings _settings;

    public Server(ServerSettings settings)
    {
        _settings = settings;
    }

    private readonly Dictionary<string, ServerQueue> _messages = new();
    
    public void AddMessage(string queueName, Message message)
    {
        if (_messages.TryGetValue(queueName, out var queue))
        {
            foreach (var existingMessage in queue.Messages)
            {
                if (existingMessage == null)
                    break;

                if (!string.Equals(existingMessage.Subject, message.Subject)) 
                    continue;
                
                existingMessage.Data = message.Data;
                existingMessage.Key = message.Key;
                
                return;
            }
            
            queue.Enqueue(message);
        }
        else
        {
            var newMessages = new ServerQueue(_settings);
            newMessages.Enqueue(message);
            _messages[queueName] = newMessages;
        }
    }

    public void DeleteQueue(string queueName)
    {
        throw new NotImplementedException();
    }

    public void DeleteMessages(string queueName)
    {
        throw new NotImplementedException();
    }

    public void DeleteMessage(string messageKey)
    {
        throw new NotImplementedException();
    }
    
    public MessageBatch GetBatch(string queueName, int batchSize)
    {
        return _messages.TryGetValue(queueName, out var messages) 
            ? messages.GetBatch(batchSize) 
            : MessageBatch.EmptyBatch;
    }

    public void CommitBatch(string queueName, string batchId)
    {
        throw new NotImplementedException();
    }
}