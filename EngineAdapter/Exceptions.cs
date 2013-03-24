using System;
using System.Diagnostics;

namespace Fishy.EngineAdapter.Exceptions
{
	public class EngineCouldNotBeStartedException : Exception
	{
		public EngineCouldNotBeStartedException (string engineName, Exception innerException = null) : 
			base(engineName + " engine could not be started", innerException)
		{
		}
	}

}

