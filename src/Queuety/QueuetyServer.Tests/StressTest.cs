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

        for (var i = 0; i < 200_000; i++)
        {
            server.AddMessage("queue", new Message
            {
                Data = Guid.NewGuid().ToString(),
                Key = Guid.NewGuid().ToString(),
                Subject = Guid.NewGuid().ToString()
            });
        }
        
        sw.Stop();
        Console.WriteLine(sw.Elapsed);
    }
}