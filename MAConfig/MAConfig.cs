// -----------------------------------------------------------------------
// <copyright file="MAConfig.cs" company="Lithnet">
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
    using System.Xml.Linq;
    using System.Xml.Schema;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Loads configuration settings from the configuration xml file
    /// </summary>
    public class MAConfig
    {
        /// <summary>
        /// Initializes static members of the MAConfig class
        /// </summary>
        static MAConfig()
        {
            MAConfig.OperationGroups = new ObjectOperationGroupKeyedCollection();
            MAConfig.GlobalOperations = new List<OperationBase>();
            MAConfig.Rules = new RuleKeyedCollection();
            MAConfig.RuleHierarchy = new List<IRule>();
        }
        
        /// <summary>
        /// Gets a list of operations groups
        /// </summary>
        public static ObjectOperationGroupKeyedCollection OperationGroups { get; private set; }

        /// <summary>
        /// Gets a list of global operations
        /// </summary>
        public static List<OperationBase> GlobalOperations { get; private set; }

        /// <summary>
        /// Gets the capabilities from this MA
        /// </summary>
        public static SshMACapabilities Capabilities { get; private set; }

        /// <summary>
        /// Gets a keyed collection of rules
        /// </summary>
        public static RuleKeyedCollection Rules { get; private set; }

        /// <summary>
        /// Gets a hierarchical representation of the rules
        /// </summary>
        public static List<IRule> RuleHierarchy { get; private set; }

        /// <summary>
        /// Loads in the configuration settings from the specified XML file
        /// </summary>
        /// <param name="fileName">The path to the configuration file</param>
        public static void Load(string fileName)
        {
            ValidateXml(fileName);
            XmlDocument configFile = new XmlDocument();
            configFile.Load(fileName);
            XmlNode rootNode = configFile.DocumentElement;

            ReadMACapabilitiesNode(rootNode);
            ReadSchemaNode(rootNode);
            ReadRulesNode(rootNode);
            ReadGlobalOperationsNode(rootNode);
            ReadOperationsGroupNode(rootNode);
        }
        
        /// <summary>
        /// Validates the XML using the specified schema file.
        /// </summary>
        /// <param name="xmlFilePath">The XML file name</param>
        private static void ValidateXml(string xmlFilePath)
        {
            var xdoc = XDocument.Load(xmlFilePath);
            xdoc.Validate(XmlSchema.GetSchemas(), null);
        }

        /// <summary>
        /// Reads the schema rootNode from the configuration file
        /// </summary>
        /// <param name="rootNode">The root rootNode of the Xml document</param>
        private static void ReadSchemaNode(XmlNode rootNode)
        {
            XmlNode node = rootNode.SelectSingleNode("schema");
            MASchema.FromXml(node);
        }

        /// <summary>
        /// Reads the ma-capabilities node from the Xml file
        /// </summary>
        /// <param name="rootNode">The root xmlNode of the Xml file</param>
        private static void ReadMACapabilitiesNode(XmlNode rootNode)
        {
            XmlNode node = rootNode.SelectSingleNode("ma-capabilities");
            Capabilities = new SshMACapabilities(node);
        }

        /// <summary>
        /// Reads the global-operations xmlNode from the Xml file
        /// </summary>
        /// <param name="rootNode">The root xmlNode of the Xml file</param>
        private static void ReadGlobalOperationsNode(XmlNode rootNode)
        {
            GlobalOperations.Clear();

            foreach (XmlNode globalOperation in rootNode.SelectNodes("global-operations/global-operation"))
            {
                GlobalOperations.Add(OperationBase.CreateObjectOperationFromXmlNode(globalOperation));
            }
        }

        /// <summary>
        /// Reads the object-operations xmlNode from the Xml file
        /// </summary>
        /// <param name="rootNode">The root xmlNode of the Xml file</param>
        private static void ReadOperationsGroupNode(XmlNode rootNode)
        {
            OperationGroups.Clear();

            foreach (XmlNode objectOperationGroup in rootNode.SelectNodes("object-operations"))
            {
                OperationGroups.Add(new ObjectOperationGroup(objectOperationGroup));
            }
        }

        /// <summary>
        /// Reads the rules xmlNode from the Xml file
        /// </summary>
        /// <param name="rootNode">The root xmlNode of the Xml file</param>
        private static void ReadRulesNode(XmlNode rootNode)
        {
            Rules.Clear();
            RuleHierarchy.Clear();

            foreach (XmlNode ruleNode in rootNode.SelectNodes("rules/*"))
            {
                if (ruleNode.Name == "rule")
                {
                    MAConfig.RuleHierarchy.Add(RuleBase.CreateRuleFromXmlNode(ruleNode));
                }
                else if (ruleNode.Name == "rule-group")
                {
                    MAConfig.RuleHierarchy.Add(new RuleGroup(ruleNode));
                }
            }

            foreach (RuleGroup group in Rules.Where(t => t is RuleGroup))
            {
                group.AddReferencedRulesToCollection();
            }
        }
    }
}