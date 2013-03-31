using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fishy.Engine
{
	public enum UCIAnalysisType
	{
		TimeBased, 		// Time boxed
		ResultBased		// Depth	
	}

	internal abstract class UCICommander : IUCICommander
	{
		public static IUCICommander Create (UCIAnalysisType type, IEngineInternals engine)
		{
			switch (type) {

			case UCIAnalysisType.TimeBased:
				return new TimeBasedCommander (engine);

			case UCIAnalysisType.ResultBased:
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
			SendCommand ("uci"); await For ("uciok");

			SendCommand ("setoption name Use Search Log value true");
			SendCommand ("setoption name Search Log Filename value /tmp/stockfish.log");
			SendCommand ("isready"); await For ("readyok");
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

		public abstract int Depth { set; }
		public abstract int ThinkingDuration { set; }

		public abstract Task AnalyseForBestMoveAsync (string fen);
		public abstract Task AnalyseMoveAsync (string fen, string move);
	}

	internal class TimeBasedCommander : UCICommander
	{
		int _thinkingTime;


		public TimeBasedCommander (IEngineInternals engine) : base(engine)
		{
			_thinkingTime = 20;
		}

		public override int Depth {
			set {
				// Not concerned about Depth
			}
		}

		public override int ThinkingDuration {
			set {
				_thinkingTime = value;
			}
		}

		public override async Task AnalyseForBestMoveAsync (string fen)
		{
			SendCommand ("position fen " + fen);

			SendCommand ("go movetime " + 
			             _thinkingTime * 1000); await For ("bestmove");
		}
		
		public override async Task AnalyseMoveAsync (string fen, string move)
		{
			SendCommand ("position fen " + fen);

			SendCommand ("go movetime " + 
			             _thinkingTime * 1000 + 
			             " searchmoves " + move); await For ("bestmove");
		}
	}

	internal class ResultBasedCommander : UCICommander
	{
		int _depth;


		public ResultBasedCommander (IEngineInternals engine) : base(engine)
		{
			_depth = 20;
		}
		
		public override int Depth {
			set {
				_depth = value;
			}
		}

		public override int ThinkingDuration {
			set {
				// Not concerned about Thinking Duration
			}
		}

		public override async Task AnalyseForBestMoveAsync (string fen)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go depth " + _depth); await For ("bestmove");
		}
		
		public override async Task AnalyseMoveAsync (string fen, string move)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go depth " + _depth + " searchmoves " + move); await For ("bestmove");
		}
	}
}

