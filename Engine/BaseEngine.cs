using System;
using System.Diagnostics;
using Fishy.Engine.Exceptions;

namespace Fishy.Engine
{
	public abstract class BaseEngine : IEngine
	{
		ProcessStartInfo _engineStartInfo;

		public BaseEngine(ProcessStartInfo engineStartInfo)
		{
			_engineStartInfo = engineStartInfo;
			UpdateEngineStartInfo();
		}

		public void Start ()
		{
			if (this.IsStarted) return;

			this.EngineProcess = new Process();
			this.EngineProcess.StartInfo = _engineStartInfo;

			bool success;

			try {
				success = this.EngineProcess.Start ();

			} catch (System.ComponentModel.Win32Exception ex) {
				throw new EngineCouldNotBeStartedException(_engineStartInfo.FileName, ex);
			}

			if (!success)
				throw new EngineCouldNotBeStartedException(_engineStartInfo.FileName);

			this.IsStarted = true;
		}

		public void Quit ()
		{
			if (!this.IsStarted) return;

			try {
				this.EngineProcess.Kill ();
				this.EngineProcess.StandardError.ReadToEnd();
				this.EngineProcess.WaitForExit ();

			} finally {
				this.EngineProcess = null;
				this.IsStarted = false;			
			}
		}

		internal Process EngineProcess { get; private set; }

		internal bool IsStarted { get; set; }

		internal void UpdateEngineStartInfo ()
		{
			if (string.IsNullOrEmpty (_engineStartInfo.FileName))
				_engineStartInfo.FileName = "stockfish";

			_engineStartInfo.UseShellExecute = false;
			_engineStartInfo.RedirectStandardInput = true;
			_engineStartInfo.RedirectStandardOutput = true;
			_engineStartInfo.RedirectStandardError = true;
		}
	}
}

