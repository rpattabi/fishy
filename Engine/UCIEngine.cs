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
		UCIAnalysisType _analysisMode = UCIAnalysisType.TimeBased;
		IUCICommander _commander;

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

		int _thinkingDuration;

		public int ThinkingDuration { 
			get {
				return _thinkingDuration;
			}

			set {
				_thinkingDuration = value;
				_commander.ThinkingDuration = value;
			}
		}

		int _depth;

		public int Depth { 
			get {
				return _depth;
			}

			set {
				_depth = value;
				_commander.Depth = value;
			}
		}

		public UCIAnalysisType AnalysisMode {
			get {
				return _analysisMode;
			}

			set {
				_analysisMode = value;
				_commander = GetCommander (_analysisMode);
			}
		}

		private IUCICommander GetCommander (UCIAnalysisType analysisMode)
		{
			var commander = UCICommander.Create (analysisMode,
			                                     this as IEngineInternals);

			commander.Depth = this.Depth;
			commander.ThinkingDuration = this.ThinkingDuration;

			return commander;
		}

		private static IUCIEngine CreateStockfish ()
		{
			return new UCIEngine (new ProcessStartInfo ("stockfish")) as IUCIEngine;
		}

		internal UCIEngine (ProcessStartInfo engineStartInfo) : base(engineStartInfo)
		{
			this.AnalysisMode = UCIAnalysisType.TimeBased;
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

			var task = _commander.AnalyseForBestMoveAsync (fen);
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

			var task = _commander.AnalyseMoveAsync (fen, move);
			task.Wait ();

			return ExtractScore (this.Output);
		}
	}
}

