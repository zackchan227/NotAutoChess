#if UNITY_EDITOR
#define DEBUG
#define ASSERT
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using System.Text;

public static class Utils
{

    //-----------------------------------
		//--------------------- Log , warning, 

		[Conditional("DEBUG")]
		public static void Log(object message)
		{
			Debug.Log(message);
		}

		[Conditional("DEBUG")]
		public static void Log(string format, params object[] args)
		{
			Debug.Log(string.Format(format, args));
		}

		[Conditional("DEBUG")]
		public static void LogWarning(object message, UnityEngine.Object context)
		{
			Debug.LogWarning(message, context);

		}

		[Conditional("DEBUG")]
		public static void LogWarning(UnityEngine.Object context, string format, params object[] args)
		{
			Debug.LogWarning(string.Format(format, args), context);
		}



		[Conditional("DEBUG")]
		public static void Warning(bool condition, object message)
		{
			if ( ! condition) Debug.LogWarning(message);
		}

		[Conditional("DEBUG")]
		public static void Warning(bool condition, object message, UnityEngine.Object context)
		{
			if ( ! condition) Debug.LogWarning(message, context);
		}

		[Conditional("DEBUG")]
		public static void Warning(bool condition, UnityEngine.Object context, string format, params object[] args)
		{
			if ( ! condition) Debug.LogWarning(string.Format(format, args), context);
		}


		//---------------------------------------------
		//------------- Assert ------------------------

		/// Thown an exception if condition = false
		[Conditional("ASSERT")]
		public static void Assert(bool condition)
		{
			if (! condition) throw new UnityException();
		}

		/// Thown an exception if condition = false, show message on console's log
		[Conditional("ASSERT")]
		public static void Assert(bool condition, string message)
		{
			if (! condition) throw new UnityException(message);
		}

		/// Thown an exception if condition = false, show message on console's log
		[Conditional("ASSERT")]
		public static void Assert(bool condition, string format, params object[] args)
		{
			if (! condition) throw new UnityException(string.Format(format, args));
		}

	/// <summary>
	/// Format Date
	/// </summary>
    public static string formatDateTime(float time)
    {
        string second = Math.Round(time,0).ToString();
        string minute = (Math.Round(time,0) / 60).ToString();
        string hour = (Math.Round(time,0) / 60 / 60).ToString();
        return hour + ":" + minute + ":" + second;
    }

	/// <summary>
	/// Format time Hours:Minutes:Seconds (00:00:00)
	/// </summary>
    public static string formatTimer(double seconds)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:00}:{1:00}:{2:00}",
            (int)span.TotalHours, span.Minutes, span.Seconds);
    }

	/// <summary>
	/// Check string length with min, max param
	/// </summary>
	public static bool checkLengthString(byte min, byte max, string str)
	{
		if(str.Length < min || str.Length > max) return false;
		return true;
	}

	/// <summary>
	/// Replace a string char at index with another char
	/// </summary>
	/// <param name="text">string to be replaced</param>
	/// <param name="index">position of the char to be replaced</param>
	/// <param name="c">replacement char</param>
	public static string ReplaceAtIndex(string text, int index, char c)
	{
		var stringBuilder = new StringBuilder(text);
		stringBuilder[index] = c;
		return stringBuilder.ToString();
	}
}
