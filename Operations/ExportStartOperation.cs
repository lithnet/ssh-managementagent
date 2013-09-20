// -----------------------------------------------------------------------
// <copyright file="ExportStartOperation.cs" company="Lithnet">
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
    /// An export start operation
    /// </summary>
    public class ExportStartOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportStartOperation class
        /// </summary>
        public ExportStartOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExportStartOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ExportStartOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
