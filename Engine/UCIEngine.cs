using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
				EnableUCIMode ();
		}

		public UCISettings UCISettings { get; private set; }
		public int ThinkingDuration { get; set; }
		public int Depth { get; set; }

		private static IUCIEngine CreateStockfish ()
		{
			return new UCIEngine (new ProcessStartInfo ("stockfish")) as IUCIEngine;
		}

		IUCICommander _commander;

		internal UCIEngine (ProcessStartInfo engineStartInfo) : base(engineStartInfo)
		{
			_commander = UCICommander.Create (UCICommanderType.TimeBased, this);
		}

		private void PrepareEngineForNewCommand ()
		{
			if (!this.IsStarted) 
				this.Start ();

			this.ResetOutput ();
		}

		public string GiveBestMove (string fen)
		{
			PrepareEngineForNewCommand ();

			var task = _commander.AnalyseForBestMoveAsync (fen, this.ThinkingDuration);
			task.Wait ();

			return ExtractBestMove (this.Output);
		}

		public static string ExtractBestMove (string engineOutput)
		{
			return Regex.Match (engineOutput, "bestmove ([a-h1-8]{4})").Groups[1].Value;
		}

		public static IScore ExtractScore (string engineOutput)
		{
			var scoreLine = engineOutput
				.Split (Environment.NewLine.ToCharArray ())
					.Last (o => o.Contains ("score"));

			return Score.Create (scoreLine);
		}

		internal void EnableUCIMode ()
		{
			PrepareEngineForNewCommand ();

			var task = _commander.EnableUCIModeAsync ();
			task.Wait ();

			this.UCISettings = new UCISettings(this.Output);
		}

		public IScore GetScore (string fen, string move)
		{
			PrepareEngineForNewCommand ();

			var task = _commander.AnalyseMoveAsync (fen, move, this.ThinkingDuration);
			task.Wait ();

			return ExtractScore (this.Output);
		}
	}
}

