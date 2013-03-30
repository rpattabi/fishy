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
		UCISettings UCISettings { get; }
		int ThinkingDuration { get; set; }
		int Depth { get; set; }

		string GiveBestMove(string fen);
		IScore GetScore(string fen, string move);
	}

	public interface IScore
	{
		double Value { get; }
	}
}

