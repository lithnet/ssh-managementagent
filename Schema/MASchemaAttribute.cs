// -----------------------------------------------------------------------
// <copyright file="MASchemaAttribute.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Xml;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Defines an reference belonging to one or more schema objects
    /// </summary>
    public class MASchemaAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MASchemaAttribute class
        /// </summary>
        public MASchemaAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MASchemaAttribute class
        /// </summary>
        /// <param name="node">The XML representation of this object</param>
        public MASchemaAttribute(XmlNode node)
            : this()
        {
            XmlAttribute nameAttribute = node.Attributes["name"];
            XmlAttribute typeAttribute = node.Attributes["type"];
            XmlAttribute multiValuedAttribute = node.Attributes["multivalued"];
            XmlAttribute operationAttribute = node.Attributes["operation"];
            XmlAttribute canResurrectAttribute = node.Attributes["canresurrect"];

            if (nameAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value))
            {
                throw new ArgumentException("The attribute element must have a 'name' attribute");
            }
            else
            {
                this.Name = nameAttribute.Value;
            }

            if (typeAttribute == null || string.IsNullOrWhiteSpace(typeAttribute.Value))
            {
                throw new ArgumentException("The attribute element must have a 'type' attribute");
            }
            else
            {
                Microsoft.MetadirectoryServices.AttributeType type = Microsoft.MetadirectoryServices.AttributeType.String;
                if (Enum.TryParse(typeAttribute.Value, true, out type))
                {
                    this.Type = type;
                }
                else
                {
                    throw new ArgumentException(string.Format("The value '{0}' for the attribute type was invalid", typeAttribute.Value));
                }
            }
            
            if (operationAttribute == null || string.IsNullOrWhiteSpace(operationAttribute.Value))
            {
                throw new ArgumentException("The attribute element must have a 'operation' attribute");
            }
            else
            {
                Microsoft.MetadirectoryServices.AttributeOperation operation = AttributeOperation.ImportExport;
                if (Enum.TryParse(operationAttribute.Value, true, out operation))
                {
                    this.Operation = operation;
                }
                else
                {
                    throw new ArgumentException(string.Format("The value '{0}' for the operation type was invalid", operationAttribute.Value));
                }
            }

            if (canResurrectAttribute != null)
            {
                bool value = false;
                if (bool.TryParse(canResurrectAttribute.Value, out value))
                {
                    this.CanResurrect = value;
                }
                else
                {
                    throw new ArgumentException(string.Format("The value '{0}' for the resurrection mode was invalid", canResurrectAttribute.Value));
                }
            }

            if (multiValuedAttribute != null && !string.IsNullOrWhiteSpace(multiValuedAttribute.Value))
            {
                bool value = false;
                if (bool.TryParse(multiValuedAttribute.Value, out value))
                {
                    this.IsMultiValued = value;
                }
                else
                {
                    throw new ArgumentException(string.Format("The value '{0}' for the 'multivalued' attribute was invalid", multiValuedAttribute.Value));
                }
            }
        }

        /// <summary>
        /// Gets the reference name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this reference is multi-valued or not
        /// </summary>
        public bool IsMultiValued { get; internal set; }

        /// <summary>
        /// Gets the data type of this reference
        /// </summary>
        public AttributeType Type { get; internal set; }

        /// <summary>
        /// Gets the allowed operations for this reference
        /// </summary>
        public AttributeOperation Operation { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this reference can be used to resurrect a deleted object
        /// </summary>
        public bool CanResurrect { get; internal set; }
    }
}
