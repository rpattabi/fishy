using System;
using System.Diagnostics;

namespace Fishy.Engine
{
	public class UCIEngine : BaseEngine, IUCIEngine
	{
		public UCIEngine (ProcessStartInfo engineStartInfo) : base(engineStartInfo)
		{
		}

		public string GiveBestMove (string fen, long duration = 20)
		{
			return "e2e4";
		}
	}
}

