using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using LogLevel = NLog.LogLevel;

namespace AccessControlAdapterSample
{
	public static class Logging
	{
		static Logging()
		{
			Logger = LogManager.GetLogger("Default");
		}



		public static NLog.Logger Logger { get; private set; }



		public static void ConfigureLogger(LogLevel logLevel, string logPath)
		{
			var config = new LoggingConfiguration();

			var layout =
				"${longdate}\t${processid}\t${threadid}\t${level:uppercase=true}\t${stack}\t${scopeproperty:item=ResourceId}\t${scopenested:bottomFrames=-1:topFrames=-1:separator=|}\t${message}${onexception:$\r\n\t${exception:format=ToString,Data:maxInnerExceptionLevel=10:separator=\r\n\t:exceptionDataSeparator=\r\n\t}${stacktrace:format=DetailedFlat:topFrames=16:skipFrames=0:separator=\r\n\t}}";

			var logfile = new FileTarget("logfile")
			{
				FileName = "${var:LogPath}",
				Layout = layout,
				MaxArchiveFiles = 10,
				EnableFileDelete = true,
				ArchiveAboveSize = 16 * 1024 * 1024,
				ArchiveNumbering = ArchiveNumberingMode.Rolling,
				KeepFileOpen = false
			};

			var logfileAtw = new AsyncTargetWrapper(logfile)
			{
				Name = "logfileAtw",
				OverflowAction = AsyncTargetWrapperOverflowAction.Grow
			};

			config.AddTarget(logfileAtw);

			config.LoggingRules.Add(new LoggingRule("*", logLevel, logfileAtw));

#if DEBUG
			var logDebugger = new DebuggerTarget("debugger")
			{
				Layout = layout
			};

			config.AddTarget(logDebugger);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, logDebugger));
#endif

			config.Variables["LogPath"] = logPath;

			// Apply config
			LogManager.Configuration = config;
			LogManager.ReconfigExistingLoggers();
		}
	}
}