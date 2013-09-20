// -----------------------------------------------------------------------
// <copyright file="RuleKeyedCollection.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A keyed collection of IRule objects
    /// </summary>
    public class RuleKeyedCollection : KeyedCollection<string, IRule>
    {
        /// <summary>
        /// Initializes a new instance of the RuleKeyedCollection class
        /// </summary>
        public RuleKeyedCollection() 
            : base()
        {
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(IRule item)
        {
            return item.Id;
        }
    }
}
