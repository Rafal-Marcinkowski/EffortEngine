using Serilog;
using Serilog.Core;

namespace SharedProject;

public static class LogManager
{
    private static Logger logger;

    public static void InitializeLogger()
    {
        logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("C:\\Users\\rafal\\Desktop\\Pogromcy\\EffortEngine\\EffortEngineLog.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger = logger;
    }

    public static void CloseAndFlushLogger()
    {
        Log.CloseAndFlush();
    }

    public static ILogger GetLogger()
    {
        return logger;
    }
}
