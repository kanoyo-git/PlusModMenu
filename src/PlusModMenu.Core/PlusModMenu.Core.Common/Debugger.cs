using System;

namespace PlusModMenu.Core.Common;

public static class Debugger
{
	public static event Action<object>? LogEvent;

	public static event Action<object>? WarningEvent;

	public static event Action<object>? ErrorEvent;

	public static void Log(object message)
	{
		LogEvent?.Invoke(message);
	}

	public static void Warning(object message)
	{
		WarningEvent?.Invoke(message);
	}

	public static void Error(object message)
	{
		ErrorEvent?.Invoke(message);
	}
}
