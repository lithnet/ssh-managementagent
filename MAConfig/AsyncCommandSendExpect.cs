// -----------------------------------------------------------------------
// <copyright file="AsyncCommandSendExpect.cs" company="Lithnet">
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
    /// Defines an asynchronously processed command launched when a particular value is obtained
    /// </summary>
    public class AsyncCommandSendExpect : AsyncCommandSend
    {
        /// <summary>
        /// Initializes a new instance of the AsyncCommandSendExpect class
        /// </summary>
        public AsyncCommandSendExpect()
        : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AsyncCommandSendExpect class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public AsyncCommandSendExpect(XmlNode node)
            : base(node)
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the value to expect before the command is sent
        /// </summary>
        public ValueDeclaration Expect { get; private set; }

        /// <summary>
        /// Gets the time, in seconds, to wait for the expected value before aborting the operation
        /// </summary>
        public int Timeout { get; private set; }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute expectAttribute = node.Attributes["expect"];
            XmlAttribute timeoutAttribute = node.Attributes["timeout"];

            if (expectAttribute == null || string.IsNullOrWhiteSpace(expectAttribute.Value))
            {
                throw new ArgumentNullException("The expect attribute must be present and not null");
            }
            else
            {
                this.Expect = new ValueDeclaration(expectAttribute.Value);
            }

            if (timeoutAttribute == null || string.IsNullOrWhiteSpace(timeoutAttribute.Value))
            {
                throw new ArgumentNullException("The timeout attribute must be present and not null");
            }

            int timeout = 0;

            if (int.TryParse(timeoutAttribute.Value, out timeout))
            {
                this.Timeout = timeout;
            }
            else
            {
                throw new ArgumentException("The value provided for timeout could not be converted to an integer");
            }
        }
    }
}
