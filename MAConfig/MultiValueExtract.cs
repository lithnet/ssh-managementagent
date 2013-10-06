// -----------------------------------------------------------------------
// <copyright file="MultiValueExtract.cs" company="Lithnet">
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
    using Microsoft.MetadirectoryServices;

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
        /// Gets the capture group name that is used to populate this mapping
        /// </summary>
        public string CaptureGroupName { get; private set; }

        /// <summary>
        /// Gets the regular expression used to capture the individual values from this multivalued attribute
        /// </summary>
        public string MappingRegEx { get; private set; }
        
        /// <summary>
        /// Populates the object from its XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute captureGroupAttribute = node.Attributes["capture-group-name"];
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

            if (captureGroupAttribute == null || string.IsNullOrWhiteSpace(captureGroupAttribute.Value))
            {
                throw new ArgumentNullException("The 'capture-group-name' attribute must be specified");
            }
            else
            {
                this.CaptureGroupName = captureGroupAttribute.Value;
            }
            
            if (string.IsNullOrWhiteSpace(node.InnerText))
            {
                throw new ArgumentNullException("A regular expression capture must be specified");
            }

            this.MappingRegEx = node.InnerText;
        }
    }
}
