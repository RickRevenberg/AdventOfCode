namespace AdventOfCode.PuzzleSolvers._2023
{
	public abstract class DayBase2023 : DayBase
    {
	    public override int Year => 2023;

	    protected async Task<List<string>> SplitInput()
	    {
		    return (TestInput() ?? await this.GetInput()).Split("\n").Select(x => x.Replace("\r", "")).ToList();
	    }

        protected virtual string TestInput()
        {
            return null;
        }
    }
}
