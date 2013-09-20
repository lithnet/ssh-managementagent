// -----------------------------------------------------------------------
// <copyright file="PasswordSetOperation.cs" company="Lithnet">
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
    /// A password set operation
    /// </summary>
    public class PasswordSetOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the PasswordSetOperation class
        /// </summary>
        public PasswordSetOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PasswordSetOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public PasswordSetOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
