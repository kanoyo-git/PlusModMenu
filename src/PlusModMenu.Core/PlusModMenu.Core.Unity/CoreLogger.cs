using System.Diagnostics;
using UnityEngine;

namespace PlusModMenu.Core.Unity;

public class CoreLogger
{
	public static void Log(object msg, LogType logType = LogType.Log)
	{
		string text = logType switch
		{
			LogType.Warning => "WARNING!!! ", 
			LogType.Error => "ERROR!!! ", 
			LogType.Exception => "FATAL!!! ", 
			_ => "", 
		};
		string text2 = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Namespace;
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "Unnamed Namespace";
		}
		if (msg is string)
		{
			UnityEngine.Debug.Log($"<b>[{text2}]</b> {text}{msg}");
			return;
		}
		UnityEngine.Debug.Log("<b>[" + text2 + "]</b> " + text);
		UnityEngine.Debug.Log(msg);
	}
}
