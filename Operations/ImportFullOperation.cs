// -----------------------------------------------------------------------
// <copyright file="ImportFullOperation.cs" company="Lithnet">
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
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// A full import operation
    /// </summary>
    public class ImportFullOperation : ImportOperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportFullOperation class
        /// </summary>
        public ImportFullOperation()
            : base()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the ImportFullOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportFullOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
