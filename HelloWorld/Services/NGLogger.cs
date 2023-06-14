public static class NGLogger
{
    public enum LogLevel{Debug, Info, Error}

    public static LogLevel LoggingLevel = LogLevel.Debug;
    public static string? ApplicationName;

    public static void WriteInfo(string message)
    {
        if(LoggingLevel <= LogLevel.Info)
            Statics.Logger!.LogInformation(MakeMessage("Info :: " + message));
    }

    public static void WriteDebug(string message)
    {
        if(LoggingLevel == LogLevel.Debug)
            Statics.Logger!.LogDebug(MakeMessage("Debug :: " + message));
    }

    public static void WriteError(string message)
    {
        Statics.Logger!.LogError(MakeMessage("ERROR :: " + message));
    }

    private static string MakeMessage(string message)
    {
        return $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ffffzzz")} [{ApplicationName}] {message}";
    }
}