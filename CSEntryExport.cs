// -----------------------------------------------------------------------
// <copyright file="CSEntryExport.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Renci.SshNet;

    /// <summary>
    /// Provides methods for exporting connector space objects to the target system
    /// </summary>
    public static class CSEntryExport
    {
        /// <summary>
        /// Exports a single entry
        /// </summary>
        /// <param name="csentry">The entry to export</param>
        /// <param name="referenceRetryRequired">A value indicating whether the export need to be retried as one of more referenced objects were not present</param>
        /// <returns>A list of anchor attributes if the object was added to the target system, otherwise returns an empty list</returns>
        public static List<AttributeChange> PutExportEntry(CSEntryChange csentry, out bool referenceRetryRequired)
        {
            List<AttributeChange> anchorchanges = new List<AttributeChange>();
            referenceRetryRequired = false;

            switch (csentry.ObjectModificationType)
            {
                case ObjectModificationType.Add:
                    anchorchanges.Add(AttributeChange.CreateAttributeAdd("entry-dn", csentry.DN));
                    CSEntryExport.PerformCSEntryExportAdd(csentry);
                    break;

                case ObjectModificationType.Delete:
                    CSEntryExport.PerformCSEntryExportDelete(csentry);
                    break;

                case ObjectModificationType.None:
                    break;

                case ObjectModificationType.Update:
                case ObjectModificationType.Replace:
                    CSEntryExport.PerformCSEntryExportUpdate(csentry);
                    
                    if (HasDNChanged(csentry))
                    {
                        anchorchanges.Add(AttributeChange.CreateAttributeReplace("entry-dn", csentry.DN));
                    }

                    break;

                case ObjectModificationType.Unconfigured:
                    throw new InvalidOperationException("The object modification type 'unconfigured' is not supported for this object type");

                default:
                    throw new InvalidOperationException("The object modification type is unknown");
            }

            return anchorchanges;
        }

        /// <summary>
        /// Checks if the DN of an object has changed due to reference changes
        /// </summary>
        /// <param name="csentry">The object to evaluate</param>
        /// <returns>A value indicating if the DN has changed</returns>
        private static bool HasDNChanged(CSEntryChange csentry)
        {
            string dn;

            try
            {
                dn = ConstructDNFromCSEntryChange(csentry);
            }
            catch (AttributeNotPresentException)
            {
                return false;
            }

            if (dn != csentry.DN)
            {
                Logger.WriteLine("The dn has changed. Old value '{0}'. New value '{1}'", csentry.DN, dn);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Constructs the DN from the attributes in the CSEntryChange object
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <returns>A constructed DN for the specified object</returns>
        private static string ConstructDNFromCSEntryChange(CSEntryChange csentry)
        {
            return MASchema.Objects[csentry.ObjectType].DNFormat.ExpandDeclaration(csentry, true);
        }
        
        /// <summary>
        /// Adds a new object to the target system
        /// </summary>
        /// <param name="csentry">The CSEntryChange containing the new object and its attributes</param>
        private static void PerformCSEntryExportAdd(CSEntryChange csentry)
        {
            ObjectOperationGroup group = MAConfig.OperationGroups[csentry.ObjectType];

            if (group == null)
            {
                throw new InvalidOperationException("The object class does not have an object operation set");
            }

            OperationBase operation = group.ObjectOperations.FirstOrDefault(t => t is ExportAddOperation);
            
            if (operation == null)
            {
                throw new InvalidOperationException("An applicable add operation could not be found in the configuration file");
            }

            OperationResult result = SshConnection.ExecuteOperation(operation, csentry);

            if (result != null)
            {
                Logger.WriteLine("ExportAdd on object '{0}' returned: {1}", csentry.DN, result.ExecutedCommands.Last().Result);
            }
        }

        /// <summary>
        /// Deletes an object from the target system
        /// </summary>
        /// <param name="csentry">The CSEntryChange containing the new object and its attributes</param>
        private static void PerformCSEntryExportDelete(CSEntryChange csentry)
        {
            ObjectOperationGroup group = MAConfig.OperationGroups[csentry.ObjectType];

            if (group == null)
            {
                throw new InvalidOperationException("The object class does not have a delete operation");
            }

            OperationBase operation = group.ObjectOperations.FirstOrDefault(t => t is ExportDeleteOperation);
            OperationResult result = SshConnection.ExecuteOperation(operation, csentry);

            if (result != null)
            {
                Logger.WriteLine("ExportDelete on object '{0}' returned: {1}", csentry.DN, result.ExecutedCommands.Last().Result);
            }
        }

        /// <summary>
        /// Updates an object in the target system
        /// </summary>
        /// <param name="csentry">The CSEntryChange containing the new object and its attributes</param>
        private static void PerformCSEntryExportUpdate(CSEntryChange csentry)
        {
            ObjectOperationGroup group = MAConfig.OperationGroups[csentry.ObjectType];

            if (group == null)
            {
                throw new InvalidOperationException("The object class does not have a modify operation");
            }

            OperationBase operation = group.ObjectOperations.FirstOrDefault(t => t is ExportModifyOperation);
            OperationResult result = SshConnection.ExecuteOperation(operation, csentry);

            if (result != null)
            {
                Logger.WriteLine("ExportModify on object '{0}' returned: {1}", csentry.DN, result.ExecutedCommands.Last().Result);
            }
        }
    }
}