// -----------------------------------------------------------------------
// <copyright file="PasswordEndOperation.cs" company="Lithnet">
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
    /// A password set or change end operation
    /// </summary>
    public class PasswordEndOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the PasswordEndOperation class
        /// </summary>
        public PasswordEndOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PasswordEndOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public PasswordEndOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
