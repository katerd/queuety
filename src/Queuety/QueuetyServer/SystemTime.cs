using System;

namespace QueuetyServer;

public static class SystemTime
{
    public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
}