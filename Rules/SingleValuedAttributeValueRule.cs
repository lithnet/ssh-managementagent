// -----------------------------------------------------------------------
// <copyright file="SingleValuedAttributeValueRule.cs" company="Lithnet">
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
    /// Defines a rule that can be used on a single-valued attributes
    /// </summary>
    public class SingleValuedAttributeValueRule : RuleBase
    {
        /// <summary>
        /// Initializes a new instance of the SingleValuedAttributeValueRule class
        /// </summary>
        public SingleValuedAttributeValueRule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SingleValuedAttributeValueRule class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public SingleValuedAttributeValueRule(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the name of the attribute 
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// Gets the operator to apply to the attribute value
        /// </summary>
        public ValueOperator Operator { get; private set; }

        /// <summary>
        /// Gets the expected value of the attribute
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange for the specified object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        public override bool Evaluate(CSEntryChange csentry)
        {
            if (MASchema.Attributes[this.AttributeName].IsMultiValued)
            {
                throw new ArgumentException("This rule cannot be applied to a multi-valued attribute");
            }
            else
            {
                return this.EvaluateSingleValuedAttribute(csentry);
            }
        }

        /// <summary>
        /// Evaluates a specific value against the rule
        /// </summary>
        /// <param name="value">The value to evaluate</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        protected bool EvaluateAttributeValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            switch (MASchema.Attributes[this.AttributeName].Type)
            {
                case AttributeType.Binary:
                    return ComparisonEngine.CompareBinary((byte[])value, this.Value, this.Operator);

                case AttributeType.Boolean:
                    return ComparisonEngine.CompareBoolean((bool)value, this.Value, this.Operator);

                case AttributeType.Integer:
                    return ComparisonEngine.CompareLong((long)value, this.Value, this.Operator);

                case AttributeType.String:
                    return ComparisonEngine.CompareString((string)value, this.Value, this.Operator);

                case AttributeType.Reference:
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Populates the RuleSet object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            base.FromXml(node);

            XmlAttribute attributeNameAttribute = node.Attributes["attribute"];
            XmlAttribute operatorAttribute = node.Attributes["operator"];
            XmlAttribute valueAttribute = node.Attributes["value"];

            if (attributeNameAttribute == null)
            {
                throw new ArgumentNullException("The 'attribute' attribute must be specified");
            }
            else
            {
                MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
                this.AttributeName = attributeNameAttribute.Value;
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

            if (valueAttribute != null)
            {
                this.Value = valueAttribute.Value;
            }
        }

        /// <summary>
        /// Evaluates the rule against a single-valued attribute
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        private bool EvaluateSingleValuedAttribute(CSEntryChange csentry)
        {
            object value = null;

            if (csentry.AttributeChanges.Any(t => t.Name == this.AttributeName))
            {
                ValueChange valueChange = csentry.AttributeChanges[this.AttributeName].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add);
                value = valueChange == null ? null : valueChange.Value;
            }

            return this.EvaluateAttributeValue(value);
        }
    }
}
