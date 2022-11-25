using FluentAssertions;

namespace QueuetyServer.Tests;

public class ServerTests
{
    private Server _server = null!;

    [SetUp]
    public void Setup()
    {
        _server = new Server(new ServerSettings());
        SystemTime.UtcNow = () => DateTime.Parse("2022-01-01T00:00:00Z");
    }

    [Test]
    public void AddMessage_Succeeds()
    {
        _server.AddMessage("queue", new Message
        {
            Key = "key",
            Subject = "subject",
            Data = "data"
        });
    }

    [Test]
    public void AddMessage_MessageWithSubjectAlreadyExists_MessageIsReplaced()
    {
        _server.AddMessage("queue", new Message
        {
            Subject = "subject",
            Key = "key1",
            Data = "data1"
        });
        
        _server.AddMessage("queue", new Message
        {
            Subject = "subject",
            Key = "key2",
            Data = "data2"
        });

        var batch = _server.GetBatch("queue", 1);

        batch.Messages[0].Key.Should().Be("key2");
        batch.Messages[0].Data.Should().Be("data2");
    }

    [Test]
    public void GetBatch_EmptyQueue_ReturnsNothing()
    {
        var result = _server.GetBatch("queue", 1);

        result.BatchId.Should().BeEmpty();
        result.Messages.Should().BeEmpty();
    }

    [Test]
    public void GetBatch_WithMessage_ReturnsOneItem()
    {
        var message = new Message
        {
            Subject = "subject",
            Key = "key",
            Data = "data"
        };
        _server.AddMessage("queue", message);

        var result = _server.GetBatch("queue", 1);

        result.BatchId.Should().NotBeNull();
        result.Messages[0].Should().Be(message);
    }

    [Test]
    public void GetBatch_NoMoreAvailableMessages_ReturnsEmpty()
    {
        var message = new Message
        {
            Subject = "subject",
            Key = "key",
            Data = "data"
        };
        _server.AddMessage("queue", message);

        var _ = _server.GetBatch("queue", 1);
        var result = _server.GetBatch("queue", 1);
    
        result.BatchId.Should().BeEmpty();
        result.Messages.Should().BeEmpty();
    }

    [Test]
    public void GetBatch_BatchExpires_ReturnsBatch()
    {
        var message = new Message
        {
            Subject = "subject",
            Key = "key",
            Data = "data"
        };

        _server.AddMessage("queue", message);

        var _ = _server.GetBatch("queue", 1);

        SystemTime.UtcNow = () => DateTime.Parse("2022-01-01T00:02:00Z");
        
        var result = _server.GetBatch("queue", 1);
        
        result.BatchId.Should().NotBeNull();
        result.Messages[0].Should().Be(message);    
    }
}