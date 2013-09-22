// -----------------------------------------------------------------------
// <copyright file="XmlSchema.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Provides access to the MA Xml schema
    /// </summary>
    public static class XmlSchema
    {
        /// <summary>
        /// Gets the IO Stream of the Xml schema data
        /// </summary>
        /// <returns>A System.IO.Stream object</returns>
        public static Stream GetSchemaStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Lithnet.SshMA.MAConfig.Lithnet.SSHMA.xsd");
        }

        /// <summary>
        /// Gets an XmlReader containing the schema for this application
        /// </summary>
        /// <returns>An XmlRead object</returns>
        public static XmlReader GetSchema()
        {
            return XmlReader.Create(GetSchemaStream());
        }

        /// <summary>
        /// Gets all the schemas that apply to this application
        /// </summary>
        /// <returns>A XmlSchemaSet object</returns>
        public static XmlSchemaSet GetSchemas()
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, GetSchema());
            return schemas;
        }
    }
}
