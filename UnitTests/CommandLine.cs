using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Fishy.Engine;
using Fishy.CommandLine;

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
			var fishyArgs = new FishyArgs(args);
			var fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);

			string feedback = fishy.Run();
			string[] feedbackLines = feedback.Split(Environment.NewLine.ToCharArray());

			Assert.IsNotEmpty(feedbackLines);
			Assert.AreEqual(true, feedbackLines[0].Contains("Usage:"));
		}

		[Test]
		public void ReturnUsage_WhenHelpIsRequested()
		{
			string[] args = {"--help"};
			var fishyArgs = new FishyArgs(args);
			var fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);

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
			var fishyArgs = new FishyArgs(args);
			string expected = "e2e4";

			_uciMock.Setup (e => e
			    .GiveBestMove(It.IsAny<string>()))
				.Returns (() => expected);

			var fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			string bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// back rank mate
			fen = "k3r3/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			fishyArgs.Position = fen;
			expected = "e5e8";

			fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);

			// smothered mate
			fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			fishyArgs.Position = fen;
			expected = "d5c7";

			fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			bestMove = fishy.Run ();
			Assert.AreEqual (expected, bestMove);
		}

		[Test]
		public void DurationDefaultsTo_20Seconds ()
		{
			string fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			string[] args = {"-fen", fen};
			var fishyArgs = new FishyArgs(args);

			var fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			fishy.Run ();

			_uciMock.VerifySet (e => e.ThinkingDuration = 20);
		}

		[Test]
		public void UseDuration_IfSpecified ()
		{
			int duration = 5;
			string fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			string[] args = {"-fen", fen, "-duration", duration.ToString ()};
			var fishyArgs = new FishyArgs(args);

			var fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			fishy.Run ();

			_uciMock.VerifySet (e => e.ThinkingDuration = duration);

			duration = 25;
			fishyArgs.ThinkingDuration = duration;

			fishy = new Fishy.CommandLine.Fishy(fishyArgs, _engine);
			fishy.Run ();

			_uciMock.VerifySet (e => e.ThinkingDuration = duration);
		}

		[Test]
		public void ReturnScore_GivePositionAndAMove ()
		{
			string fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
			string move= "e5e8";

			string[] args = {"-fen", fen, "-move", move};

			// centipawns score
			IScore score = Score.Create ("info depth 19 seldepth 26 score cp 44 nodes 10051581 nps 1911310 time 5259");

			_uciMock.Setup (e => e
                .GetScore (It.IsAny<string> (), It.IsAny<string> ()))
				.Returns (() => score);

			var fishyArgs = new FishyArgs(args);
			var fishy = new Fishy.CommandLine.Fishy (fishyArgs, _engine);
			string output = fishy.Run ();

			Assert.AreEqual("44", output);

			// mate score
			score = Score.Create ("info depth 19 seldepth 26 score mate 4 nodes 10051581 nps 1911310 time 5259");

			fishy = new Fishy.CommandLine.Fishy (fishyArgs, _engine);
			output = fishy.Run ();

			Assert.AreEqual("#4", output);
		}
	}
}
