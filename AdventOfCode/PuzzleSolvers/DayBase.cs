﻿namespace AdventOfCode.PuzzleSolvers
{
	using Logic;

	public abstract class DayBase
	{
		public abstract int Day { get; }
		public abstract int Year { get; }

		public async Task<string> GetInput()
		{
			var cacheDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../../../InputCache");
			if (!Directory.Exists(cacheDirectory))
			{
				Directory.CreateDirectory(cacheDirectory);
			}

			var cacheFile = Path.Combine(cacheDirectory, $"{Year}-{Day}.txt");
			if (File.Exists(cacheFile))
			{
				var input = await File.ReadAllTextAsync(cacheFile);

				// Remove last newline character From string.
				input = input.Substring(0, input.Length - 1);

				return input;
			}
			else
			{
				var input = await ApiConnector.RetrieveInput(Year, Day);
				File.WriteAllText(cacheFile, input);

				input = input.Substring(0, input.Length - 1);
				return input;
			}
		}
	}
}
