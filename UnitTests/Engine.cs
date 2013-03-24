using System;
using NUnit.Framework;
using Fishy.Engine;
using Fishy.Engine.Exceptions;
using System.Diagnostics;

namespace Fishy.Tests.UnitTests.Engine
{
	[TestFixture]
	public class UCIEngineTests
	{
		string _engineProcessName;
		ProcessStartInfo _stockfishStartInfo;
		UCIEngine _stockfish;


		[SetUp]
		public void Setup ()
		{
			_engineProcessName = "stockfish";
			_stockfishStartInfo = new ProcessStartInfo(_engineProcessName);
			_stockfish = new UCIEngine(_stockfishStartInfo);
		}

		[TearDown]
		public void TearDown ()
		{
			_stockfish.Quit ();
		}

		[Test]
		public void Stockfish_ShouldNotBeStarted_OnInstanciation ()
		{
			var stockfish = new UCIEngine(_stockfishStartInfo);
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
			_stockfish.Start ();

			Assert.IsNotNull (_stockfish.Engine);
			Assert.IsTrue (_stockfish.IsStarted);
			Assert.IsFalse (_stockfish.Engine.HasExited);
		}

		[Test]
		public void AfterStarting_EngineShouldBeKeptAlive ()
		{
			_stockfish.Start ();

			Assert.IsFalse (_stockfish.Engine.HasExited);

			//
			// Not sure what is the right practice here.
			//
			var timer = new Stopwatch();
			timer.Start();

			while (timer.ElapsedMilliseconds <= 5 * 1000)
				Assert.IsFalse (_stockfish.Engine.HasExited);

			timer.Stop ();
		}

		[Test]
		public void OnQuit_ProcessShouldBeTerminated ()
		{
			var stockfishesBefore = Process.GetProcessesByName (_engineProcessName);

			_stockfish.Start ();
			_stockfish.Quit();

			var stockfishesAfter = Process.GetProcessesByName (_engineProcessName);

			Assert.AreEqual (stockfishesBefore, stockfishesAfter);
		}

		[Test]
		public void RepeatedlyStoppingAndStarting_ShouldWork ()
		{
			for (int i = 0; i < 10; ++i) {
				Assert.DoesNotThrow( () => _stockfish.Start ());
				Assert.IsFalse (_stockfish.Engine.HasExited);

				Assert.DoesNotThrow ( () => _stockfish.Quit ());
			}
		}
	}
}

