using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Fishy.Engine;

namespace Fishy.Tests.UnitTests
{
	[TestFixture]
	public class CommandLineTests
	{
		Mock<IUCIEngine> _uciMock;
		IUCIEngine _engine;


		[SetUp]
		public void Setup ()
		{
			_uciMock = new Mock<IUCIEngine>();
			_engine = _uciMock.Object;
		}

		[Test]
		public void ReturnUsage_WhenNoArguments()
		{
			string[] args = {};
			var fishy = new Fishy.CommandLine.Fishy(args, _engine);

			string feedback = fishy.Run();
			string[] feedbackLines = feedback.Split(Environment.NewLine.ToCharArray());

			Assert.IsNotEmpty(feedbackLines);
			Assert.AreEqual(true, feedbackLines[0].Contains("Usage:"));
		}

		[Test]
		public void ReturnUsage_WhenHelpIsRequested()
		{
			string[] args = {"--help"};
			var fishy = new Fishy.CommandLine.Fishy(args, _engine);

			string feedback = fishy.Run();
			string[] feedbackLines = feedback.Split(Environment.NewLine.ToCharArray());

			Assert.IsNotEmpty(feedbackLines);
			Assert.AreEqual(true, feedbackLines[0].Contains("Usage:"));
		}

		/*
		[Test]
		public void ReturnBestMove_GivenFEN2 ()
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
		*/

		[Test]
		public void ReturnBestMove_GivenFEN ()
		{
			// start pos
			string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			string[] args = {"-fen", fen};
			string expected = "e2e4";

			_uciMock.Setup (e => e
			    .GiveBestMove(It.IsAny<string>(), It.IsAny<int>()))
				.Returns (() => expected);

			var fishy = new Fishy.CommandLine.Fishy(args, _engine);
			string bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// back rank mate
			fen = "k3r3/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			args[1] = fen;
			expected = "e5e8";

			fishy = new Fishy.CommandLine.Fishy(args, _engine);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// smothered mate
			fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			args[1] = fen;
			expected = "d5c7";

			fishy = new Fishy.CommandLine.Fishy(args, _engine);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);
		}
	}
}
