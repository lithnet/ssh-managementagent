// -----------------------------------------------------------------------
// <copyright file="MASchemaObjects.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System.Collections.ObjectModel;
    using System.Xml;

    /// <summary>
    /// Represents a keyed collection of MASchemaObjects objects
    /// </summary>
    public class MASchemaObjects : KeyedCollection<string, MASchemaObject>
    {
        /// <summary>
        /// Initializes a new instance of the MASchemaObjects class
        /// </summary>
        public MASchemaObjects()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MASchemaObjects class
        /// </summary>
        /// <param name="node">The XmlNode representation of this object</param>
        public MASchemaObjects(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(MASchemaObject item)
        {
            return item.ObjectClass;
        }

        /// <summary>
        /// Populates this collection of MASchemaObject objects from an Xml definition
        /// </summary>
        /// <param name="node">The XmlNode representation of this object</param>
        private void FromXml(XmlNode node)
        {
            foreach (XmlNode child in node.SelectNodes("schema-objects/schema-object"))
            {
                MASchemaObject schemaObject = new MASchemaObject(child);
                this.Add(schemaObject);
            }
        }
    }
}
