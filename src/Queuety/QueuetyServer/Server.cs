using System.Collections.Generic;

namespace QueuetyServer;

public class Server
{
    private readonly ServerSettings _settings;

    public Server(ServerSettings settings)
    {
        _settings = settings;
    }

    private readonly Dictionary<string, ServerQueue> _queues = new();
    
    public void AddMessage(string queueName, Message message)
    {
        if (_queues.TryGetValue(queueName, out var queue))
        {
            queue.AddMessage(message);
        }
        else
        {
            var newMessages = new ServerQueue(_settings);
            newMessages.Enqueue(message);
            _queues[queueName] = newMessages;
        }
    }

    public void DeleteQueue(string queueName)
    {
        _queues.Remove(queueName);
    }

    public void DeleteMessages(string queueName)
    {
        GetQueue(queueName)?.DeleteMessages();
    }

    public MessageBatch GetBatch(string queueName, int batchSize)
    {
        return GetQueue(queueName)?.GetBatch(batchSize) ?? MessageBatch.EmptyBatch;
    }

    public void CommitBatch(string queueName, string batchId)
    {
        GetQueue(queueName)?.CommitBatch(batchId);
    }

    private ServerQueue? GetQueue(string queueName)
    {
        _queues.TryGetValue(queueName, out var result);
        return result;
    }
}