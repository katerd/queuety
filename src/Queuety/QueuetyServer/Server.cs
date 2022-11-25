using System;
using System.Collections.Generic;

namespace QueuetyServer;

public class Message
{
    public string Key { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

public class MessageBatch
{
    public string BatchId { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = new();
}

public class Server
{
    private readonly Dictionary<string, Queue<Message>> _messages = new();
    
    public void AddMessage(string queueName, Message message)
    {
        if (_messages.TryGetValue(queueName, out var messages))
        {
            foreach (var existingMessage in messages)
            {
                if (existingMessage == null)
                    break;

                if (!string.Equals(existingMessage.Subject, message.Subject)) 
                    continue;
                
                existingMessage.Data = message.Data;
                existingMessage.Key = message.Key;
                
                return;
            }
            
            messages.Enqueue(message);
        }
        else
        {
            var newMessages = new Queue<Message>();
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

    private static readonly MessageBatch EmptyBatch = new();
    
    public MessageBatch GetBatch(string queueName, int batchSize)
    {
        if (!_messages.TryGetValue(queueName, out var messages))
            return EmptyBatch;

        var result = new List<Message>(batchSize);
        while (messages.Count > 0 && result.Count < batchSize)
        {
            var message = messages.Dequeue();
            result.Add(message);
        }

        if (result.Count == 0)
            return EmptyBatch;

        return new MessageBatch
        {
            BatchId = Guid.NewGuid().ToString(),
            Messages = result
        };
    }

    public void CommitBatch(string queueName, string batchId)
    {
        throw new NotImplementedException();
    }
}