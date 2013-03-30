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

		public override void Start ()
		{
			base.Start ();

			if (this.UCISettings == null)
				PutInUCIMode ();
		}

		public UCISettings UCISettings { get; private set; }

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

		internal void Prepare ()
		{
			if (!this.IsStarted) 
				this.Start ();

			this.ClearOutput ();
			this.EngineProcess.BeginOutputReadLine ();
		}

		internal void SendCommand (string command)
		{
			this.ToEngine.WriteLine ("stop");
			this.ToEngine.WriteLine (command);
			this.EngineProcess.WaitForInputIdle ();
		}

		public string GiveBestMove (string fen, int duration)
		{
			Prepare ();

			var task = GiveBestMoveAsync (fen, duration);
			return task.Result;
		}

		internal async Task<string> GiveBestMoveAsync (string fen, int duration)
		{
			SendCommand ("position fen " + fen);
			SendCommand ("go infinite"); Thread.Sleep (duration * 1000);
			SendCommand ("stop"); await For ("bestmove");

			return ExtractBestMove (this.Output);
		}

		internal void PutInUCIMode ()
		{
			Prepare ();

			var task = PutInUCIModeAsync ();
			this.UCISettings = new UCISettings(task.Result);
		}

		internal async Task<string> PutInUCIModeAsync ()
		{
			SendCommand ("uci");
			await For ("uciok");

			return this.Output;
		}

		public static string ExtractBestMove (string engineOutput)
		{
			return Regex.Match (engineOutput, "bestmove ([a-h1-8]{4})").Groups[1].Value;
		}

		internal Task For (string what)
		{
			return Task.Run ( ()=> {while(!this.Output.Contains (what)); } );
		}
	}
}

