// -----------------------------------------------------------------------
// <copyright file="AsyncCommandSend.cs" company="Lithnet">
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
