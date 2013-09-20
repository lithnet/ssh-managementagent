// -----------------------------------------------------------------------
// <copyright file="ImportFullEndOperation.cs" company="Lithnet">
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
    /// A full import end operation
    /// </summary>
    public class ImportFullEndOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportFullEndOperation class
        /// </summary>
        public ImportFullEndOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ImportFullEndOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportFullEndOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
