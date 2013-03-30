using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mono.Options;
using System.Threading.Tasks;
using Fishy.Engine;

namespace Fishy.CommandLine
{
	public class Fishy
	{
		string[] _args;

		string _fen;
		int _duration;
		string _move;

 		const int DefaultDuration = 20;

		bool _showHelp;
		OptionSet _options; 

		IUCIEngine _engine;

		public Fishy (string[] args, IUCIEngine uciEngine)
		{
			_args = args;
			_engine = uciEngine;
			_duration = DefaultDuration;

			_options = new OptionSet() {
				"Usage:",
				"",
				"Options:",
				{ "fen=", "position to analyse in {FEN} notation.",
					v => _fen = v },
				{ "move=", "move to analyse for the given position.",
					v => _move = v },
				{ "d|duration=", "{DURATION} in seconds to analyze per position. If not given, defaults to 20 seconds.", 
					(int v) => _duration = v },
				{ "h|?|help", "show this message and exit",
					v => _showHelp = v != null },
			};

			try {
				_options.Parse (_args);

				if (string.IsNullOrEmpty (_fen))
					_showHelp = true;
				
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

				if (!string.IsNullOrEmpty (_fen) && !string.IsNullOrEmpty (_move))
					return GetScore(_fen, _move);
				else
					return GetBestMove(_fen, _duration);
			
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

		internal string GetBestMove (string fen, int duration)
		{
			return this.Engine.GiveBestMove (_fen, _duration);
		}

		internal string GetScore (string fen, string move)
		{
			return this.Engine.GetScore (_fen, _move).ToString ();
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
