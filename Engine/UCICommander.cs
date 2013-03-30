using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fishy.Engine
{
	internal class BaseUCICommander : IUCICommander
	{
		IEngineInternals _engine;

		internal BaseUCICommander (IEngineInternals engine)
		{
			_engine = engine;
		}

		internal void SendCommand (string command)
		{
			_engine.CommandChannel.WriteLine ("stop");
			_engine.CommandChannel.WriteLine (command);

			_engine.EngineProcess.WaitForInputIdle ();
		}

		public async Task PutInUCIModeAsync ()
		{
			SendCommand ("uci");
			await For ("uciok");
		}
		
		public async Task AnalyseForBestMoveAsync (string fen, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite"); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}
		
		public async Task AnalyseMoveAsync (string fen, string move, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite " + "searchmoves " + move); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");
		}

		private Task For (string what)
		{
			return Task.Run ( () => {
				while(!_engine.Output.Contains (what))
					;
			});
		}
	}
}

