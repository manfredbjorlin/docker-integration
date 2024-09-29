public static class Logger
{
    public enum LogLevel{Debug, Info, Error}

    public static LogLevel LoggingLevel = LogLevel.Debug;
    public static void WriteInfo(string message)
    {
        if(LoggingLevel <= LogLevel.Info)
            Statics.Logger!.LogInformation(message);
    }

    public static void WriteDebug(string message)
    {
        if(LoggingLevel == LogLevel.Debug)
            Statics.Logger!.LogDebug(message);
    }

    public static void WriteError(string message)
    {
        Statics.Logger!.LogError(message);
    }
}