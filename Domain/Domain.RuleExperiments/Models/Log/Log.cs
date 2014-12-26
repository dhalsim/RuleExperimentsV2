using System;

namespace Domain.RuleExperiments.Models.Log
{
	public class Log
	{
		private DateTime Time { get; set; }

		public LogLevel Level { get; private set; }

		private string Message { get; set; }

		public Log (string message)
		{
			Message = message;
			Level = LogLevel.Error;
			Time = DateTime.Now;
		}

		public Log (LogLevel level, string message)
		{
			Message = message;
			Level = level;
			Time = DateTime.Now;
		}
	}
}