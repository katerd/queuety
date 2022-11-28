namespace QueuetyServer;

public class Message
{
    public string Key { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;

    public void UpdateMessage(Message from)
    {
        Data = from.Data;
        Key = from.Key;
    }
}