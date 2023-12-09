namespace AdventOfCode.PuzzleSolvers._2023
{
    using System.Text.RegularExpressions;
    using Logic.Extensions;
    using Logic.Modules;

    public class Day_08 : DayBase2023
    {
        public override int Day => 8;

        private List<char> instructions;
        private SafeDictionary<string, DoublyLinkedListNode<string>> linkNodes;


        [SetUp]
        public async Task SetUp()
        {
            var input = await this.SplitInput();

            this.instructions = input[0].ToCharArray().ToList();
            this.linkNodes = new SafeDictionary<string, DoublyLinkedListNode<string>>(id => new DoublyLinkedListNode<string> { Value = id });

            var idRegex = new Regex(@"\w{3}");

            var nodes = input.Skip(2).ToList();
            nodes.ForEach(node =>
            {
                var ids = idRegex.Matches(node).Select(x => x.Value).ToList();

                linkNodes[ids[0]].Previous = linkNodes[ids[1]];
                linkNodes[ids[0]].Next = linkNodes[ids[2]];
            });
        }

        [Test]
        public void PartOne()
        {
            var counter = 0;
            var tracker = 0;

            var current = this.linkNodes["AAA"];

            while (true)
            {
                counter++;

                current = this.instructions[tracker] == 'R' ? current.Next : current.Previous;

                if (current.Value == "ZZZ")
                {
                    break;
                }

                tracker = (tracker + 1) % this.instructions.Count;
            }

            counter.Pass();
        }

        [Test]
        public void PartTwo()
        {
            var startNodes = this.linkNodes.Keys.Where(x => x.EndsWith("A")).Select(key => this.linkNodes[key]).ToList();

            var ringLenths = new List<int>();
            for (var i = 0; i < startNodes.Count; i++)
            {
                var tracker = 0;
                var currNode = startNodes[i];
                var historyDict = new SafeDictionary<string, bool>();

                while (true)
                {
                    var instruction = this.instructions[tracker];
                    var currIndex = $"{tracker}_{currNode.Value}";

                    if (historyDict.ContainsKey(currIndex))
                    {
                        var offset = historyDict.Keys.ToList().IndexOf(currIndex);
                        ringLenths.Add(historyDict.Keys.Count - offset);
                        
                        break;
                    }

                    historyDict[currIndex] = false;
                    tracker = (tracker + 1) % this.instructions.Count;

                    currNode = instruction == 'R' ? currNode.Next : currNode.Previous;
                }
            }

            ringLenths.Select(x => (long)x).ToList().LCM().Pass();
        }
    }
}
