using System;
using NUnit.Framework;

namespace Fishy.Tests.UnitTests
{
	[TestFixture]
	public class CommandLineTests
	{
		[Test]
		public void ReturnUsage_WhenNoArguments()
		{
			string[] args = {};
			var fishy = new Fishy.CommandLine.Fishy(args);

			string feedback = fishy.Run();
			string[] feedbackLines = feedback.Split(Environment.NewLine.ToCharArray());

			Assert.IsNotEmpty(feedbackLines);
			Assert.AreEqual(true, feedbackLines[0].Contains("Usage:"));
		}
	}
}
