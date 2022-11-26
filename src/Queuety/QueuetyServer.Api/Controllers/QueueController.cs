using Microsoft.AspNetCore.Mvc;

namespace QueuetyServer.Api.Controllers;

[Route("queue/{queueName}")]
public class QueueController : ControllerBase
{
    private readonly ILogger<QueueController> _logger;
    private readonly Server _server;

    public QueueController(
        ILogger<QueueController> logger,
        Server server)
    {
        _logger = logger;
        _server = server;
    }

    [HttpPost("message")]
    public void AddMessage([FromRoute] string queueName, [FromBody] Message message)
    {
        _server.AddMessage(queueName, message);
    }

    [HttpDelete("")]
    public void DeleteQueue([FromRoute] string queueName)
    {
        _server.DeleteQueue(queueName);
    }

    [HttpDelete("messages")]
    public void DeleteMessages([FromRoute] string queueName)
    {
        _server.DeleteMessages(queueName);
    }

    [HttpPost("batch")]
    public MessageBatch GetMessages([FromRoute] string queueName, [FromQuery] int batchSize = 10)
    {
        var batch = _server.GetBatch(queueName, batchSize);
        return batch;
    }

    [HttpPost("batch/{batchId}/commit")]
    public void CommitBatch([FromRoute]string queueName, [FromRoute]string batchId)
    {
        _server.CommitBatch(queueName, batchId);
    }
}