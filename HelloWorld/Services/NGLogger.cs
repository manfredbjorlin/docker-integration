public static class NGLogger
{
    public enum LogLevel{Debug, Info, Error}

    public static LogLevel LoggingLevel = LogLevel.Debug;
    public static void WriteInfo(string message)
    {
        if(LoggingLevel <= LogLevel.Info)
            Statics.Logger!.LogInformation("Info :: " + message);
    }

    public static void WriteDebug(string message)
    {
        if(LoggingLevel == LogLevel.Debug)
            Statics.Logger!.LogDebug("Debug :: " + message);
    }

    public static void WriteError(string message)
    {
        Statics.Logger!.LogError("ERROR :: " + message);
    }
}