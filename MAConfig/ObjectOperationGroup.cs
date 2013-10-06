// -----------------------------------------------------------------------
// <copyright file="ObjectOperationGroup.cs" company="Lithnet">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// A group of operations for a specific object class
    /// </summary>
    public class ObjectOperationGroup
    {
        /// <summary>
        /// Initializes a new instance of the ObjectOperationGroup class
        /// </summary>
        public ObjectOperationGroup()
        {
            this.ObjectOperations = new List<OperationBase>();
        }

        /// <summary>
        /// Initializes a new instance of the ObjectOperationGroup class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ObjectOperationGroup(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the class of object that this operation group applies to
        /// </summary>
        public string ObjectClass { get; private set; }

        /// <summary>
        /// Gets the list of object operations that apply to this object class
        /// </summary>
        public List<OperationBase> ObjectOperations { get; private set; }

        /// <summary>
        /// Populates the object from an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            XmlAttribute classAttribute = node.Attributes["object-class"];

            if (classAttribute == null)
            {
                throw new ArgumentNullException("The object class must be specified");
            }

            if (MASchema.Objects.Contains(classAttribute.Value))
            {
                this.ObjectClass = classAttribute.Value;
            }
            else 
            {
                throw new NoSuchObjectTypeException(classAttribute.Value);
            }

            foreach (XmlNode operationNode in node.ChildNodes)
            {
                if (operationNode.Name == "global-operation" || operationNode.Name == "object-operation")
                {
                    this.ObjectOperations.Add(OperationBase.CreateObjectOperationFromXmlNode(operationNode));
                }
            }

            if (MAConfig.Capabilities.DeltaImport)
            {
                if (!this.ObjectOperations.Any(t => t is ImportDeltaOperation))
                {
                    throw new ArgumentException("A delta import operation must be defined for all objects if the delta capabilities is enabled in the configuration file");                        
                }
            }
        }
    }
}
