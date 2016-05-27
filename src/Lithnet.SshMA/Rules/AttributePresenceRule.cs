// -----------------------------------------------------------------------
// <copyright file="AttributePresenceRule.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993// </copyright>
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
    /// Defines a rule that can be used to determine the presence of an attribute
    /// </summary>
    public class AttributePresenceRule : RuleBase
    {
        /// <summary>
        /// Initializes a new instance of the AttributePresenceRule class
        /// </summary>
        public AttributePresenceRule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributePresenceRule class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public AttributePresenceRule(XmlNode node)
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
        public PresenceOperator Operator { get; private set; }

        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange for the specified object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        public override bool Evaluate(CSEntryChange csentry)
        {
            if (this.Operator == PresenceOperator.IsPresent)
            {
                return csentry.AttributeChanges.Any(t => t.Name == this.AttributeName);
            }
            else
            {
                return !csentry.AttributeChanges.Any(t => t.Name == this.AttributeName);
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

            if (attributeNameAttribute == null)
            {
                throw new ArgumentNullException("The 'attribute' attribute must be specified");
            }
            else
            {
                MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
                this.AttributeName = attributeNameAttribute.Value;
            }

            PresenceOperator presenceOperator = PresenceOperator.IsPresent;
            if (Enum.TryParse(operatorAttribute.Value, true, out presenceOperator))
            {
                this.Operator = presenceOperator;
            }
            else
            {
                throw new ArgumentException(string.Format("The presence operator '{0}' is unknown", operatorAttribute.Value));
            }
        }
    }
}