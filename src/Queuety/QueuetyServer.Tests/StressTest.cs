using System.Diagnostics;

namespace QueuetyServer.Tests;

[TestFixture]
public class StressTest
{
    [Test, Explicit]
    public void Test()
    {
        var sw = new Stopwatch();
        sw.Start();

        var server = new Server(new ServerSettings());

        var rnd = new Random();
        const int max = 1_000_000;

        var startBytes = GC.GetTotalMemory(true);
        
        for (var i = 0; i < max; i++)
        {
            server.AddMessage("queue", new Message
            {
                Data = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString(),
                Subject = rnd.Next(0, max).ToString()
            });
        }

        var addMessageBytes = GC.GetTotalMemory(true);
        var allocBytes = addMessageBytes - startBytes;
        
        Console.WriteLine(sw.Elapsed);

        var batches = new MessageBatch[max];
        
        for (var i = 0; i < max; i++)
        {
            var batch = server.GetBatch("queue", rnd.Next(0, 5));
            batches[i] = batch;
        }

        var finalBytes = GC.GetTotalMemory(true);
        var getBatchBytes = finalBytes - addMessageBytes;
        
        Console.WriteLine(sw.Elapsed);

        Console.WriteLine(
            $"Memory allocations: Add:{allocBytes / (1024 * 1024.0f):N} MB Batch:{getBatchBytes / (1024 * 1024.0f):N} MB");
    }
}