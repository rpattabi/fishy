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

		[Test]
		public void ReturnBestMove_GivenFEN ()
		{
			string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

			string[] args = {"-fen", fen};

			var fishy = new Fishy.CommandLine.Fishy(args);
			string bestMove = fishy.Run ();

			Assert.AreEqual ("e2e4", bestMove);
		}
	}
}
