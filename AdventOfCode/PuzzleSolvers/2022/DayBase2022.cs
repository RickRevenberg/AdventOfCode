namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public abstract class DayBase2022 : DayBase
	{
		public override int Year => 2022;

		protected async Task<List<string>> SplitInput()
		{
			return (await this.GetInput()).Split("\n").ToList();
		}
	}
}
