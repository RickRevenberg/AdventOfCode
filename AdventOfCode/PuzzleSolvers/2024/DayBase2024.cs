namespace AdventOfCode.PuzzleSolvers._2024
{
    public abstract class DayBase2024 : DayBase
    {
        public override int Year => 2024;

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
