public static class NGLogger
{
    public enum LogLevel{Debug, Info, Error}

    public static LogLevel LoggingLevel = LogLevel.Debug;
    public static string? ApplicationName;

    public static void WriteInfo(string message)
    {
        if(LoggingLevel <= LogLevel.Info)
            WriteLog("Info :: " + message);
    }

    public static void WriteDebug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if(LoggingLevel == LogLevel.Debug)
            WriteLog("Debug :: " + message);
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLog("ERROR :: " + message);
        Console.ResetColor();
    }

    private static void WriteLog(string message)
    {
        Console.WriteLine($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ffffzzz")} [{ApplicationName}] {message}");
    }
}