// -----------------------------------------------------------------------
// <copyright file="MultiValuedAttributeValueRule.cs" company="Lithnet">
// Copyright (c) 2013
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
    /// Defines a rule that can be used on a multi-valued attributes
    /// </summary>
    public class MultiValuedAttributeValueRule : SingleValuedAttributeValueRule
    {
        /// <summary>
        /// Initializes a new instance of the MultiValuedAttributeValueRule class
        /// </summary>
        public MultiValuedAttributeValueRule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiValuedAttributeValueRule class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public MultiValuedAttributeValueRule(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the conditions required to apply this rule to a multi-valued attribute
        /// </summary>
        public MultiValueCondition MultiValueCondition { get; private set; }

        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange for the specified object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        public override bool Evaluate(CSEntryChange csentry)
        {
            if (MASchema.Attributes[this.AttributeName].IsMultiValued)
            {
                return this.EvaluateMultiValuedAttribute(csentry);
            }
            else
            {
                throw new InvalidOperationException("The rule cannot be used on a single-valued attribute");
            }
        }

        /// <summary>
        /// Populates the RuleSet object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            base.FromXml(node);

            XmlAttribute multivalueConditionAttribute = node.Attributes["multivalue-condition"];

            if (multivalueConditionAttribute == null)
            {
                if (MASchema.Attributes[this.AttributeName].IsMultiValued)
                {
                    throw new ArgumentException(string.Format("The rule specifies a multi-valued attribute for evaulation, but does not specify a multivalue-condition operator"));
                }
            }
            else
            {
                if (!MASchema.Attributes[this.AttributeName].IsMultiValued)
                {
                    throw new ArgumentException(string.Format("The rule specifies a multivalued-condition for a single-valued attribute"));
                }

                MultiValueCondition multivalueCondition = MultiValueCondition.Any;
                if (Enum.TryParse(multivalueConditionAttribute.Value, true, out multivalueCondition))
                {
                    this.MultiValueCondition = multivalueCondition;
                }
                else
                {
                    throw new ArgumentException(string.Format("The multivalued-condition operator '{0}' is unknown", multivalueConditionAttribute.Value));
                }
            }
        }

        /// <summary>
        /// Evaluates the rule against a multi-valued attribute
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        private bool EvaluateMultiValuedAttribute(CSEntryChange csentry)
        {
            if (!csentry.AttributeChanges.Any(t => t.Name == this.AttributeName))
            {
                switch (this.MultiValueCondition)
                {
                    case MultiValueCondition.One:
                    case MultiValueCondition.All:
                    case MultiValueCondition.Any:
                        return false;

                    case MultiValueCondition.None:
                        return true;
                    default:
                        break;
                }
            }
            else if (csentry.AttributeChanges[this.AttributeName].ValueChanges.Count(t => t.ModificationType == ValueModificationType.Add) == 0)
            {
                switch (this.MultiValueCondition)
                {
                    case MultiValueCondition.One:
                    case MultiValueCondition.All:
                    case MultiValueCondition.Any:
                        return false;

                    case MultiValueCondition.None:
                        return true;
                    default:
                        break;
                }
            }

            bool hasSuccess = false;

            foreach (ValueChange change in csentry.AttributeChanges[this.AttributeName].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add))
            {
                bool result = this.EvaluateAttributeValue(change.Value);

                switch (this.MultiValueCondition)
                {
                    case MultiValueCondition.None:
                        if (result)
                        {
                            return false;
                        }

                        break;

                    case MultiValueCondition.All:
                        if (!result)
                        {
                            return false;
                        }

                        break;

                    case MultiValueCondition.Any:
                        if (result)
                        {
                            return true;
                        }

                        break;

                    case MultiValueCondition.One:
                        if (result)
                        {
                            if (hasSuccess)
                            {
                                return false;
                            }
                        }

                        break;
                }

                if (result)
                {
                    hasSuccess = true;
                }
            }

            return true;
        }
    }
}