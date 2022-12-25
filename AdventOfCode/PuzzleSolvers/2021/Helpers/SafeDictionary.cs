namespace AdventOfCode.PuzzleSolvers._2021.Helpers
{
	using System;
	using System.Collections.Generic;

	internal class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private readonly Func<TKey, TValue> defaultOperation;

		internal SafeDictionary(Func<TKey, TValue> defaultValue = null)
		{
			this.defaultOperation = defaultValue;
		}

		internal new TValue this[TKey key]
		{
			get
			{
				if (!base.ContainsKey(key)) { base.Add(key, defaultOperation == null ? default : defaultOperation.Invoke(key)); }
				return base[key];
			}
			set => base[key] = value;
		}
	}
}
