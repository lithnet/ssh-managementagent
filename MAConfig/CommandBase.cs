// -----------------------------------------------------------------------
// <copyright file="CommandBase.cs" company="Lithnet">
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
    /// Represents an SSH command
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the CommandBase class
        /// </summary>
        public CommandBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandBase class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public CommandBase(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the rule or rules that apply to this command
        /// </summary>
        public IEvaluableRuleObject RuleSet { get; private set; }

        /// <summary>
        /// Evaluates the rule sets that apply to this command to determine if it should be executed
        /// </summary>
        /// <param name="csentry">The CSEntryChange to evaluate the rules against</param>
        /// <returns>A value indicating if the command should be executed</returns>
        public bool ShouldExecute(CSEntryChange csentry)
        {
            if (csentry == null && this.RuleSet != null)
            {
                throw new ArgumentNullException("csentry");
            }

            if (this.RuleSet == null)
            {
                return true;
            }
            else
            {
                return this.RuleSet.Evaluate(csentry);
            }
        }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute idRefattribute = node.Attributes["rule-id"];

            if (idRefattribute != null && !string.IsNullOrWhiteSpace(idRefattribute.Value))
            {
                if (!MAConfig.Rules.Contains(idRefattribute.Value))
                {
                    throw new ArgumentException(string.Format("The referenced rule '{0}' does not exist", idRefattribute.Value));
                }
                else
                {
                    this.RuleSet = MAConfig.Rules[idRefattribute.Value];
                }
            }
        }
    }
}
