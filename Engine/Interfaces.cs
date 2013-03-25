using System;
using System.Diagnostics;

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
		string GiveBestMove(string fen, long duration = 20);
	}
}

