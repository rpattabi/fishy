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
		UCIAnalysisType AnalysisMode { get; set; }

		string GiveBestMove(string fen);
		IScore GetScore(string fen, string move);
	}

	internal interface IUCICommander
	{
		Task EnableUCIModeAsync();

		int Depth { set; }
		int ThinkingDuration { set; }

		Task AnalyseForBestMoveAsync(string fen);
		Task AnalyseMoveAsync(string fen, string move);
	}

	public interface IScore
	{
		double Value { get; }
	}
}
