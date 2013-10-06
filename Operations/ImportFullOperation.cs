// -----------------------------------------------------------------------
// <copyright file="ImportFullOperation.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993
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
