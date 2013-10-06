// -----------------------------------------------------------------------
// <copyright file="ExportAddOperation.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// An export add operation
    /// </summary>
    public class ExportAddOperation : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ExportAddOperation class
        /// </summary>
        public ExportAddOperation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExportAddOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ExportAddOperation(XmlNode node)
            : base(node)
        {
        }
    }
}
