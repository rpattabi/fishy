using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mono.Options;
using System.Threading.Tasks;
using Fishy.Engine;

namespace Fishy.CommandLine
{
	public class FishyArgs
	{
		string[] _args;
		OptionSet _options;

 		const int DefaultThinkingTime = 20;

		public bool ShowHelp { get; internal set; }
		public string Position { get; internal set; }
		public string Move { get; internal set; }

		public int ThinkingDuration { get; internal set; }
		public int Depth { get; internal set; }

		public bool IsDepthMode {
			get {
				return this.Depth > 0;
			}
		}

		public bool NeedScore {
			get {
				if (this.ShowHelp) 
					return false;

				return (!string.IsNullOrEmpty (this.Position)
				        && !string.IsNullOrEmpty (this.Move));
			}
		}

		public bool NeedBestMove {
			get {
				if (this.ShowHelp || this.NeedScore)
					return false;

				return !string.IsNullOrEmpty (this.Position);
			}
		}

		public FishyArgs (string[] args)
		{
			_args = args;
			this.ThinkingDuration = DefaultThinkingTime;

			_options = new OptionSet () {
				"Usage:",
				"",
				"Options:",
				{ "fen=", "position to analyse in {FEN} notation.",
					v => this.Position = v },
				{ "move=", "move to analyse for the given position.",
					v => this.Move = v },
				{ "depth=", "{DEPTH} of the search. If mentioned duration option is ignored.",
					(int v) => this.Depth = v },
				{ "duration=", "{DURATION} in seconds to analyze per position. If not given, defaults to 20 seconds.", 
					(int v) => this.ThinkingDuration = v },
				{ "h|?|help", "show this message and exit",
					v => this.ShowHelp = v != null },
			};

			try {
				_options.Parse (_args);

				if (string.IsNullOrEmpty (this.Position))
					this.ShowHelp = true;
				
			} catch (OptionException e) {
				throw new OptionException("fishy: " + Environment.NewLine +
				                          e.Message + Environment.NewLine +
				                          "Try `fishy --help` for more information.", e.OptionName);
			}
		}

		public string GetUsage ()
		{
			var stringWriter = new StringWriter();
			_options.WriteOptionDescriptions (stringWriter);
			return stringWriter.ToString ();
		}	
	}

	public class Fishy
	{
		FishyArgs _args;
		IUCIEngine _engine;

		public Fishy (FishyArgs args, IUCIEngine uciEngine)
		{
			_args = args;

			_engine = uciEngine;
			_engine.ThinkingDuration = _args.ThinkingDuration;
			_engine.Depth = _args.Depth;
		}

		public string Run ()
		{
			try {
				if (_args.ShowHelp)
					return _args.GetUsage();

				if (_args.NeedBestMove)
					return GetBestMove(_args.Position);

				if (_args.NeedScore)
					return GetScore(_args.Position, _args.Move);

				return _args.GetUsage ();
			
			} finally {
				if (_engine != null)
					_engine.Quit();
			}
		}

		internal string GetBestMove (string fen)
		{
			return this.Engine.GiveBestMove (fen);
		}

		internal string GetScore (string fen, string move)
		{
			return this.Engine.GetScore (fen, move).ToString ();
		}

		internal IUCIEngine Engine {
			get {
				if (!_engine.IsStarted) {
					_engine.Start ();
				}

				return _engine;
			}
		}
	}
}
