namespace AdventOfCode.Logic.Modules
{
    using System;
    using System.Collections.Generic;

    internal class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> defaultOperation;

        internal SafeDictionary(Func<TKey, TValue> defaultValue = null)
        {
            defaultOperation = defaultValue;
        }

        internal new TValue this[TKey key]
        {
	        get
            {
                if (!ContainsKey(key)) { Add(key, defaultOperation == null ? default : defaultOperation.Invoke(key)); }
                return base[key];
            }
	        set
	        {
                if (!ContainsKey(key)){ Add(key, defaultOperation == null ? default : defaultOperation.Invoke(key)); }
		        base[key] = value;
	        }
        }
    }
}
