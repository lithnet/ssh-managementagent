// -----------------------------------------------------------------------
// <copyright file="CSEntryImport.cs" company="Lithnet">
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
    using System.Text.RegularExpressions;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Renci.SshNet;

    /// <summary>
    /// Provides methods for importing connector space objects to the FIM Sync Service
    /// </summary>
    public class CSEntryImport
    {
        /// <summary>
        /// Gets the objects required for import
        /// </summary>
        /// <param name="schemaTypes">The object classes to import</param>
        /// <param name="operationType">The type of import operation</param>
        /// <returns>A list of CSEntryChange objects</returns>
        public static IEnumerable<CSEntryChange> GetObjects(KeyedCollection<string, SchemaType> schemaTypes, OperationType operationType)
        {
            List<CSEntryChange> csentries = new List<CSEntryChange>();

            foreach (ObjectOperationGroup group in MAConfig.OperationGroups.Where(t => schemaTypes.Contains(t.ObjectClass)))
            {
                OperationBase operation;
                if (operationType == OperationType.Delta)
                {
                    operation = group.ObjectOperations.FirstOrDefault(t => t is ImportDeltaOperation);
                }
                else
                {
                    operation = group.ObjectOperations.FirstOrDefault(t => t is ImportFullOperation);
                }

                if (operation == null)
                {
                    throw new ExtensibleExtensionException("No import operation was defined for the object type " + group.ObjectClass);
                }

                OperationResult result = SshConnection.ExecuteOperation(operation);
                csentries.AddRange(GetCSEntryChanges(result, schemaTypes[group.ObjectClass]));
            }

            return csentries;
        }

        /// <summary>
        /// Creates CSEntryChange objects for each result returned from the import operation
        /// </summary>
        /// <param name="result">The result of the import operation</param>
        /// <param name="schemaType">The definition of the type of object to get the CSEntryChanges for</param>
        /// <returns>An enumeration of CSEntryChange objects</returns>
        public static IEnumerable<CSEntryChange> GetCSEntryChanges(OperationResult result, SchemaType schemaType)
        {
            ImportOperationBase operation = result.ExecutedOperation as ImportOperationBase;

            if (result.ExecutedCommandsWithObjects.Count == 0)
            {
                throw new FailedSearchException("No commands were executed that contained import results");
            }

            foreach (SshCommand command in result.ExecutedCommandsWithObjects)
            {
                MatchCollection matchCollection = Regex.Matches(command.Result, operation.ImportMapping, RegexOptions.ExplicitCapture | RegexOptions.Multiline);

                foreach (Match match in matchCollection)
                {
                    CSEntryChange csentry = CSEntryChange.Create();
                    ObjectModificationType modifcationType = GetObjectChangeTypeFromMatchCollection(match, operation);

                    if (modifcationType == ObjectModificationType.None)
                    {
                        Logger.WriteLine("Discarding object due to invalid modification type: " + match.Value);
                        continue;
                    }
                    else
                    {
                        csentry.ObjectModificationType = modifcationType;
                    }

                    csentry.ObjectType = schemaType.Name;

                    if (modifcationType == ObjectModificationType.Delete)
                    {
                        // We need to create a temporary CSEntryChange here because a CSEntryChange with an
                        // object modification type of 'delete' cannot contain attribute changes
                        // This stops us from being able to construct a DN for the CSEntryChange
                        CSEntryChange temporaryChange = CSEntryChange.Create();
                        temporaryChange.ObjectModificationType = ObjectModificationType.Update;
                        temporaryChange.ObjectType = csentry.ObjectType;
                        PopulateCSEntryWithMatches(schemaType, operation, match, temporaryChange);
                        csentry.DN = MASchema.Objects[schemaType.Name].DNFormat.ExpandDeclaration(temporaryChange, true);
                        csentry.AnchorAttributes.Add(AnchorAttribute.Create("entry-dn", csentry.DN));
                    }
                    else
                    {
                        PopulateCSEntryWithMatches(schemaType, operation, match, csentry);
                        csentry.DN = MASchema.Objects[schemaType.Name].DNFormat.ExpandDeclaration(csentry, true);
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("entry-dn", csentry.DN));
                    }

                    if (string.IsNullOrEmpty(csentry.DN))
                    {
                        Logger.WriteLine("Discarding object from result as no DN could be derived: " + match.Value, LogLevel.Debug);
                        continue;
                    }

                    if (operation.IsFiltered(csentry))
                    {
                        Logger.WriteLine("Filtering object: " + csentry.DN, LogLevel.Debug);
                    }
                    else
                    {
                        yield return csentry;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the CSEntryChange with the data found in the regular expression match
        /// </summary>
        /// <param name="schemaType">The type of schema object</param>
        /// <param name="operation">The operation being performed</param>
        /// <param name="match">The regular expression match containing the object with values to extract</param>
        /// <param name="csentry">The CSEntryChange to populate</param>
        private static void PopulateCSEntryWithMatches(SchemaType schemaType, ImportOperationBase operation, Match match, CSEntryChange csentry)
        {
            foreach (MASchemaAttribute attribute in MASchema.Objects[schemaType.Name].Attributes.Where(t => schemaType.Attributes.Contains(t.Name) && t.Operation != AttributeOperation.ExportOnly))
            {
                IList<object> values = new List<object>();
                Group group = match.Groups[attribute.Name];

                if (!group.Success)
                {
                    foreach (MultiValueExtract mapping in operation.MultiValuedAttributeMappings)
                    {
                        if (mapping.Attribute.Name == attribute.Name)
                        {
                            group = match.Groups[mapping.CaptureGroupName];
                            if (group.Success)
                            {
                                values = values.Concat(GetAttributeValuesFromMatch(attribute, mapping, match, operation)).ToList<object>();
                            }
                        }
                    }
                }
                else
                {
                    values = GetAttributeValuesFromMatch(attribute, match, operation);
                }

                if (values != null && values.Count > 0)
                {
                    IList<object> transformedValues = TransformAttributeValues(attribute, operation, values);
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, transformedValues));
                }
            }
        }

        /// <summary>
        /// Applies a set of transforms to the specified attribute
        /// </summary>
        /// <param name="attribute">The definition of the attribute to transform</param>
        /// <param name="operation">The operation being performed</param>
        /// <param name="values">The values to transform</param>
        /// <returns>A list of transformed values</returns>
        private static IList<object> TransformAttributeValues(MASchemaAttribute attribute, ImportOperationBase operation, IList<object> values)
        {
            AttributeTransformation transformation = operation.AttributeTransformations.FirstOrDefault(t => t.Attribute.Name == attribute.Name);

            if (transformation == null)
            {
                return values;
            }

            List<object> newValues = new List<object>();

            foreach (object value in values)
            {
                string newValue = Regex.Replace(value.ToString(), transformation.RegexFind, transformation.RegexReplace);
                newValues.Add(newValue);
            }

            return newValues;
        }

        /// <summary>
        /// Gets a list of attribute values from a regular expression match
        /// </summary>
        /// <param name="attribute">The attribute to return the values for</param>
        /// <param name="mapping">The multivalued extract mapping, or null, if this is a single valued attribute</param>
        /// <param name="match">The regular expression match to extract the values from</param>
        /// <param name="operation">The current operation being performed</param>
        /// <returns>A list of values for the specified attribute</returns>
        private static IList<object> GetAttributeValuesFromMatch(MASchemaAttribute attribute, MultiValueExtract mapping, Match match, ImportOperationBase operation)
        {
            List<object> values = new List<object>();
            string captureGroupName = mapping == null ? attribute.Name : mapping.CaptureGroupName;

            Group group = match.Groups[captureGroupName];
            if (!group.Success)
            {
                return null;
            }

            if (attribute.IsMultiValued)
            {
                if (mapping == null)
                {
                    values.Add(group.Value);
                    return values;
                }
                else
                {
                    MatchCollection matchCollection = Regex.Matches(group.Value, mapping.MappingRegEx, RegexOptions.Multiline);
                    foreach (Match subMatch in matchCollection)
                    {
                        values.Add(subMatch.Value);
                    }
                }
            }
            else
            {
                values.Add(group.Value);
            }

            return values;
        }

        /// <summary>
        /// Extracts a list of attribute values from a regular expression match
        /// </summary>
        /// <param name="attribute">The definition of the attribute to extract</param>
        /// <param name="match">The regular expression match containing the values to extract</param>
        /// <param name="operation">The operation being performed</param>
        /// <returns>A list of attribute values extracted from the source</returns>
        private static IList<object> GetAttributeValuesFromMatch(MASchemaAttribute attribute, Match match, ImportOperationBase operation)
        {
            return GetAttributeValuesFromMatch(attribute, null, match, operation);
        }

        /// <summary>
        /// Gets the object modification type from a regular expression match collection
        /// </summary>
        /// <param name="match">The match containing the delta object entry</param>
        /// <param name="operation">The operation being performed</param>
        /// <returns>An object modification type value</returns>
        private static ObjectModificationType GetObjectChangeTypeFromMatchCollection(Match match, ImportOperationBase operation)
        {
            if (operation is ImportFullOperation)
            {
                return ObjectModificationType.Add;
            }

            ImportDeltaOperation deltaOperation = operation as ImportDeltaOperation;

            Group group = match.Groups[deltaOperation.ChangeTypeCaptureGroupName];

            if (!group.Success)
            {
                throw new ArgumentException("Capture group for change type not found");
            }

            if (Regex.IsMatch(group.Value, deltaOperation.ModificationTypeAddRegEx))
            {
                return ObjectModificationType.Add;
            }
            else if (Regex.IsMatch(group.Value, deltaOperation.ModificationTypeReplaceRegEx))
            {
                return ObjectModificationType.Replace;
            }
            else if (Regex.IsMatch(group.Value, deltaOperation.ModificationTypeDeleteRegEx))
            {
                return ObjectModificationType.Delete;
            }

            if (deltaOperation.UnexpectedModTypeAction == UnexpectedModificationTypeAction.Ignore)
            {
                return ObjectModificationType.None;
            }
            else
            {
                throw new ArgumentException("Unknown object modification type");
            }
        }
    }
}