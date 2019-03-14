namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    /// <summary>
    /// Provides a basic, component site-specific, key-value pair dictionary through
    /// a service that a designer can use to store user-defined data.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IChoDictionaryService<TKey, TValue>
        //where TKey : class
        //where TValue : class
    {
        /// <summary>
        /// Gets the key corresponding to the specified value.
        /// </summary>
        /// <param name="value">The value to look up in the dictionary.</param>
        /// <returns>The associated key, or null if no key exists.</returns>
        TKey GetKey(TValue value);

        /// <summary>
        /// Gets the value corresponding to the specified key.
        /// </summary>
        /// <param name="key">The key to look up the value for.</param>
        /// <returns>The associated value, or null if no value exists.</returns>
        TValue GetValue(TKey key);

        /// <summary>
        /// Sets the specified key-value pair.
        /// </summary>
        /// <param name="key">An object to use as the key to associate the value with.</param>
        /// <param name="value">The value to store.</param>
        void SetValue(TKey key, TValue value);

        /// <summary>
        /// Sets the specified key-value pair.
        /// </summary>
        /// <param name="key">An object to use as the key to associate the value with.</param>
        /// <param name="value">The value to store.</param>
        void RemoveValue(TKey key);

		bool ContainsKey(TKey key);

		bool ContainsValue(TValue value);

        object SyncRoot { get; }
    }
}
