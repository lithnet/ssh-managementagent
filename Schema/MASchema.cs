// -----------------------------------------------------------------------
// <copyright file="MASchema.cs" company="Lithnet">
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
    /// Provides access to the schema for this management agent
    /// </summary>
    public static class MASchema
    {
        /// <summary>
        /// Initializes static members of the MASchema class
        /// </summary>
        static MASchema()
        {
            BuiltInVariables = new List<string>();
            BuiltInVariables.Add("dn");
            BuiltInVariables.Add("newpassword");
            BuiltInVariables.Add("oldpassword");
        }

        /// <summary>
        /// Gets a list of MASchemaObjects
        /// </summary>
        public static MASchemaObjects Objects { get; private set; }

        /// <summary>
        /// Gets a list of MASchemaAttributes
        /// </summary>
        public static MASchemaAttributes Attributes { get; private set; }

        /// <summary>
        /// Gets a list of supported built-in variables
        /// </summary>
        public static List<string> BuiltInVariables { get; private set; }

        /// <summary>
        /// Throws an exception if the specified reference is not present in the schema
        /// </summary>
        /// <param name="attributeName">The name of the reference to check</param>
        public static void ThrowOnMissingAttribute(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new NoSuchAttributeException(attributeName);
            }

            if (!BuiltInVariables.Contains(attributeName))
            {
                if (!MASchema.Attributes.Contains(attributeName))
                {
                    throw new NoSuchAttributeException(attributeName);
                }
            }
        }

        /// <summary>
        /// Throws an exception if the specified reference is not present on the specified object class
        /// </summary>
        /// <param name="attributeName">The name of the reference to check</param>
        /// <param name="objectClass">The object class to which the reference should belong</param>
        public static void ThrowOnMissingAttribute(string attributeName, string objectClass)
        {
            if (!BuiltInVariables.Contains(attributeName))
            {
                if (string.IsNullOrWhiteSpace(attributeName) || !MASchema.Attributes.Contains(attributeName))
                {
                    throw new NoSuchAttributeException(attributeName);
                }

                if (string.IsNullOrWhiteSpace(objectClass) || !MASchema.Objects.Contains(objectClass))
                {
                    throw new NoSuchObjectTypeException(objectClass);
                }

                if (!MASchema.Objects[objectClass].Attributes.Contains(attributeName))
                {
                    throw new NoSuchAttributeInObjectTypeException(attributeName);
                }
            }
        }

        /// <summary>
        /// Throws an exception if the specified object class is not present in the schema
        /// </summary>
        /// <param name="objectClass">The name of the object class to check</param>
        public static void ThrowOnMissingObjectClass(string objectClass)
        {
            if (string.IsNullOrWhiteSpace(objectClass) || !MASchema.Objects.Contains(objectClass))
            {
                throw new NoSuchObjectTypeException(objectClass);
            }
        }

        /// <summary>
        /// Gets the type of a specified attribute
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The type of attribute</returns>
        public static AttributeType GetAttributeType(string attributeName)
        {
            ThrowOnMissingAttribute(attributeName);

            switch (attributeName)
            {
                case "dn":
                    return AttributeType.Reference;

                case "newpassword":
                case "oldpassword":
                    return AttributeType.String;

                default:
                    return Attributes[attributeName].Type;
            }
        }
        
        /// <summary>
        /// Reads the MASchema from the specified XmlNode
        /// </summary>
        /// <param name="node">The XmlNode from which to read the schema</param>
        public static void FromXml(XmlNode node)
        {
            MASchema.Attributes = new MASchemaAttributes(node);
            MASchema.Objects = new MASchemaObjects(node);
        }
    }
}
