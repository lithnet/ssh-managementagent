// -----------------------------------------------------------------------
// <copyright file="MASchemaObject.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Xml;

    /// <summary>
    /// Represents a defined schema object
    /// </summary>
    public class MASchemaObject
    {
        /// <summary>
        /// Initializes a new instance of the MASchemaObject class
        /// </summary>
        public MASchemaObject()
        {
            this.Attributes = new MASchemaAttributes();
            this.ResurrectionAttributes = new MASchemaAttributes();
        }

        /// <summary>
        /// Initializes a new instance of the MASchemaObject class
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        public MASchemaObject(XmlNode node)
            : this()
        {
            XmlAttribute objectClassAttribute = node.Attributes["objectclass"];

            if (objectClassAttribute == null || string.IsNullOrWhiteSpace(objectClassAttribute.Value))
            {
                throw new ArgumentException("The schema-entry element must have a 'name' attribute");
            }

            this.ObjectClass = objectClassAttribute.Value;

            XmlNode dnConstructor = node.SelectSingleNode("dn-format");
            this.DNFormat = new ValueDeclaration(dnConstructor);

            if (string.IsNullOrWhiteSpace(this.DNFormat.DeclarationText) || this.DNFormat.AttributeReferences.Count == 0)
            {
                throw new ArgumentNullException("A dn-format attribute must be specified and contain at least one attribute reference");
            }

            foreach (XmlNode child in node.SelectNodes("attributes/attribute"))
            {
                MASchema.ThrowOnMissingAttribute(child.InnerText);

                this.Attributes.Add(MASchema.Attributes[child.InnerText]);

                if (MASchema.Attributes[child.InnerText].CanResurrect)
                {
                    this.ResurrectionAttributes.Add(MASchema.Attributes[child.InnerText]);
                }
            }
        }

        /// <summary>
        /// Gets the DN format declaration
        /// </summary>
        public ValueDeclaration DNFormat { get; private set; }

        /// <summary>
        /// Gets the class of object defined in this schema entry
        /// </summary>
        public string ObjectClass { get; private set; }

        /// <summary>
        /// Gets the list of attributes that apply to this object
        /// </summary>
        public MASchemaAttributes Attributes { get; private set; }

        /// <summary>
        /// Gets the list of attributes that can be used to resurrect a deleted object of this type
        /// </summary>
        public MASchemaAttributes ResurrectionAttributes { get; private set; }
    }
}
