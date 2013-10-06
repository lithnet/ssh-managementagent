// -----------------------------------------------------------------------
// <copyright file="RuleBase.cs" company="Lithnet">
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
    /// An abstract base class used for the implementation of constructor rules
    /// </summary>
    public abstract class RuleBase : IEvaluableRuleObject
    {
        /// <summary>
        /// Initializes a new instance of the RuleBase class
        /// </summary>
        public RuleBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RuleBase class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public RuleBase(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the ID for this rule
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Creates a new RuleSet object from an XML representation
        /// </summary>
        /// <param name="node">The XML xmlNode representing the object</param>
        /// <returns>A new object that inherits from the RuleBase class</returns>
        public static RuleBase CreateRuleFromXmlNode(XmlNode node)
        {
            XmlAttribute typeAttribute = node.Attributes["xsi:type"];

            if (typeAttribute == null)
            {
                throw new ArgumentNullException("The rule type was not specified");
            }

            string typeName = typeAttribute.Value.Replace("sshma:rule-", string.Empty);
            Type t = Type.GetType("Lithnet.SshMA." + typeName);

            if (t == null)
            {
                if (t == null)
                {
                    throw new ArgumentException("The rule type '" + typeAttribute.Value + " 'is invalid");
                }
            }

            if (!t.IsSubclassOf(typeof(RuleBase)))
            {
                throw new ArgumentException("The rule type '" + typeAttribute.Value + " 'is invalid");
            }

            return Activator.CreateInstance(t, new object[] { node }) as RuleBase;
        }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange that applies to this rule</param>
        /// <returns>A value indicating whether the rule was successful or not</returns>
        public abstract bool Evaluate(CSEntryChange csentry);

        /// <summary>
        /// Populates the RuleSet object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        protected virtual void FromXml(XmlNode node)
        {
            XmlAttribute idAttribute = node.Attributes["id"];

            if (idAttribute == null || string.IsNullOrWhiteSpace(idAttribute.Value))
            {
                throw new ArgumentNullException("The rule must have an ID");
            }
            else
            {
                this.Id = idAttribute.Value;
                MAConfig.Rules.Add(this);
            }
        }
    }
}
