namespace AdventOfCode.PuzzleSolvers._2022
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Logic;
	using NUnit.Framework;

	public class Day_07 : DayBase2022
	{
		public override int Day => 7;

		private Directory Root;

		[SetUp]
		public async Task SetUp()
		{
			var lines = await this.SplitInput();
			Root = new Directory { Path = "" };

			var dirPath = "";
            foreach (var line in lines)
			{
				if (line.StartsWith('$'))
				{
					var command = line.Substring(2, line.Length - 2);
					if (command.StartsWith("cd"))
					{
						command = command.Substring(3, command.Length - 3);
						dirPath = command switch
						{
							"/" => "",
							".." => dirPath.Split("/").Take(dirPath.Split("/").Length - 1).Join("/"),
							_ => dirPath + $"/{command}"
                        };
					}
				}
				else if (line.StartsWith("dir"))
				{
					var dirName = line.Substring(4, line.Length - 4);
					Root.RetrieveStructure().Single(x => x.Path == dirPath).Directories.Add(new Directory
					{
						Name = dirName,
						Path = dirPath + $"/{dirName}"
					});
				}
				else
				{
					var fileSize = line.Split(" ")[0].ToInt();
					Root.RetrieveStructure().Single(x => x.Path == dirPath).Files.Add(new File
					{
						Size = fileSize
					});
				}
			}
		}

		[Test]
	    public override void PartOne()
	    {
			Root.RetrieveStructure().Where(d => d.Size <= 100000).Sum(x => x.Size).Pass();
	    }

		[Test]
	    public override void PartTwo()
	    {
		    var requiredSpace = 30000000 - (70000000 - Root.Size);
			Root.RetrieveStructure().OrderBy(x => x.Size).First(x => x.Size >= requiredSpace).Size.Pass();
	    }


	    private class Directory
	    {
		    internal int Size => Files.Sum(x => x.Size) + this.Directories.Sum(x => x.Size);

		    internal List<Directory> RetrieveStructure()
		    {
			    var data = this.Directories.SelectMany(x => x.RetrieveStructure()).ToList();
				data.Add(this);

				return data;
		    }

			internal string Name { get; set; }
			internal string Path { get; init; }

			internal List<Directory> Directories { get; set; } = new List<Directory>();
			internal List<File> Files { get; set; } = new List<File>();
	    }

	    private class File
	    {
			internal int Size { get; init; }
	    }
    }
}
