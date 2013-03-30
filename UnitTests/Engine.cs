using System;
using System.Threading;
using NUnit.Framework;
using Fishy.Engine;
using Fishy.Engine.Exceptions;
using System.Diagnostics;

namespace Fishy.Tests.UnitTests.EngineProcess
{
	[TestFixture]
	public class UCIEngineTests
	{
		[Test]
		public void Stockfish_ShouldNotBeStarted_OnInstanciation ()
		{
			var stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;
			Assert.IsFalse (stockfish.IsStarted);
		}

		[Test]
		public void IncaseEngineCouldNotBeStarted_ExceptionIsExpected ()
		{
			var badEngine = new UCIEngine(new ProcessStartInfo("stuckfish"));
			Assert.Throws<EngineCouldNotBeStartedException>(() => badEngine.Start ());
		}

		[Test]
		public void OnStarting_EngineProcessShouldNotBeNull()
		{
			UCIEngine stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			try {
				stockfish.Start ();

				Assert.IsNotNull (stockfish.EngineProcess);
				Assert.IsTrue (stockfish.IsStarted);
				Assert.IsFalse (stockfish.EngineProcess.HasExited);			

			} finally {
				stockfish.Quit ();
			}
		}

		[Test]
		public void AfterStarting_EngineShouldBeKeptAlive ()
		{
			UCIEngine stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			try {
				stockfish.Start ();

				Assert.IsFalse (stockfish.EngineProcess.HasExited);

				//
				// Not sure what is the right practice here.
				//
				var timer = new Stopwatch();
				timer.Start();

				while (timer.ElapsedMilliseconds <= 5 * 1000)
					Assert.IsFalse (stockfish.EngineProcess.HasExited);

				timer.Stop ();			

			} finally {
				stockfish.Quit ();
			}
		}

		[Test]
		public void AfterStarting_EngineShouldBeSetTo_UCIMode ()
		{
			var stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			try {
				stockfish.Start ();

				Assert.IsNotNull (stockfish.UCISettings);
				StringAssert.StartsWith ("Stockfish", stockfish.UCISettings.Id);

			} finally {
				stockfish.Quit ();
			}
		}

		[Test]
		public void OnQuit_ProcessShouldBeTerminated ()
		{
			var stockfishProcess = UCIEngineInfo.GetInfo (EngineKey.Stockfish).ProcessName;
			var stockfishesBefore = Process.GetProcessesByName (stockfishProcess);

			UCIEngine stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;
			stockfish.Start ();
			stockfish.Quit ();

			var stockfishesAfter = Process.GetProcessesByName (stockfishProcess);

			//CollectionAssert.AreEquivalent (stockfishesBefore, stockfishesAfter);
			Assert.AreEqual (stockfishesBefore.Length, stockfishesAfter.Length);

			int i = 0;
			foreach (var process in stockfishesBefore) {
				Assert.AreEqual (process.Id, stockfishesAfter [i].Id);
				++i;
			}
		}

		[Test]
		public void RepeatedlyStoppingAndStarting_ShouldWork ()
		{
			UCIEngine stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			for (int i = 0; i < 10; ++i) {
				Assert.DoesNotThrow( () => stockfish.Start ());
				Assert.IsFalse (stockfish.EngineProcess.HasExited);

				Assert.DoesNotThrow ( () => stockfish.Quit ());
			}
		}

		[Test]
		public void GiveBestMove_GivenFEN ()
		{
			UCIEngine stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			try {
				int duration = 1;

				// back rank mate
				string fen = "k3r3/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
				string expected = "e5e8";

				string bestMove = stockfish.GiveBestMove (fen, duration);
				Assert.AreEqual (expected, bestMove);

				// smothered mate
				fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
				expected = "d5c7";

				bestMove = stockfish.GiveBestMove (fen, duration);
				Assert.AreEqual (expected, bestMove);			

			} finally {
				stockfish.Quit ();
			}
		}

		[Test]
		public void GivenPositionAndAMove_ReturnScore ()
		{
			var stockfish = UCIEngine.Create (EngineKey.Stockfish) as UCIEngine;

			try {
				// back rank mate
				string fen = "k3r3/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
				string move = "e5e8";

				IScore score = stockfish.GetScore (fen, move);
				Assert.AreEqual (1.0, score.Value);

				// smothered mate
				fen = "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1";
				move = "d5c7";

				score = stockfish.GetScore (fen, move);
				Assert.AreEqual (4.0, score.Value);

				// start position
				fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
				move = "e2e4";

				score = stockfish.GetScore (fen, move);
				Assert.Greater (score.Value, 1);

			} finally {
				stockfish.Quit ();					
			}		
		}
	}
}

