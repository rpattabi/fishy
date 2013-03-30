using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fishy.Engine
{
	public class Score : IScore
	{
		public static IScore Create (string engineScoreLine)
		{
			var groups = Regex.Match (engineScoreLine, @"score (mate|cp) (\d+)").Groups;
			double score = double.Parse (groups[2].Value);

			if (groups[1].Value == "mate") {
				return new MateScore(score);
			}
			else {
				return new Score(score);
			}
		}

		internal Score (double value)
		{
			this.Value = value;
		}

		public double Value { get; private set; }

		public override string ToString ()
		{
			return this.Value.ToString ();
		}
	}

	internal class MateScore : IScore
	{
		internal MateScore (double value)
		{
			this.Value = value;
		}

		public double Value { get; private set; }

		public override string ToString ()
		{
			return "#" + int.Parse (this.Value.ToString ());
		}
	}
}

