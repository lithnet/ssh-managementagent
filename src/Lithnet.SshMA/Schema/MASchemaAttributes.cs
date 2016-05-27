// -----------------------------------------------------------------------
// <copyright file="MASchemaAttributes.cs" company="Lithnet">
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
    /// Represents a keyed collection of MASchemaAttributes objects
    /// </summary>
    public class MASchemaAttributes : KeyedCollection<string, MASchemaAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the MASchemaAttributes class
        /// </summary>
        public MASchemaAttributes()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MASchemaAttributes class
        /// </summary>
        /// <param name="node">The XmlNode representation of this object</param>
        public MASchemaAttributes(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(MASchemaAttribute item)
        {
            return item.Name;
        }
        
        /// <summary>
        /// Populates this collection of MASchemaAttribute objects from an Xml definition
        /// </summary>
        /// <param name="node">The XmlNode representation of this object</param>
        private void FromXml(XmlNode node)
        {
            foreach (XmlNode child in node.SelectNodes("schema-attributes/schema-attribute"))
            {
                this.Add(new MASchemaAttribute(child));
            }
        }
    }
}
