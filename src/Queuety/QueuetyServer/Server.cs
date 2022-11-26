using System;
using System.Collections.Concurrent;

namespace QueuetyServer;

public class Server
{
    private readonly ServerSettings _settings;

    public Server(ServerSettings settings)
    {
        _settings = settings;
    }

    private readonly ConcurrentDictionary<string, ServerQueue> _queues = new();
    
    public void AddMessage(string queueName, Message message)
    {
        lock (_queues)
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
    }

    public void DeleteQueue(string queueName)
    {
        _queues.TryRemove(queueName, out _);
    }

    public void DeleteMessages(string queueName)
    {
        WithQueue(queueName, queue => queue.DeleteMessages());
    }

    public MessageBatch GetBatch(string queueName, int batchSize)
    {
        return WithQueue(queueName,
            queue => queue.GetBatch(batchSize),
            MessageBatch.EmptyBatch);
    }

    public void CommitBatch(string queueName, string batchId)
    {
        WithQueue(queueName, queue => queue.CommitBatch(batchId));
    }

    private T WithQueue<T>(string queueName, Func<ServerQueue, T> action, T defaultValue)
    {
        if (!_queues.TryGetValue(queueName, out var queue))
            return defaultValue;

        lock (queue)
        {
            return action(queue);
        }
    }
    
    private void WithQueue(string queueName, Action<ServerQueue> action)
    {
        if (!_queues.TryGetValue(queueName, out var queue))
            return;

        lock (queue)
        {
            action(queue);
        }
    }
}