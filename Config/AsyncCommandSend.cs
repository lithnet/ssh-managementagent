// -----------------------------------------------------------------------
// <copyright file="AsyncCommandSend.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
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
    /// TODO: Update summary.
    /// </summary>
    public class AsyncCommandSend
    {
        /// <summary>
        /// Initializes a new instance of the AsyncCommandSend class
        /// </summary>
        public AsyncCommandSend()
        : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommandSend class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public AsyncCommandSend(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the command to execute
        /// </summary>
        public ValueDeclaration Command { get; private set; }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            this.Command = new ValueDeclaration(node);
        }
    }
}
