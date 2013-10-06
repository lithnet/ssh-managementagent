// -----------------------------------------------------------------------
// <copyright file="OperationBase.cs" company="Lithnet">
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
    /// An abstract base class defining the common elements of Operation objects
    /// </summary>
    public abstract class OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the OperationBase class
        /// </summary>
        public OperationBase()
        {
            this.Commands = new List<CommandBase>();
        }

        /// <summary>
        /// Initializes a new instance of the OperationBase class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public OperationBase(XmlNode node)
        : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets a list of command sets for this operation
        /// </summary>
        public List<CommandBase> Commands { get; private set; }

        /// <summary>
        /// Creates a new Operation object from an XML representation
        /// </summary>
        /// <param name="node">The XML rootNode representing the object</param>
        /// <returns>A new object that inherits from the OperationBase class</returns>
        public static OperationBase CreateObjectOperationFromXmlNode(XmlNode node)
        {
            XmlAttribute typeAttribute = node.Attributes["xsi:type"];

            if (typeAttribute == null)
            {
                throw new ArgumentNullException("The rule type was not specified");
            }

            string typeName = typeAttribute.Value.Replace("sshma:object-operation-", string.Empty);
            typeName = typeName.Replace("sshma:global-operation-", string.Empty);
            typeName = typeName + "Operation";

            Type t = Type.GetType("Lithnet.SshMA." + typeName);

            if (t == null)
            {
                if (t == null)
                {
                    throw new ArgumentException("The operation type '" + typeAttribute.Value + " 'is invalid");
                }
            }

            if (!t.IsSubclassOf(typeof(OperationBase)))
            {
                throw new ArgumentException("The operation type '" + typeAttribute.Value + " 'is invalid");
            }

            return Activator.CreateInstance(t, new object[] { node }) as OperationBase;
        }
        
        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        protected virtual void FromXml(XmlNode node)
        {
            foreach (XmlNode commandNode in node.SelectSingleNode("commands").ChildNodes)
            {
                if (commandNode.Name == "command")
                {
                    this.Commands.Add(new SyncCommand(commandNode));
                }
                else if (commandNode.Name == "async-command")
                {
                    this.Commands.Add(new AsyncCommand(commandNode));
                }
            }
        }
    }
}
