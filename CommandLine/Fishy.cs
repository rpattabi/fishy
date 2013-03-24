using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace Fishy.CommandLine
{
	public class Fishy
	{
		string[] _args;
		string _fen;
		bool _showHelp;

		OptionSet _options; 

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
			if (_showHelp) {
				return GetUsage();
			}

			if (!string.IsNullOrEmpty (_fen)) {
				return "e2e4";
			} 
			else {
				return GetUsage();
			}
		}

		internal string GetUsage ()
		{
			var stringWriter = new StringWriter();
			_options.WriteOptionDescriptions (stringWriter);
			return stringWriter.ToString ();
		}
	}
}

