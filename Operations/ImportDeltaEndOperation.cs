// -----------------------------------------------------------------------
// <copyright file="ImportDeltaEndOperation.cs" company="Lithnet">
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
    /// A delta import end operation
    /// </summary>
    public class ImportDeltaEndOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportDeltaEndOperation class
        /// </summary>
        public ImportDeltaEndOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportDeltaEndOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportDeltaEndOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
