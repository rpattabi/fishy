using System;
using Fishy.Engine;

namespace Fishy.CommandLine
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				IUCIEngine engine = UCIEngine.Create (EngineKey.Stockfish);
				var fishy = new Fishy(args, engine);

				var feedback = fishy.Run();
				Console.WriteLine(feedback);
			}
			catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
