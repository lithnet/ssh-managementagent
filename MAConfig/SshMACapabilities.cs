// -----------------------------------------------------------------------
// <copyright file="SshMACapabilities.cs" company="Lithnet">
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
    /// Defines a capability set for the management agent
    /// </summary>
    public class SshMACapabilities
    {
        /// <summary>
        /// Initializes a new instance of the SshMACapabilities class
        /// </summary>
        public SshMACapabilities()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SshMACapabilities class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public SshMACapabilities(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets a value indicating whether this management agent supports delta imports
        /// </summary>
        public bool DeltaImport { get; private set; }

        /// <summary>
        /// Gets the object update mode supported by this management agent
        /// </summary>
        public MAExportType ObjectUpdateMode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the management agent supports 'replace' operations in place of separate 'delete' and 'add' operations
        /// </summary>
        public bool DeleteAddAsReplace { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the management agent supports object renames
        /// </summary>
        public bool ObjectRenameAllowed { get; private set; }

        /// <summary>
        /// Populates the object based on an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlNode child = node.SelectSingleNode("delta-import");
            bool result = false;

            if (bool.TryParse(child.InnerText, out result))
            {
                this.DeltaImport = result;
            }
            else
            {
                throw new ArgumentException("Unknown value specified for the delta-import node: " + child.InnerText);
            }

            child = node.SelectSingleNode("delete-add-as-replace");
            result = false;

            if (bool.TryParse(child.InnerText, out result))
            {
                this.DeleteAddAsReplace = result;
            }
            else
            {
                throw new ArgumentException("Unknown value specified for the delete-add-as-replace node: " + child.InnerText);
            }

            child = node.SelectSingleNode("object-rename-allowed");
            result = false;

            if (bool.TryParse(child.InnerText, out result))
            {
                this.ObjectRenameAllowed = result;
            }
            else
            {
                throw new ArgumentException("Unknown value specified for the object-rename-allowed node: " + child.InnerText);
            }

            child = node.SelectSingleNode("object-update-mode");
            MAExportType exportType = MAExportType.AttributeReplace;

            if (Enum.TryParse(child.InnerText, true, out exportType))
            {
                this.ObjectUpdateMode = exportType;
            }
            else
            {
                throw new ArgumentException("Unknown value specified for the object-update-mode node: " + child.InnerText);
            }
        }
    }
}
