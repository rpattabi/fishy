using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Fishy.Engine.Exceptions;

namespace Fishy.Engine
{
	public abstract class BaseEngine : IEngine, IEngineInternals
	{
		ProcessStartInfo _engineStartInfo;

		object _syncRoot = new object();
		StringBuilder _engineOutput = new StringBuilder();//TODO: Fixed Capacity?

		public BaseEngine(ProcessStartInfo engineStartInfo)
		{
			_engineStartInfo = engineStartInfo;
			UpdateEngineStartInfo();
		}

		public virtual void Start ()
		{
			if (this.IsStarted) return;

			this.EngineProcess = new Process();
			this.EngineProcess.StartInfo = _engineStartInfo;

			bool success;

			try {
				success = this.EngineProcess.Start ();
				this.EngineProcess.OutputDataReceived += OutputReceived;

				if (success) this.EngineProcess.WaitForInputIdle ();

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
				if (!this.EngineProcess.HasExited) {
					this.EngineProcess.Kill ();

					this.EngineProcess.BeginOutputReadLine ();
					this.EngineProcess.StandardError.ReadToEnd();
					this.EngineProcess.WaitForExit ();

				} else {
					this.EngineProcess.Dispose();
				}

			} finally {
				this.EngineProcess = null;
				this.IsStarted = false;			
			}
		}

		public bool IsStarted { get; set; }

		public Process EngineProcess { get; private set; }

		public StreamWriter CommandChannel {
			get {
				return this.EngineProcess.StandardInput;
			}
		}

		protected void ResetOutput ()
		{
			lock (_syncRoot) {
				_engineOutput.Clear ();
			}

			this.EngineProcess.BeginOutputReadLine ();
		}

		internal void UpdateEngineStartInfo ()
		{
			if (string.IsNullOrEmpty (_engineStartInfo.FileName))
				_engineStartInfo.FileName = "stockfish";

			_engineStartInfo.CreateNoWindow = true;

			_engineStartInfo.UseShellExecute = false;
			_engineStartInfo.RedirectStandardInput = true;
			_engineStartInfo.RedirectStandardOutput = true;
			_engineStartInfo.RedirectStandardError = true;
		}

		public void OutputReceived (object sender, DataReceivedEventArgs eventArgs)
		{
			lock (_syncRoot) {
				_engineOutput.AppendLine (TimeStamp () + eventArgs.Data);
			}

			//using (var log = new StreamWriter ("/tmp/stockfish.log", append: true)) {
			//	log.WriteLine (TimeStamp() + eventArgs.Data);
			//}
		}

		private string TimeStamp ()
		{
			return DateTime.Now.ToLongTimeString () + " Milli: " + DateTime.Now.Millisecond.ToString () + " >> ";
		}

		public string Output {
			get {
				//using (var log = new StreamWriter ("/tmp/stockfish_engineOutput.log", append: false)) {
				//	log.Write (TimeStamp() + _engineOutput.ToString ());
				//}

				lock (_syncRoot) {
					return _engineOutput.ToString ();
				}
			}				
		}
	}
}

