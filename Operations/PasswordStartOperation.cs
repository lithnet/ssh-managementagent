// -----------------------------------------------------------------------
// <copyright file="PasswordStartOperation.cs" company="Lithnet">
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
    /// A password set or change start operation
    /// </summary>
    public class PasswordStartOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the PasswordStartOperation class
        /// </summary>
        public PasswordStartOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PasswordStartOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public PasswordStartOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
