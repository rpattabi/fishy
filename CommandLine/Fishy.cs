using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mono.Options;
using Fishy.Engine;

namespace Fishy.CommandLine
{
	public class Fishy
	{
		string[] _args;
		string _fen;
		bool _showHelp;
		OptionSet _options; 

		IUCIEngine _engine;

		public Fishy (string[] args)
		{
			_args = args;

			_options = new OptionSet() {
				"Usage:",
				"",
				"Options:",
				{ "fen=", "position to analyse in {FEN} notation.",
					v => _fen = v },
				{ "h|help", "show this message and exit",
					v => _showHelp = v != null },
			};

			//List<string> extra;

			try {
				//extra = 
				_options.Parse (_args);
				
			} catch (OptionException e) {
				Console.Write ("fishy: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `fishy --help` for more information.");
				return;
			}
		}

		public string Run ()
		{
			try {
				if (_showHelp) {
					return GetUsage();
				}

				if (!string.IsNullOrEmpty (_fen)) {
					return this.Engine.GiveBestMove (_fen);
				} 
				else {
					return GetUsage();
				}			
			
			} finally {
				if (_engine != null)
					_engine.Quit();
			}
		}

		internal string GetUsage ()
		{
			var stringWriter = new StringWriter();
			_options.WriteOptionDescriptions (stringWriter);
			return stringWriter.ToString ();
		}

		internal IUCIEngine Engine {
			get {
				if (_engine == null) {
					_engine = UCIEngine.Create (EngineKey.Stockfish);
				}

				if (!_engine.IsStarted) {
					_engine.Start ();
				}

				return _engine;
			}
		}
	}
}

