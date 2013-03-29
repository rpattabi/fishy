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
		UCIEngine _stockfish;


		[SetUp]
		public void Setup ()
		{
			_stockfish = UCIEngine.Create(EngineKey.Stockfish) as UCIEngine;
		}

		[TearDown]
		public void TearDown ()
		{
			_stockfish.Quit ();
		}

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
			_stockfish.Start ();

			Assert.IsNotNull (_stockfish.EngineProcess);
			Assert.IsTrue (_stockfish.IsStarted);
			Assert.IsFalse (_stockfish.EngineProcess.HasExited);
		}

		[Test]
		public void AfterStarting_EngineShouldBeKeptAlive ()
		{
			_stockfish.Start ();

			Assert.IsFalse (_stockfish.EngineProcess.HasExited);

			Thread.Sleep (2 * 1000);

			//
			// Not sure what is the right practice here.
			//
			var timer = new Stopwatch();
			timer.Start();

			while (timer.ElapsedMilliseconds <= 5 * 1000)
				Assert.IsFalse (_stockfish.EngineProcess.HasExited);

			timer.Stop ();
		}

		[Test]
		public void OnQuit_ProcessShouldBeTerminated ()
		{
			var stockfishProcess = UCIEngineInfo.GetInfo (EngineKey.Stockfish).ProcessName;
			var stockfishesBefore = Process.GetProcessesByName (stockfishProcess);

			_stockfish.Start ();
			_stockfish.Quit ();

			var stockfishesAfter = Process.GetProcessesByName (stockfishProcess);

			//CollectionAssert.AreEquivalent (stockfishesBefore, stockfishesAfter);
			Assert.AreEqual (stockfishesBefore.Length, stockfishesAfter.Length);

			int i = 0;
			foreach (var stockfish in stockfishesBefore) {
				Assert.AreEqual (stockfish.Id, stockfishesAfter [i].Id);
				++i;
			}
		}

		[Test]
		public void RepeatedlyStoppingAndStarting_ShouldWork ()
		{
			for (int i = 0; i < 10; ++i) {
				Assert.DoesNotThrow( () => _stockfish.Start ());
				Assert.IsFalse (_stockfish.EngineProcess.HasExited);

				Assert.DoesNotThrow ( () => _stockfish.Quit ());
			}
		}
	}
}

