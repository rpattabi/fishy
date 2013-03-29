using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Fishy.Engine
{
	public enum EngineKey
	{
		Stockfish
	}

	internal class UCIEngineInfo
	{
		private static Dictionary<EngineKey, UCIEngineInfo> enginesManual = new Dictionary<EngineKey, UCIEngineInfo> {
			{ EngineKey.Stockfish, new UCIEngineInfo ("stockfish") }
		};

		public static UCIEngineInfo GetInfo (EngineKey key)
		{
			return enginesManual [key];
		}

		internal UCIEngineInfo (string processName)
		{
			this.ProcessName = processName;
		}

		public string ProcessName { get; private set; }
	}

	public class UCIEngine : BaseEngine, IUCIEngine
	{	
		public static IUCIEngine Create (EngineKey engineKey)
		{
			switch (engineKey) {
				case EngineKey.Stockfish:
					return CreateStockfish ();

				default:
					return CreateStockfish ();
			}
		}

		private static IUCIEngine CreateStockfish ()
		{
			return new UCIEngine (new ProcessStartInfo ("stockfish")) as IUCIEngine;
		}

		internal UCIEngine (ProcessStartInfo engineStartInfo) : base(engineStartInfo)
		{
		}

		internal StreamWriter ToEngine {
			get {
				return this.EngineProcess.StandardInput;
			}
		}

		internal StreamReader FromEngine {
			get {
				return this.EngineProcess.StandardOutput;
			}
		}

		internal void SendCommand (string command)
		{
			this.ToEngine.WriteLine ("stop");
			this.ToEngine.WriteLine (command);
			this.EngineProcess.WaitForInputIdle ();
		}

		public async Task<string> GiveBestMove (string fen, int duration = 5)
		{
			try {
			
				this.ClearOutput();
				this.EngineProcess.BeginOutputReadLine ();
					
				SendCommand ("position fen " + fen);
				SendCommand ("go infinite"); Thread.Sleep (duration * 1000);
				SendCommand ("stop"); await Task.Run ( () => { while(!this.Output.Contains ("bestmove")); });

				return ExtractBestMove (this.Output);

			} finally {
				SendCommand ("quit");
			}
		}

		public static string ExtractBestMove (string engineOutput)
		{
			return Regex.Match (engineOutput, "bestmove ([a-h1-8]{4})").Groups[1].Value;
		}
	}
}

