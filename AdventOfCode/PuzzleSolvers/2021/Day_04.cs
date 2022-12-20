namespace AdventOfCode.PuzzleSolvers._2021
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using NUnit.Framework;

	[TestFixture]
    public class Day_04 : DayBase2021
    {
	    private List<string> drawnNumbers = new List<string>();
	    private List<List<(string number, bool drawn)>> boards = new List<List<(string, bool)>>();

	    public override int Day => 4;

        [SetUp]
	    public async Task SetUp()
	    {
		    var input = await this.GetInput();
		    drawnNumbers = input.Substring(0, input.IndexOf("\n\n")).Split(',').ToList();
		    boards = new List<List<(string number, bool drawn)>>();

		    var boardNumbers = input
			    .Substring(input.IndexOf("\n\n"), input.Length - input.IndexOf("\n\n"))
			    .Replace("\n", " ").Split(' ')
			    .Where(x => !string.IsNullOrEmpty(x)).ToList();

		    for (var i = 0; i < boardNumbers.Count / 25; i++)
		    {
			    var boardList = new List<(string, bool)>();
				boardList.AddRange(boardNumbers.Skip(i * 25).Take(25).Select(x => (x, false)));
				boards.Add(boardList);
		    }
	    }

	    [Test]
	    public void PartOne()
	    {
		    foreach (var number in drawnNumbers)
		    {
			    foreach (var board in boards)
			    {
				    for (var i = 0; i < board.Count; i++)
				    {
					    board[i] = (board[i].number, board[i].drawn || board[i].number == number);
				    }
			    }

			    var winningBoard = boards.SingleOrDefault(HasWon);
			    if (winningBoard == null)
			    {
				    continue;
			    }

			    var unmarkedSum = winningBoard.Where(x => !x.drawn).Sum(x => Convert.ToInt32(x.number));
			    var score = unmarkedSum * Convert.ToInt32(number);

				Assert.Pass(score.ToString());
		    }
	    }

	    [Test]
	    public void PartTwo()
	    {
		    var lastDrawnNumber = "";
		    List<(string number, bool drawn)> winningBoard = null;

		    foreach (var number in drawnNumbers)
		    {
			    foreach (var board in boards)
			    {
				    for (var i = 0; i < board.Count; i++)
				    {
					    board[i] = (board[i].number, board[i].drawn || board[i].number == number);
				    }
			    }

			    if (boards.Count > 1)
			    {
				    var winning = boards.Where(HasWon);
				    winning.ToList().ForEach(x => boards.Remove(x));
				}
				else if (HasWon(boards.Single()))
			    {
				    winningBoard = boards.Single();
				    lastDrawnNumber = number;

				    break;
			    }
		    }

		    var unmarkedSum = winningBoard.Where(x => !x.drawn).Sum(x => Convert.ToInt32(x.number));
		    var score = unmarkedSum * Convert.ToInt32(lastDrawnNumber);

			Assert.Pass(score.ToString());
	    }

		private static bool HasWon(List<(string, bool)> board)
	    {
		    for (var i = 0; i < 5; i++)
		    {
			    var row = board.Skip(i * 5).Take(5).ToList();
			    var column = board.Where((_, Index) => Index % 5 == i).ToList();

			    if (row.All(x => x.Item2) || column.All(x => x.Item2))
			    {
				    return true;
			    }
		    }

		    return false;
	    }
    }
}
