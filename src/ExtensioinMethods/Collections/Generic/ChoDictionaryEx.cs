namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public static class ChoDictionaryEx
    {
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        public static void Delete<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }

        public static void RemoveAllMatchingKeyItems<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> itemsToBeRemoved)
        {
            if (itemsToBeRemoved == null) return;

            foreach (TKey item in itemsToBeRemoved)
            {
                if (!dictionary.ContainsKey(item)) continue;
                dictionary.Remove(item);
            }
        }

		public static bool IsEquals<K, V>(this IDictionary<K, V> d1, IDictionary<K, V> d2)
		{
			if (d1.Count != d2.Count)
				return false;

			foreach (KeyValuePair<K, V> pair in d1)
			{
				if (!d2.ContainsKey(pair.Key))
					return false;

				if (!Equals(d2[pair.Key], pair.Value))
					return false;
			}

			return true;
		}
	}
}
