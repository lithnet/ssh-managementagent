// -----------------------------------------------------------------------
// <copyright file="ExportModifyOperation.cs" company="Lithnet">
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
    /// An export modify operation
    /// </summary>
    public class ExportModifyOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportModifyOperation class
        /// </summary>
        public ExportModifyOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExportModifyOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ExportModifyOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
