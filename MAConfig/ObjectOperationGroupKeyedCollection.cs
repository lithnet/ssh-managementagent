// -----------------------------------------------------------------------
// <copyright file="ObjectOperationGroupKeyedCollection.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993
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
