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
		public void ReturnUsage_WhenHelpIsRequested()
		{
			string[] args = {"--help"};
			var fishy = new Fishy.CommandLine.Fishy(args);

			string feedback = fishy.Run();
			string[] feedbackLines = feedback.Split(Environment.NewLine.ToCharArray());

			Assert.IsNotEmpty(feedbackLines);
			Assert.AreEqual(true, feedbackLines[0].Contains("Usage:"));
		}

		[Test]
		public void ReturnBestMove_GivenFEN ()
		{
			// start pos
			string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			string[] args = {"-fen", fen};
			string expected = "e2e4";

			var fishy = new Fishy.CommandLine.Fishy(args);
			string bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// back rank mate
			fen = "k3r3/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			args[1] = fen;
			expected = "e5e8";

			fishy = new Fishy.CommandLine.Fishy(args);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// smothered mate
			fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			args[1] = fen;
			expected = "d5c7";

			fishy = new Fishy.CommandLine.Fishy(args);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);
		}
	}
}
