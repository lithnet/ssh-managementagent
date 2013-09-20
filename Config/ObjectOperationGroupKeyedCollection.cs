// -----------------------------------------------------------------------
// <copyright file="ObjectOperationGroupKeyedCollection.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A keyed collection of ObjectOperationGroup objects
    /// </summary>
    public class ObjectOperationGroupKeyedCollection : KeyedCollection<string, ObjectOperationGroup>
    {
        /// <summary>
        /// Initializes a new instance of the ObjectOperationGroupKeyedCollection class
        /// </summary>
        public ObjectOperationGroupKeyedCollection() 
            : base()
        {
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(ObjectOperationGroup item)
        {
            return item.ObjectClass;
        }
    }
}
