// -----------------------------------------------------------------------
// <copyright file="ImportFullStartOperation.cs" company="Lithnet">
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
    /// A full import start operation
    /// </summary>
    public class ImportFullStartOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportFullStartOperation class
        /// </summary>
        public ImportFullStartOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportFullStartOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportFullStartOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
