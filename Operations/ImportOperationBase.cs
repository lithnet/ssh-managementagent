// -----------------------------------------------------------------------
// <copyright file="ImportOperationBase.cs" company="Lithnet">
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
    /// A base class for import operations
    /// </summary>
    public abstract class ImportOperationBase : OperationBase
    {
        /// <summary>
        /// Initializes a new instance of the ImportOperationBase class
        /// </summary>
        public ImportOperationBase()
            : base()
        {
            this.MultiValuedAttributeMappings = new List<MultiValueExtract>();
            this.Filters = new List<ObjectFilter>();
            this.AttributeTransformations = new List<AttributeTransformation>();
        }

        /// <summary>
        /// Initializes a new instance of the ImportOperationBase class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ImportOperationBase(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Gets the import mapping regular expression
        /// </summary>
        public string ImportMapping { get; private set; }

        /// <summary>
        /// Gets the list of multi-valued reference mappings
        /// </summary>
        public List<MultiValueExtract> MultiValuedAttributeMappings { get; private set; }

        /// <summary>
        /// Gets the list of object filters for this operation
        /// </summary>
        public List<ObjectFilter> Filters { get; private set; }

        /// <summary>
        /// Gets the list of reference transformations applicable to this operation
        /// </summary>
        public List<AttributeTransformation> AttributeTransformations { get; private set; }

        /// <summary>
        /// Compares an object to the filter rules in this operation
        /// </summary>
        /// <param name="csentry">The object to evaluate</param>
        /// <returns>A value indicating if the object should be filtered from the result set</returns>
        public bool IsFiltered(CSEntryChange csentry)
        {
            return this.Filters.Any(t => t.IsFiltered(csentry));
        }

        /// <summary>
        /// Populates the object from its XML representation
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        protected override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            
            XmlNode child = node.SelectSingleNode("import-mapping/object-extract");

            if (child == null || string.IsNullOrWhiteSpace(child.InnerText))
            {
                throw new ArgumentNullException("An object-extract must be defined");
            }
            else
            {
                this.ImportMapping = child.InnerText;
            }

            XmlNodeList entryFilters = node.SelectNodes("import-mapping/object-filters/object-filter");

            foreach (XmlNode entryFilter in entryFilters)
            {
                this.Filters.Add(new ObjectFilter(entryFilter));
            }

            XmlNodeList mappings = node.SelectNodes("import-mapping/multivalue-extracts/multivalue-extract");

            foreach (XmlNode mapping in mappings)
            {
                this.MultiValuedAttributeMappings.Add(new MultiValueExtract(mapping));
            }

            XmlNodeList transforms = node.SelectNodes("import-mapping/attribute-transformations/attribute-transformation");

            foreach (XmlNode transform in transforms)
            {
                this.AttributeTransformations.Add(new AttributeTransformation(transform));
            }

            if (!this.Commands.Any(t => t is SyncCommand && ((SyncCommand)t).HasObjects))
            {
                throw new ArgumentException("At least one import command must provide result objects by setting the 'result-has-objects' attribute to 'true'");
            }
        }
    }
}