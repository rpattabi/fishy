using System;
using Fishy.Engine;

namespace Fishy.CommandLine
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				var inputs = new FishyArgs(args);
				IUCIEngine engine = UCIEngine.Create (EngineKey.Stockfish);

				var fishy = new Fishy(inputs, engine);

				var feedback = fishy.Run();
				Console.WriteLine(feedback);
			}
			catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
