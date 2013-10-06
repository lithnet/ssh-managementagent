// -----------------------------------------------------------------------
// <copyright file="SyncCommand.cs" company="Lithnet">
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
    /// Represents an SSH command containing variables to be expanded
    /// </summary>
    public class SyncCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the SyncCommand class
        /// </summary>
        public SyncCommand()
            : base()
        {
            this.SuccessCodes = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the SyncCommand class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public SyncCommand(XmlNode node)
            : base(node)
        {
            this.SuccessCodes = new List<int>();
            this.FromXml(node);
        }

        /// <summary>
        /// Gets a list of exit codes that determine a successful execution of a command
        /// </summary>
        public List<int> SuccessCodes { get; private set; }

        /// <summary>
        /// Gets the command to execute
        /// </summary>
        public ValueDeclaration Command { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether the result for this operation contains objects to extract
        /// </summary>
        public bool HasObjects { get; private set; }
        
        /// <summary>
        /// Gets the multi-valued reference to iterate through for this command
        /// </summary>
        public MASchemaAttribute ForEachAttribute { get; private set; }

        /// <summary>
        /// Gets the value modification type to apply to the for each multi-valued attribute
        /// </summary>
        public ValueModificationType ForEachValueModificationType { get; private set; }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute successCodes = node.Attributes["success-codes"];
            XmlAttribute forEachAttribute = node.Attributes["for-each"];
            XmlAttribute valueModificationTypeAttribute = node.Attributes["value-modification"];
            XmlAttribute hasObjectsAttribute = node.Attributes["result-has-objects"];

            if (hasObjectsAttribute != null && !string.IsNullOrWhiteSpace(hasObjectsAttribute.Value))
            {
                bool hasObjects = false;
                if (bool.TryParse(hasObjectsAttribute.Value, out hasObjects))
                {
                    this.HasObjects = hasObjects;
                }
                else
                {
                    throw new ArgumentException("The boolean value could not be parsed: " + hasObjectsAttribute.Value);
                }
            }

            if (forEachAttribute != null && !string.IsNullOrWhiteSpace(forEachAttribute.Value))
            {
                MASchema.ThrowOnMissingAttribute(forEachAttribute.Value);
                this.ForEachAttribute = MASchema.Attributes[forEachAttribute.Value];
            }

            if (successCodes != null)
            {
                string[] split = successCodes.Value.Split(',');
                foreach (string value in split)
                {
                    int result = 0;

                    if (int.TryParse(value, out result))
                    {
                        this.SuccessCodes.Add(result);
                    }
                    else
                    {
                        throw new ArgumentException("The specified success code value cannot be converted to an integer: " + value);
                    }
                }
            }

            if (this.SuccessCodes.Count == 0)
            {
                this.SuccessCodes.Add(0);
            }

            if (valueModificationTypeAttribute == null || string.IsNullOrWhiteSpace(valueModificationTypeAttribute.Value))
            {
                this.ForEachValueModificationType = ValueModificationType.Unconfigured;
            }
            else
            {
                ValueModificationType valueModificationType = ValueModificationType.Unconfigured;
                if (Enum.TryParse(valueModificationTypeAttribute.Value, true, out valueModificationType))
                {
                    this.ForEachValueModificationType = valueModificationType;
                }
                else
                {
                    throw new ArgumentException("The value modification type is unknown: " + valueModificationTypeAttribute.Value);
                }
            }

            this.Command = new ValueDeclaration(node);
        }
    }
}
