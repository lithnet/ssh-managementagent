// -----------------------------------------------------------------------
// <copyright file="RuleGroup.cs" company="Lithnet">
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
    /// Represents a group of rules
    /// </summary>
    public class RuleGroup : IEvaluableRuleObject
    {
        /// <summary>
        /// The XML representation of this object
        /// </summary>
        private XmlNode xmlNode;

        /// <summary>
        /// Initializes a new instance of the RuleGroup class
        /// </summary>
        public RuleGroup()
        {
            this.RuleGroups = new List<RuleGroup>();
            this.Rules = new List<RuleBase>();
        }

        /// <summary>
        /// Initializes a new instance of the RuleGroup class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public RuleGroup(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the logical operator used to apply to this rule group
        /// </summary>
        public RuleGroupOperator Operator { get; private set; }

        /// <summary>
        /// Gets the list of rules that are within this group
        /// </summary>
        public List<RuleBase> Rules { get; private set; }

        /// <summary>
        /// Gets the list of rule groups within this group
        /// </summary>
        public List<RuleGroup> RuleGroups { get; private set; }

        /// <summary>
        /// Gets the ID for this rule
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Evaluates the rule group against the specified object
        /// </summary>
        /// <param name="csentry">The object to run the rules against</param>
        /// <returns>A value indicating whether the group and its child rules matched against the object</returns>
        public bool Evaluate(CSEntryChange csentry)
        {
            bool lastResult = false;

            if (this.RuleGroups.Count == 0 && this.Rules.Count == 0)
            {
                return true;
            }

            foreach (RuleBase rule in this.Rules)
            {
                bool thisResult = rule.Evaluate(csentry);

                if (thisResult && this.Operator == RuleGroupOperator.Or)
                {
                    return true;
                }

                if (!thisResult && this.Operator == RuleGroupOperator.And)
                {
                    return false;
                }

                if (thisResult && this.Operator == RuleGroupOperator.None)
                {
                    return false;
                }

                if (thisResult && this.Operator == RuleGroupOperator.Xor && lastResult)
                {
                    return false;
                }

                lastResult = thisResult;
            }

            foreach (RuleGroup ruleGroup in this.RuleGroups)
            {
                bool thisResult = ruleGroup.Evaluate(csentry);

                if (thisResult && this.Operator == RuleGroupOperator.Or)
                {
                    return true;
                }

                if (!thisResult && this.Operator == RuleGroupOperator.And)
                {
                    return false;
                }

                if (thisResult && this.Operator == RuleGroupOperator.None)
                {
                    return false;
                }

                if (thisResult && this.Operator == RuleGroupOperator.Xor && lastResult)
                {
                    return false;
                }

                lastResult = thisResult;
            }

            if (!lastResult && this.Operator == RuleGroupOperator.Or)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds any referenced rules to the collection
        /// </summary>
        public void AddReferencedRulesToCollection()
        {
            XmlNodeList nodes = this.xmlNode.SelectNodes("rule-ref");

            if (nodes != null)
            {
                foreach (XmlNode child in nodes)
                {
                    XmlAttribute ruleIdAttribute = child.Attributes["rule-id"];

                    if (ruleIdAttribute == null || string.IsNullOrWhiteSpace(ruleIdAttribute.Value))
                    {
                        continue;
                    }

                    IEvaluableRuleObject rule = MAConfig.Rules[ruleIdAttribute.Value];

                    if (rule is RuleBase)
                    {
                        this.Rules.Add(rule as RuleBase);
                    }
                    else if (rule is RuleGroup)
                    {
                        if (rule != this)
                        {
                            this.RuleGroups.Add(rule as RuleGroup);
                        }
                    }
                }
            }

            if (this.RuleGroups == null)
            {
                return;
            }
            else
            {
                foreach (RuleGroup group in this.RuleGroups)
                {
                    group.AddReferencedRulesToCollection();
                }
            }
        }

        /// <summary>
        /// Populates the object based on an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            this.xmlNode = node;

            XmlAttribute operatorAttribute = node.Attributes["operator"];
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

            if (operatorAttribute == null)
            {
                this.Operator = RuleGroupOperator.And;
            }
            else
            {
                RuleGroupOperator ruleGroupOperator = RuleGroupOperator.And;
                if (Enum.TryParse(operatorAttribute.Value, true, out ruleGroupOperator))
                {
                    this.Operator = ruleGroupOperator;
                }
                else
                {
                    throw new ArgumentException(string.Format("The rule group operator '{0}' is unknown", operatorAttribute.Value));
                }
            }

            XmlNodeList nodes = node.SelectNodes("rule");

            if (nodes == null)
            {
                return;
            }
            else
            {
                foreach (XmlNode child in nodes)
                {
                    this.Rules.Add(RuleBase.CreateRuleFromXmlNode(child));
                }
            }

            nodes = node.SelectNodes("rule-group");

            if (nodes == null)
            {
                return;
            }
            else
            {
                foreach (XmlNode child in nodes)
                {
                    this.RuleGroups.Add(new RuleGroup(child));
                }
            }
        }
    }
}