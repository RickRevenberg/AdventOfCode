namespace AdventOfCode.Logic.Modules
{
	using System;

	internal class InfiniteNumber
    {
	    public override int GetHashCode()
	    {
		    return this.ToString().GetHashCode();
	    }

	    private bool positive = true;

        private List<bool> BinaryBits = new ();

        public InfiniteNumber Power(int power)
        {
	        if (power < 2)
	        {
		        return this;
	        }

	        var root = this.Clone();
            var powerNumber = this.Clone();

            for (var i = 0; i < power; i++)
            {
	            root *= powerNumber;
            }

            return root;
        }

        public InfiniteNumber Clone()
        {
	        return new InfiniteNumber
	        {
		        BinaryBits = this.BinaryBits,
		        positive = this.positive
	        };
        }

        internal static InfiniteNumber FromInt(int number)
	    {
		    var binaryValue = Convert.ToString(number, 2);
		    return FromBinaryString(binaryValue);
	    }

	    internal static InfiniteNumber FromBinaryString(string binaryString)
	    {
		    var negative = binaryString.StartsWith('-');
		    var newBinaryData = binaryString.TrimStart('-').ToCharArray().Select(c => c == '1').ToList();

		    return new InfiniteNumber
		    {
			    positive = !negative,
			    BinaryBits = newBinaryData
		    };
	    }

	    protected bool Equals(InfiniteNumber other)
	    {
		    return this == other;
	    }

	    public override bool Equals(object obj)
	    {
		    if (obj is not InfiniteNumber num)
		    {
			    return false;
		    }

		    return this == num;
	    }

        private void PruneLeadingZeroes()
	    {
		    var firstIndex = this.BinaryBits.IndexOf(true);
		    if (firstIndex < 0)
		    {
				this.BinaryBits.Clear();
		    }
		    else
		    {
			    this.BinaryBits = this.BinaryBits.Skip(firstIndex).ToList();
		    }
	    }

	    public static InfiniteNumber operator +(InfiniteNumber one, int two) => one + FromInt(two);
	    public static InfiniteNumber operator +(InfiniteNumber one, InfiniteNumber two)
	    {
		    var numberOne = one.Clone();
		    var numberTwo = two.Clone();
			
		    if (numberOne.positive && !numberTwo.positive)
		    {
			    numberTwo.positive = true;
			    return numberOne - numberTwo;
		    }

		    if (!numberOne.positive && numberTwo.positive)
		    {
			    numberOne.positive = true;
			    return numberTwo - numberOne;
		    }

		    var bothNegative = !numberOne.positive && !numberTwo.positive;

		    var larger = numberOne > numberTwo ? numberOne : numberTwo;
		    var smaller = larger == numberOne ? numberTwo : numberOne;

			var sizeDiff = larger.BinaryBits.Count - smaller.BinaryBits.Count;

		    var carryOver = 0;
		    var newData = new List<bool>();

		    for (var i = smaller.BinaryBits.Count - 1; i >= 0; i--)
		    {
			    var valueOne = larger.BinaryBits[i + sizeDiff];
			    var valueTwo = smaller.BinaryBits[i];

				var total = ToInt(valueOne) + ToInt(valueTwo) + carryOver;
				newData.Add(total % 2 == 1);

				carryOver = total > 1 ? 1 : 0;
		    }

		    if (carryOver > 0)
		    {
			    var subTotal = (new InfiniteNumber { BinaryBits = larger.BinaryBits.Take(sizeDiff).ToList() } + FromBinaryString("1"));
			    subTotal.BinaryBits.Reverse();

			    newData.AddRange(subTotal.BinaryBits);
            }
		    else if (sizeDiff > 0)
		    {
			    larger.BinaryBits = larger.BinaryBits.Take(sizeDiff).ToList();
			    larger.BinaryBits.Reverse();

			    newData.AddRange(larger.BinaryBits);
		    }

            newData.Reverse();

		    var result = new InfiniteNumber
		    {
				positive = !bothNegative,
			    BinaryBits = newData
		    };

			result.PruneLeadingZeroes();

			return result;
	    }

	    public static InfiniteNumber operator -(InfiniteNumber one, int two) => one - FromInt(two);
	    public static InfiniteNumber operator -(InfiniteNumber one, InfiniteNumber two)
	    {
		    var numberOne = one.Clone();
			var numberTwo = two.Clone();

			if (!numberTwo.positive)
		    {
			    numberTwo.positive = true;
			    return numberOne + numberTwo;
		    }

		    if (!numberOne.positive && !numberTwo.positive)
		    {
			    numberOne.positive = true;
			    numberTwo.positive = true;

				return numberOne + numberTwo;
            }

		    var resultIsNegative = numberTwo > numberOne;

			var larger = resultIsNegative ? numberTwo : numberOne;
			var smaller = larger == numberOne ? numberTwo : numberOne;

			var carry = 0;
			var newData = new List<bool>();
			var sizeDiff = larger.BinaryBits.Count - smaller.BinaryBits.Count;

			for (var i = smaller.BinaryBits.Count - 1; i >= 0; i--)
			{
				var bitOne = ToInt(larger.BinaryBits[i + sizeDiff]);
				var bitTwo = ToInt(smaller.BinaryBits[i]);

				var total = bitOne + bitTwo + carry;
				newData.Add(total % 2 != 0);

				carry = (bitTwo + carry) > bitOne ? 1 : 0;
			}

			if (carry > 0)
			{
				var subTotal = (new InfiniteNumber { BinaryBits = larger.BinaryBits.Take(sizeDiff).ToList() } - FromBinaryString("1"));
				subTotal.BinaryBits.Reverse();
				
				newData.AddRange(subTotal.BinaryBits);
			}
			else if (sizeDiff > 0)
			{
				larger.BinaryBits = larger.BinaryBits.Take(sizeDiff).ToList();
				larger.BinaryBits.Reverse();

				newData.AddRange(larger.BinaryBits);
			}

			newData.Reverse();

			var result = new InfiniteNumber
			{
				positive = !resultIsNegative,
				BinaryBits = newData
			};

			result.PruneLeadingZeroes();

			return result;
	    }

		public static InfiniteNumber operator *(InfiniteNumber one, int two) => one * FromInt(two);
		public static InfiniteNumber operator *(InfiniteNumber one, InfiniteNumber two)
		{
			var numberOne = one.Clone();
			var numberTwo = two.Clone();

		    var resultIsNegative = numberOne.positive != numberTwo.positive;

		    var multiplicant = numberOne.BinaryBits.Count >= numberTwo.BinaryBits.Count ? numberOne : numberTwo;
		    var multiplier = multiplicant == numberOne ? numberTwo : numberOne;

		    var total = FromInt(0);
            var multiplicantString = multiplicant.ToString();

            multiplicant.BinaryBits.Reverse();
            multiplier.BinaryBits.Reverse();

            for (var i = 0; i < multiplier.BinaryBits.Count; i++)
		    {
			    var partial = (multiplier.BinaryBits[i] ? multiplicantString : "").PadRight(multiplicantString.Length + i, '0');

			    total += FromBinaryString(partial);
		    }

		    total.positive = !resultIsNegative;
			total.PruneLeadingZeroes();

		    return total;
	    }

	    public static bool operator ==(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
		    return numberOne?.ToString() == numberTwo?.ToString();
	    }
	    public static bool operator !=(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
		    return !(numberOne == numberTwo);
	    }

		public static bool operator >(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
		    var countOne = numberOne.BinaryBits.Count - 1;
		    var countTwo = numberTwo.BinaryBits.Count - 1;

		    if (countOne != countTwo)
		    {
				return countOne > countTwo;
		    }

		    for (var i = 0; i < countOne; i++)
		    {
			    if (numberOne.BinaryBits[i] == numberTwo.BinaryBits[i])
			    {
				    continue;
			    }

			    return numberOne.BinaryBits[i];
		    }

		    return false;
	    }
	    public static bool operator <(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
			return !(numberOne >= numberTwo);
	    }

	    public static bool operator >=(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
		    return (numberOne == numberTwo) || (numberOne > numberTwo);
	    }
	    public static bool operator <=(InfiniteNumber numberOne, InfiniteNumber numberTwo)
	    {
		    return !(numberOne > numberTwo);
	    }

        public override string ToString()
        {
	        if (!BinaryBits.Any())
	        {
		        return "0";
	        }

	        return (positive ? "" : "-") + string.Join("", BinaryBits.Select(ToInt));
        }

		public string ToHexFormat()
		{
			var data = this.Clone().BinaryBits;
			data.Reverse();

			var chunkedData = data.Chunk(4).Select(x =>
			{
				var y = x.Reverse().Select(ToInt).ToList();
				var binaryString = string.Join("", y).PadLeft(4, '0');

				return Convert.ToInt16(binaryString, 2).ToString("X");
			}).ToList();

			chunkedData.Reverse();

			return (this.positive ? "" : "-") + string.Join("", chunkedData);
		}

		private static int ToInt(bool value)
		{
			return value ? 1 : 0;
		}
    }
}
