// -----------------------------------------------------------------------
// <copyright file="CSEntryImport.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
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
        /// <returns>A list of CSEntryChange objects</returns>
        public static IEnumerable<CSEntryChange> GetObjects(KeyedCollection<string, SchemaType> schemaTypes)
        {
            List<CSEntryChange> csentries = new List<CSEntryChange>();
            
            foreach (ObjectOperationGroup group in MAConfig.OperationGroups.Where(t => schemaTypes.Contains(t.ObjectClass)))
            {
                OperationBase operation = group.ObjectOperations.FirstOrDefault(t => t is ImportFullOperation);

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
                throw new FailedSearchException("No search results were returned");
            }

            foreach (SshCommand command in result.ExecutedCommandsWithObjects)
            {
                MatchCollection matchCollection = Regex.Matches(command.Result, operation.ImportMapping, RegexOptions.ExplicitCapture | RegexOptions.Multiline);

                foreach (Match match in matchCollection)
                {
                    CSEntryChange csentry = CSEntryChange.Create();
                    csentry.ObjectModificationType = GetObjectChangeTypeFromMatchCollection(matchCollection, operation);
                    csentry.ObjectType = schemaType.Name;

                    foreach (MASchemaAttribute attribute in MASchema.Objects[schemaType.Name].Attributes.Where(t => schemaType.Attributes.Contains(t.Name)))
                    {
                        Group group = match.Groups[attribute.Name];
                        if (!group.Success)
                        {
                            continue;
                        }

                        IList<object> values = GetAttributeValuesFromMatch(attribute, match, operation);

                        if (values != null && values.Count > 0)
                        {
                            IList<object> transformedValues = TransformAttributeValues(attribute, operation, values);
                            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, transformedValues));
                        }
                    }

                    csentry.DN = MASchema.Objects[schemaType.Name].DNFormat.ExpandDeclaration(csentry, true);
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("entry-dn", csentry.DN));

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
                newValues.Add(Regex.Replace(value.ToString(), transformation.RegexFind, transformation.RegexReplace));
            }

            return newValues;
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
            List<object> values = new List<object>();
               
            Group group = match.Groups[attribute.Name];
            if (!group.Success)
            {
                return null;
            }

            if (attribute.IsMultiValued)
            {
                MultiValueExtract mapping = operation.MultiValuedAttributeMappings.FirstOrDefault(t => t.Attribute.Name == attribute.Name);

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
        /// Gets the object modification type from a regular expression match collection
        /// </summary>
        /// <param name="matchCollection">The collection of matches containing the change type</param>
        /// <param name="operation">The operation being performed</param>
        /// <returns>An object modification type value</returns>
        private static ObjectModificationType GetObjectChangeTypeFromMatchCollection(MatchCollection matchCollection, ImportOperationBase operation)
        {
            if (operation is ImportFullOperation)
            {
                return ObjectModificationType.Add;
            }

            ImportDeltaOperation deltaOperation = operation as ImportDeltaOperation;

            foreach (Match match in matchCollection)
            {
                Group group = match.Groups[deltaOperation.ChangeTypeCaptureGroupName];

                if (!group.Success)
                {
                    continue;
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