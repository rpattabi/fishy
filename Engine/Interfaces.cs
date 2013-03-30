using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fishy.Engine
{
	internal interface IEngineInternals
	{
		Process EngineProcess { get; }
		StreamWriter CommandChannel { get; }
		string Output { get; }
	}

	public interface IEngine
	{
		void Start();
		void Quit();
		bool IsStarted { get; }
	}

	public interface IUCIEngine : IEngine
	{
		UCISettings UCISettings { get; }
		int ThinkingDuration { get; set; }
		int Depth { get; set; }

		string GiveBestMove(string fen);
		IScore GetScore(string fen, string move);
	}

	internal interface IUCICommander
	{
		Task EnableUCIModeAsync();

		Task AnalyseForBestMoveAsync(string fen, int duration);
		Task AnalyseMoveAsync(string fen, string move, int duration);
	}

	public interface IScore
	{
		double Value { get; }
	}
}
