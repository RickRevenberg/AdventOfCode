namespace AdventOfCode.PuzzleSolvers._2021.Helpers
{
	using System.Collections.Generic;

	internal class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		internal new TValue this[TKey key]
		{
			get
			{
				if (!base.ContainsKey(key)) { base.Add(key, default); }
				return base[key];
			}
			set => base[key] = value;
		}
	}
}
