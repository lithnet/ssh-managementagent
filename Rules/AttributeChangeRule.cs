// -----------------------------------------------------------------------
// <copyright file="AttributeChangeRule.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// A rule that can detect a state change in an attribute
    /// </summary>
    public class AttributeChangeRule : RuleBase
    {
        /// <summary>
        /// Initializes a new instance of the AttributeChangeRule class
        /// </summary>
        public AttributeChangeRule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeChangeRule class
        /// </summary>
        /// <param name="node">The XmlNode representation of this object</param>
        public AttributeChangeRule(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets or sets the attribute name that this rule applies to
        /// </summary>
        private string AttributeName { get; set; }

        /// <summary>
        /// Gets or sets the trigger events that this rule applies to 
        /// </summary>
        private TriggerEvents TriggerEvents { get; set; }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange that applies to this rule</param>
        /// <returns>A value indicating whether the rule was successful or not</returns>
        public override bool Evaluate(CSEntryChange csentry)
        {
            return this.EvaulateRuleOnAttributeChanges(csentry.AttributeChanges);
        }

        /// <summary>
        /// Populates the rule based on an XML representation of the object
        /// </summary>
        /// <param name="node">The XmlNode representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            base.FromXml(node);

            XmlAttribute attributeNameAttribute = node.Attributes["attribute"];
            XmlAttribute triggerAttribute = node.Attributes["triggers"];

            if (attributeNameAttribute == null || triggerAttribute == null)
            {
                throw new ArgumentException("The attribute name and trigger must be specified");
            }

            MASchema.ThrowOnMissingAttribute(attributeNameAttribute.Value);
            this.AttributeName = attributeNameAttribute.Value;

            TriggerEvents events;
            if (Enum.TryParse(triggerAttribute.Value, true, out events))
            {
                this.TriggerEvents = events;
            }
            else
            {
                throw new ArgumentException("The trigger value is unknown or unsupported");
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified list of attribute changes
        /// </summary>
        /// <param name="attributeChanges">The list of attribute changes made to the object</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaulateRuleOnAttributeChanges(KeyedCollection<string, AttributeChange> attributeChanges)
        {
            foreach (AttributeChange attributeChange in attributeChanges.Where(t => this.AttributeName == t.Name))
            {
                if (this.EvaluateRuleOnAttributeChange(attributeChange))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates the rule against the specified attribute change
        /// </summary>
        /// <param name="attributeChange">The attribute change to evaluate</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaluateRuleOnAttributeChange(AttributeChange attributeChange)
        {
            bool result = false;

            if (attributeChange.ModificationType == AttributeModificationType.Add && this.TriggerEvents.HasFlag(TriggerEvents.Add))
            {
                result = true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Update && this.TriggerEvents.HasFlag(TriggerEvents.Update))
            {
                result = true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Replace && this.TriggerEvents.HasFlag(TriggerEvents.Update))
            {
                result = true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Delete && this.TriggerEvents.HasFlag(TriggerEvents.Delete))
            {
                result = true;
            }

            if (result)
            {
                Logger.WriteLine("Attribute change rule '{0}' passed due to attribute modification type '{1}' on '{2}'", LogLevel.Debug, this.AttributeName, attributeChange.ModificationType.ToString(), attributeChange.Name);
            }

            return result;
        }
    }
}
