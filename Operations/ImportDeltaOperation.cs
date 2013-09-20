// -----------------------------------------------------------------------
// <copyright file="ImportDeltaOperation.cs" company="Lithnet">
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
    /// A delta import operation
    /// </summary>
    public class ImportDeltaOperation : ImportOperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportDeltaOperation class
        /// </summary>
        public ImportDeltaOperation()
            : base()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the ImportDeltaOperation class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportDeltaOperation(XmlNode node)
            : base(node)
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the name of the change type capture group from the import regular expression
        /// </summary>
        public string ChangeTypeCaptureGroupName { get; private set; }

        /// <summary>
        /// Gets action to take when an unexpected modification type is encountered
        /// </summary>
        public UnexpectedModificationTypeAction UnexpectedModTypeAction { get; private set; }
        
        /// <summary>
        /// Gets the string used to identify a replace operation
        /// </summary>
        public string ModificationTypeReplaceRegEx { get; private set; }

        /// <summary>
        /// Gets the string used to identify an add operation
        /// </summary>
        public string ModificationTypeAddRegEx { get; private set; }

        /// <summary>
        /// Gets the string used to identify an delete operation
        /// </summary>
        public string ModificationTypeDeleteRegEx { get; private set; }

        /// <summary>
        /// Populates the object from its XML representation
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            XmlNode child = node.SelectSingleNode("import-mapping/modification-type-mappings");

            if (child == null)
            {
                throw new ArgumentNullException("The delta import object operation must have a modification-type-mappings node");
            }

            XmlAttribute captureGroupNameattribute = child.Attributes["capture-group-name"];
            XmlAttribute unexpectedModificationTypeActionAttribute = child.Attributes["unexpected-modification-type-action"];

            if (captureGroupNameattribute == null || string.IsNullOrWhiteSpace(captureGroupNameattribute.Value))
            {
                throw new ArgumentNullException("The modification-type-mappings node must have a capture-group-name attribute");
            }
            else
            {
                this.ChangeTypeCaptureGroupName = captureGroupNameattribute.Value;
            }

            if (unexpectedModificationTypeActionAttribute == null || string.IsNullOrWhiteSpace(unexpectedModificationTypeActionAttribute.Value))
            {
                throw new ArgumentNullException("The modification-type-mappings node must have an unexpected-modification-type-action attribute");
            }
            else
            {
                UnexpectedModificationTypeAction action = UnexpectedModificationTypeAction.Ignore;

                if (Enum.TryParse(unexpectedModificationTypeActionAttribute.Value, true, out action))
                {
                    this.UnexpectedModTypeAction = action;
                }
                else
                {
                    throw new ArgumentException("The value provided for the attribute 'unexpected-modification-type-action' was unknown or unsupported: " + unexpectedModificationTypeActionAttribute.Value);
                }
            }
            
            XmlNode modificationTypeAddNode = child.SelectSingleNode("modification-type-add");

            if (modificationTypeAddNode == null || string.IsNullOrWhiteSpace(modificationTypeAddNode.InnerText))
            {
                throw new ArgumentNullException("The modification-type-mappings node must have a modification-type-add node");
            }
            else
            {
                this.ModificationTypeAddRegEx = modificationTypeAddNode.InnerText;
            }

            XmlNode modificationTypeReplaceNode = child.SelectSingleNode("modification-type-replace");

            if (modificationTypeReplaceNode == null || string.IsNullOrWhiteSpace(modificationTypeReplaceNode.InnerText))
            {
                throw new ArgumentNullException("The modification-type-mappings node must have a modification-type-replace node");
            }
            else
            {
                this.ModificationTypeReplaceRegEx = modificationTypeReplaceNode.InnerText;
            }

            XmlNode modificationTypeDeleteNode = child.SelectSingleNode("modification-type-replace");

            if (modificationTypeDeleteNode == null || string.IsNullOrWhiteSpace(modificationTypeDeleteNode.InnerText))
            {
                throw new ArgumentNullException("The modification-type-mappings node must have a modification-type-delete node");
            }
            else
            {
                this.ModificationTypeDeleteRegEx = modificationTypeDeleteNode.InnerText;
            }
        }
    }
}
