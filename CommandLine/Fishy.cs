using System;
using Mono.Options;

namespace Fishy.CommandLine
{
	public class Fishy
	{
		string[] _args;

		public Fishy (string[] args)
		{
			_args = args;
		}

		public string Run ()
		{
			if (_args.Length == 0)
				return "Usage:";
			else
				return string.Empty;
		}
	}
}

