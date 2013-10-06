// -----------------------------------------------------------------------
// <copyright file="ExportModifyOperation.cs" company="Lithnet">
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
