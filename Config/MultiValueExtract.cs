// -----------------------------------------------------------------------
// <copyright file="MultiValueExtract.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
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
    /// A class used to map the individual values of a multi-valued reference from an import-mapping
    /// /// </summary>
    public class MultiValueExtract
    {
        /// <summary>
        /// Initializes a new instance of the MultiValueExtract class
        /// </summary>
        public MultiValueExtract()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiValueExtract class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public MultiValueExtract(XmlNode node)
        {
            this.FromXml(node);
        }
       
        /// <summary>
        /// Gets the reference that this mapping populates
        /// </summary>
        public MASchemaAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the regular expression used to capture this multi-valued reference
        /// </summary>
        public string MappingRegEx { get; private set; }
        
        /// <summary>
        /// Populates the object from its XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute captureGroupAttribute = node.Attributes["capture-group"];
            XmlAttribute attributeNameAttribute = node.Attributes["attribute"];

            if (attributeNameAttribute == null || string.IsNullOrWhiteSpace(attributeNameAttribute.Value))
            {
                throw new ArgumentNullException("The 'attribute' attribute must be specified");
            }
            else
            {
                MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
                this.Attribute = MASchema.Attributes[attributeNameAttribute.Value];
            }

            if (string.IsNullOrWhiteSpace(node.InnerText))
            {
                throw new ArgumentNullException("A regular expression capture must be specified");
            }

            this.MappingRegEx = node.InnerText;
        }
    }
}
