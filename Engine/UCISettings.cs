using System;
using System.Text.RegularExpressions;

namespace Fishy.Engine
{
	public class UCISettings
	{
		string _engineOutputForUCICommand;

		public UCISettings (string engineOutputForUCICommand)
		{
			_engineOutputForUCICommand = engineOutputForUCICommand;
		}

		public string Id {
			get {
				return Regex.Match (_engineOutputForUCICommand, "id name (.*)").Groups[1].Value;
			}
		}
	}
}

