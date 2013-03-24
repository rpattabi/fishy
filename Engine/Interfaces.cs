using System;
using System.Diagnostics;

namespace Fishy.Engine
{
	public interface IEngine
	{
		void Start();
		void Quit();
	}

	public interface IUCIEngine : IEngine
	{
		string GiveBestMove(string fen, long duration = 20);
	}
}

