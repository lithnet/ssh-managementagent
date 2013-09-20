// -----------------------------------------------------------------------
// <copyright file="ExportEndOperation.cs" company="Lithnet">
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
    /// An export end operation
    /// </summary>
    public class ExportEndOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportEndOperation class
        /// </summary>
        public ExportEndOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExportEndOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ExportEndOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
