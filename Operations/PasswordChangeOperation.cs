// -----------------------------------------------------------------------
// <copyright file="PasswordChangeOperation.cs" company="Lithnet">
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
    /// A password change operation
    /// </summary>
    public class PasswordChangeOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the PasswordChangeOperation class
        /// </summary>
        public PasswordChangeOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PasswordChangeOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public PasswordChangeOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
