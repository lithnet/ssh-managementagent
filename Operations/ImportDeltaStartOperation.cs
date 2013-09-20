// -----------------------------------------------------------------------
// <copyright file="ImportDeltaStartOperation.cs" company="Lithnet">
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
    /// A delta import start operation
    /// </summary>
    public class ImportDeltaStartOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportDeltaStartOperation class
        /// </summary>
        public ImportDeltaStartOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportDeltaStartOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportDeltaStartOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
