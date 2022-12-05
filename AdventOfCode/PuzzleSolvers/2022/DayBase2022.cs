namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Threading.Tasks;

	public abstract class DayBase2022 : DayBase
	{
		public override int Year => 2022;

		protected async Task<string[]> SplitInput()
		{
			return (await this.GetInput()).Split("\n");
		}
	}
}
