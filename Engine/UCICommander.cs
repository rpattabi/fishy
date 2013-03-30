using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fishy.Engine
{
	internal enum UCICommanderType
	{
		TimeBased,
		ResultBased
	}

	internal abstract class UCICommander : IUCICommander
	{
		public static IUCICommander Create (UCICommanderType type, IEngineInternals engine)
		{
			switch (type) {

			case UCICommanderType.TimeBased:
				return new TimeBasedCommander (engine);
			case UCICommanderType.ResultBased:
				return new ResultBasedCommander (engine);

			default:
				throw new InvalidProgramException ();
			}
		}

		protected IEngineInternals _engine;

		internal UCICommander (IEngineInternals engine)
		{
			_engine = engine;
		}

		public async Task EnableUCIModeAsync ()
		{
			SendCommand ("uci");
			await For ("uciok");
		}

		protected void SendCommand (string command)
		{
			_engine.CommandChannel.WriteLine ("stop");
			_engine.CommandChannel.WriteLine (command);

			_engine.EngineProcess.WaitForInputIdle ();
		}

		protected Task For (string what)
		{
			return Task.Run ( () => {
				while(!_engine.Output.Contains (what))
					;
			});
		}

		public abstract Task AnalyseForBestMoveAsync (string fen, int duration);

		public abstract Task AnalyseMoveAsync (string fen, string move, int duration);
	}

	internal class TimeBasedCommander : UCICommander
	{
		public TimeBasedCommander (IEngineInternals engine) : base(engine)
		{
		}

		public override async Task AnalyseForBestMoveAsync (string fen, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite"); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}
		
		public override async Task AnalyseMoveAsync (string fen, string move, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite " + "searchmoves " + move); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}
	}

	internal class ResultBasedCommander : UCICommander
	{
		public ResultBasedCommander (IEngineInternals engine) : base(engine)
		{
		}

		public override async Task AnalyseForBestMoveAsync (string fen, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite"); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}
		
		public override async Task AnalyseMoveAsync (string fen, string move, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite " + "searchmoves " + move); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}
	}
}

