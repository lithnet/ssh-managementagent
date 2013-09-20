// -----------------------------------------------------------------------
// <copyright file="ExportDeleteOperation.cs" company="Lithnet">
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
    /// An export delete operation
    /// </summary>
    public class ExportDeleteOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportDeleteOperation class
        /// </summary>
        public ExportDeleteOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExportDeleteOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ExportDeleteOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
