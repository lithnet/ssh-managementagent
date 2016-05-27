// -----------------------------------------------------------------------
// <copyright file="AsyncCommand.cs" company="Lithnet">
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

    /// <summary>
    /// Defines a group of asynchronously run commands
    /// </summary>
    public class AsyncCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the AsyncCommand class
        /// </summary>
        public AsyncCommand()
            : base()
        {
            this.Commands = new List<AsyncCommandSend>();
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommand class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public AsyncCommand(XmlNode node)
            : base(node)
        {
            this.Commands = new List<AsyncCommandSend>();
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the list of asynchronous commands associated with this object
        /// </summary>
        public List<AsyncCommandSend> Commands { get; private set; }

        /// <summary>
        /// Gets the expect value used to determine if the command was successful
        /// </summary>
        public AsyncCommandSendExpect ExpectSuccess { get; private set; }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlNode successNode = node.SelectSingleNode("success-when");

            if (successNode == null)
            {
                throw new ArgumentNullException("The async command must have a success-when node");
            }
            else
            {
                this.ExpectSuccess = new AsyncCommandSendExpect(successNode);
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "send")
                {
                    this.Commands.Add(new AsyncCommandSend(child));
                }
                else if (child.Name == "send-when")
                {
                    this.Commands.Add(new AsyncCommandSendExpect(child));
                }
            }
        }
    }
}
