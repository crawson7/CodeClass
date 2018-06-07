using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Console
{
	public static RingBuffer<ConsoleLog> Logs;
	private static Console_Ctrl Viewer;

	public static void Initialize(Console_Ctrl ctrl)
	{
		Viewer = ctrl;
		Application.logMessageReceived += HandleLog;
		Logs = new RingBuffer<ConsoleLog>(100);
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		ConsoleLog log = new ConsoleLog(logString, stackTrace, type);
		Logs.Add(log);
		Viewer.UpdateView();
	}

	public static void UpdateInput(string s)
	{
		Viewer.UpdateInput(s);
	}
}


public class ConsoleLog
{
	public string Message;
	public string Stack;
	public LogType Type;

	public ConsoleLog(string logString, string stackTrace, LogType type)
	{
		Message = logString;
		Stack = stackTrace;
		Type = type;
	}
}