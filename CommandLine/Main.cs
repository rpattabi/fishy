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

/*
 * Manual Tests

# in any of the following tests, stockfish process should not continue to run after showing the result

# show usage
mono fishy.exe
mono fishy.exe --help

# should return best move i.e. d5c7
mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1"

# should return score #4
mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1" -move d5c7

#should return score 20+ approx
mono fishy.exe -fen "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" -move e2e4	

# for all the above with fen, engine should have taken about 20seconds to think

# given -duration option with number_of_seconds, engine should take time accordingly
mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1" -duration 5
mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1" -move d5c7 -duration 3
mono fishy.exe -fen "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" -move e2e4 -duration 4

# depth instead of duration

mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1" -depth 5
mono fishy.exe -fen "k2r4/pp6/8/3NQ3/8/8/3q1PPP/6K1 w - - 0 1" -move d5c7 -depth 20
mono fishy.exe -fen "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" -move e2e4 -depth 8

# mate search

# mate in two
mono fishy.exe -fen "8/8/8/B7/4p3/1Q6/1K2kp2/3R4 w - - 0 1" -mate 2
*/