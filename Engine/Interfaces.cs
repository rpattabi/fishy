using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fishy.Engine
{
	public interface IEngine
	{
		void Start();
		void Quit();
		bool IsStarted { get; }

		string Output { get; }
		void ClearOutput();
	}

	public interface IUCIEngine : IEngine
	{
		Task<string> GiveBestMove(string fen, int duration = 20);
	}
}

