// -----------------------------------------------------------------------
// <copyright file="ObjectChangeRule.cs" company="Lithnet">
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
    /// A rule used to detect a state change on an object
    /// </summary>
    public class ObjectChangeRule : RuleBase
    {
        /// <summary>
        /// Initializes a new instance of the ObjectChangeRule class
        /// </summary>
        public ObjectChangeRule()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ObjectChangeRule class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public ObjectChangeRule(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets or sets a list of trigger events
        /// </summary>
        public TriggerEvents TriggerEvents { get; set; }

        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange for the specified object</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        public override bool Evaluate(CSEntryChange csentry)
        {
            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                    if (this.TriggerEvents.HasFlag(TriggerEvents.Add))
                    {
                        return true;
                    }

                    break;

                case ObjectModificationType.Delete:
                    if (this.TriggerEvents.HasFlag(TriggerEvents.Delete))
                    {
                        return true;
                    }

                    break;

                case ObjectModificationType.Replace:
                case ObjectModificationType.Update:
                    if (this.TriggerEvents.HasFlag(TriggerEvents.Update))
                    {
                        return true;
                    }

                    break;

                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Populates the RuleSet object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            base.FromXml(node);

            TriggerEvents events = 0;
            XmlAttribute triggerAttribute = node.Attributes["triggers"];

            if (triggerAttribute == null)
            {
                throw new ArgumentException("A triggers value must be specified");
            }

            if (Enum.TryParse(triggerAttribute.Value, true, out events))
            {
                this.TriggerEvents = events;
            }
            else
            {
                throw new ArgumentException("The trigger value is unknown or unsupported");
            }
        }
    }
}