// -----------------------------------------------------------------------
// <copyright file="ObjectFilter.cs" company="Lithnet">
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
    /// Defines a filter for an entry in a target system
    /// </summary>
    public class ObjectFilter
    {
        /// <summary>
        /// Initializes a new instance of the ObjectFilter class
        /// </summary>
        public ObjectFilter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectFilter class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ObjectFilter(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the reference to filter upon
        /// </summary>
        public MASchemaAttribute Attribute { get; private set; }

        /// <summary>
        /// Gets the value operator to use against the expected value
        /// </summary>
        public ValueOperator Operator { get; private set; }

        /// <summary>
        /// Gets the expected value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Determines if the specified object should be filtered from the result set
        /// </summary>
        /// <param name="csentry">The CSEntryChange object</param>
        /// <returns>A value indicating whether the specified object should be filtered from the result set</returns>
        public bool IsFiltered(CSEntryChange csentry)
        {
            AttributeChange attributeChange = csentry.AttributeChanges.FirstOrDefault(t => t.Name == this.Attribute.Name);

            if (attributeChange == null)
            {
                return false;
            }

            if (attributeChange.IsMultiValued)
            {
                return this.IsFilteredMultiValue(attributeChange);
            }
            else
            {
                return this.IsFilteredSingleValue(attributeChange);
            }
        }

        /// <summary>
        /// Determines if the specified single-valued attribute change matches this object filter
        /// </summary>
        /// <param name="attributeChange">The attribute change object</param>
        /// <returns>A value indicating whether the specified object should be filtered from the result set</returns>
        private bool IsFilteredSingleValue(AttributeChange attributeChange)
        {
            ValueChange valueChange = attributeChange.ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add);

            if (valueChange == null)
            {
                return false;
            }

            switch (this.Attribute.Type)
            {
                case AttributeType.Binary:
                    return ComparisonEngine.CompareBinary((byte[])valueChange.Value, this.Value, this.Operator);

                case AttributeType.Boolean:
                    return ComparisonEngine.CompareBoolean(valueChange.Value as string, this.Value, this.Operator);

                case AttributeType.Integer:
                    return ComparisonEngine.CompareLong(valueChange.Value as string, this.Value, this.Operator);

                case AttributeType.String:
                    return ComparisonEngine.CompareString((string)valueChange.Value, this.Value, this.Operator);

                case AttributeType.Reference:
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Determines if the specified multi-valued attribute change matches this object filter
        /// </summary>
        /// <param name="attributeChange">The attribute change object</param>
        /// <returns>A value indicating whether the specified object should be filtered from the result set</returns>
        private bool IsFilteredMultiValue(AttributeChange attributeChange)
        {
            foreach (ValueChange valueChange in attributeChange.ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add))
            {
                bool result;

                switch (this.Attribute.Type)
                {
                    case AttributeType.Binary:
                        result = ComparisonEngine.CompareBinary((byte[])valueChange.Value, this.Value, this.Operator);
                        break;

                    case AttributeType.Boolean:
                        result = ComparisonEngine.CompareBoolean(valueChange.Value as string, this.Value, this.Operator);
                        break;

                    case AttributeType.Integer:
                        result = ComparisonEngine.CompareLong(valueChange.Value as string, this.Value, this.Operator);
                        break;

                    case AttributeType.String:
                        result = ComparisonEngine.CompareString((string)valueChange.Value, this.Value, this.Operator);
                        break;

                    case AttributeType.Reference:
                        result = ComparisonEngine.CompareString((string)valueChange.Value, this.Value, this.Operator);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                if (result)
                {
                    return result;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute attributeNameAttribute = node.Attributes["attribute"];
            XmlAttribute operatorAttribute = node.Attributes["operator"];

            if (attributeNameAttribute == null)
            {
                throw new ArgumentNullException("The 'attribute' attribute must be specified");
            }
            else
            {
                MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
                this.Attribute = MASchema.Attributes[attributeNameAttribute.Value];
            }

            ValueOperator valueOperator = ValueOperator.Equals;
            if (Enum.TryParse(operatorAttribute.Value, true, out valueOperator))
            {
                this.Operator = valueOperator;
            }
            else
            {
                throw new ArgumentException(string.Format("The value operator '{0}' is unknown", operatorAttribute.Value));
            }

            this.Value = node.InnerText;
        }
    }
}
