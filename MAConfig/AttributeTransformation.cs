// -----------------------------------------------------------------------
// <copyright file="AttributeTransformation.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Defines an reference transformation
    /// </summary>
    public class AttributeTransformation
    {
        /// <summary>
        /// Initializes a new instance of the AttributeTransformation class
        /// </summary>
        public AttributeTransformation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeTransformation class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public AttributeTransformation(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the reference to transform
        /// </summary>
        public MASchemaAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the regular expression find pattern
        /// </summary>
        public string RegexFind { get; private set; }

        /// <summary>
        /// Gets the regular expression replace pattern
        /// </summary>
        public string RegexReplace { get; private set; }

        /// <summary>
        /// Populates the object based on an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute attributeNameAttribute = node.Attributes["attribute"];
            XmlAttribute regexFindAttibute = node.Attributes["regex-find"];
            XmlAttribute regexReplaceAttribute = node.Attributes["regex-replace"];

            if (attributeNameAttribute == null || string.IsNullOrWhiteSpace(attributeNameAttribute.Value))
            {
                throw new ArgumentNullException("An attribute attribute must be specified");
            }
            else
            {
                MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
                this.Attribute = MASchema.Attributes[attributeNameAttribute.Value];
            }

            if (regexFindAttibute == null || string.IsNullOrWhiteSpace(regexFindAttibute.Value))
            {
                throw new ArgumentNullException("A regex-find attribute must be specified");
            }
            else
            {
                this.RegexFind = regexFindAttibute.Value;
            }

            if (regexReplaceAttribute == null || string.IsNullOrWhiteSpace(regexReplaceAttribute.Value))
            {
                throw new ArgumentNullException("A regex-replace attribute must be specified");
            }
            else
            {
                this.RegexReplace = regexReplaceAttribute.Value;
            }
        }
    }
}
