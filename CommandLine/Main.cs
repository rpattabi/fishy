using System;

namespace Fishy.CommandLine
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				var fishy = new Fishy(args);
				var feedback = fishy.Run();

				Console.WriteLine(feedback);
			}
			catch(Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
