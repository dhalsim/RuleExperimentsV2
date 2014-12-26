using Application.RuleExperiments.Loggers;
using Domain.RuleExperiments.Models.Log;
using NUnit.Framework;

namespace UnitTests.RuleExperiments.LoggerTests
{
	[TestFixture]
	public class StaticLoggerFixture
	{
		[Test]
		public void Should_log()
		{
			StaticLogger logger = new StaticLogger();
			Log log = new Log(LogLevel.Debug, "log example");
			logger.Log(log);

			var logged = logger.GetLogs()[0];
            
			Assert.AreSame(log, logged);
			Assert.AreEqual(LogLevel.Debug, logged.Level);
		}
	}
}