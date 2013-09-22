// -----------------------------------------------------------------------
// <copyright file="ValueDeclaration.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Microsoft.MetadirectoryServices.DetachedObjectModel;

    /// <summary>
    /// Represents a declaratively constructed value
    /// </summary>
    public class ValueDeclaration
    {
        /// <summary>
        /// The text of the declaration before processing
        /// </summary>
        private string declarationText;

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        public ValueDeclaration()
        {
            this.AttributeReferences = new List<ReferencedAttribute>();
        }

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        /// <param name="node">The Xml representation of the object</param>
        public ValueDeclaration(XmlNode node)
            : this()
        {
            this.FromXml(node);
        }

        /// <summary>
        /// Initializes a new instance of the ValueDeclaration class
        /// </summary>
        /// <param name="declaration">The declaration string to use for this value declaration</param>
        public ValueDeclaration(string declaration)
            : this()
        {
            this.DeclarationText = declaration;
        }

        /// <summary>
        /// Gets the raw text of the declaration before processing
        /// </summary>
        public string DeclarationText
        {
            get
            {
                return this.declarationText;
            }

            private set
            {
                this.declarationText = value;
                this.ExtractAttributeReferences(value);
            }
        }

        /// <summary>
        /// Gets the list of attributes referenced in the DeclarationText
        /// </summary>
        public List<ReferencedAttribute> AttributeReferences { get; private set; }

        /// <summary>
        /// Gets the value declaration with all attributes values expanded
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="oldPassword">The old password value</param>
        /// <param name="newPassword">The new password value</param>
        /// <param name="obfuscatePasswordFields">A value indicating if passwords should be obfuscated in the resulting output. Typically used for logging purposes</param>
        /// <returns>The DeclarationText value with all references to attributes replaced with their actual values</returns>
        public string ExpandDeclaration(CSEntry csentry, string oldPassword, string newPassword, bool obfuscatePasswordFields)
        {
            if (csentry == null)
            {
                return this.DeclarationText;
            }

            string constructedValue = this.DeclarationText;

            foreach (ReferencedAttribute reference in this.AttributeReferences)
            {
                switch (reference.AttributeName)
                {
                    case "dn":
                        constructedValue = constructedValue.Replace(reference.Declaration, this.GetReferencedDNComponent(csentry, reference));
                        break;

                    case "newpassword":
                        constructedValue = constructedValue.Replace(reference.Declaration, obfuscatePasswordFields ? "#hiddennewpassword#" : newPassword);
                        break;

                    case "oldpassword":
                        constructedValue = constructedValue.Replace(reference.Declaration, obfuscatePasswordFields ? "#hiddenoldpassword#" : oldPassword);
                        break;
                }
            }

            return constructedValue;
        }

        /// <summary>
        /// Gets the value declaration with all attributes values expanded
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="throwOnMissingAttribute">Sets a value indicating whether an exception should be thrown if an attribute is not present in the CSEntryChange, otherwise replaces the declaration with an empty string</param>
        /// <returns>The DeclarationText value with all references to attributes replaced with their actual values</returns>
        public string ExpandDeclaration(CSEntryChange csentry, bool throwOnMissingAttribute)
        {
            if (csentry == null)
            {
                return this.DeclarationText;
            }

            string constructedValue = this.DeclarationText;

            foreach (ReferencedAttribute attribute in this.AttributeReferences)
            {
                constructedValue = constructedValue.Replace(attribute.Declaration, this.GetSourceAttributeValue(csentry, attribute, throwOnMissingAttribute) ?? string.Empty);
            }

            return constructedValue;
        }

        /// <summary>
        /// Gets an enumeration of expanded attribute value declarations for a multi-valued attribute
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="multiValuedAttribute">The multi-valued attribute to expand</param>
        /// <param name="modificationType">Specifies which value modification types to expand during the enumeration</param>
        /// <returns>An enumeration of expanded declaration strings</returns>
        public IEnumerable<string> ExpandDeclarationWithMultiValued(CSEntryChange csentry, MASchemaAttribute multiValuedAttribute, ValueModificationType modificationType)
        {
            if (csentry == null)
            {
                yield return this.DeclarationText;
            }

            string constructedValue = this.DeclarationText;

            foreach (ReferencedAttribute reference in this.AttributeReferences.Where(t => t.AttributeName != multiValuedAttribute.Name))
            {
                if (MASchema.GetAttributeType(reference.AttributeName) == AttributeType.Reference)
                {
                    constructedValue = constructedValue.Replace(reference.Declaration, this.GetReferencedDNComponent(csentry, reference));
                }
                else
                {
                    constructedValue = constructedValue.Replace(reference.Declaration, this.GetSourceAttributeValue(csentry, reference, false) ?? string.Empty);
                }
            }

            ReferencedAttribute mvattribute = this.AttributeReferences.FirstOrDefault(t => t.AttributeName == multiValuedAttribute.Name);

            if (mvattribute != null && csentry.AttributeChanges.Contains(multiValuedAttribute.Name))
            {
                AttributeChange change = csentry.AttributeChanges[multiValuedAttribute.Name];

                foreach (string value in this.GetSourceAttributeValues(change, mvattribute, modificationType))
                {
                    yield return constructedValue.Replace(mvattribute.Declaration, value);
                }
            }
            else
            {
                yield return constructedValue;
            }

            yield break;
        }

        /// <summary>
        /// Retrieves the reference value from the specified object
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="reference">The ReferencedAttribute to obtain the value for</param>
        /// <param name="throwOnMissingAttribute">Sets a value indicating whether an exception should be thrown if an attribute is not present in the CSEntryChange, otherwise replaces the declaration with an empty string</param>
        /// <returns>The value of the specified reference on the source object</returns>
        private string GetSourceAttributeValue(CSEntryChange csentry, ReferencedAttribute reference, bool throwOnMissingAttribute)
        {
            string sourceAttributeValue = string.Empty;

            if (MASchema.GetAttributeType(reference.AttributeName) == AttributeType.Reference)
            {
                return this.GetReferencedDNComponent(csentry, reference);
            }
            
            if (csentry.AttributeChanges.Contains(reference.AttributeName))
            {
                ValueChange valueChange = csentry.AttributeChanges[reference.AttributeName].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add);

                if (valueChange != null)
                {
                    sourceAttributeValue = reference.PreReferenceString + valueChange.Value.ToString() + reference.PostReferenceString;
                }
            }
            else
            {
                if (throwOnMissingAttribute)
                {
                    throw new AttributeNotPresentException(reference.AttributeName);
                }
            }

            return sourceAttributeValue;
        }

        /// <summary>
        /// Retrieves the source attribute values from the specified attribute change
        /// </summary>
        /// <param name="attributeChange">The attribute change to enumerate the values from</param>
        /// <param name="reference">The declaration of the referenced attribute</param>
        /// <param name="modificationType">The value modification type to enumerate</param>
        /// <returns>An enumeration of expanded string values</returns>
        private IEnumerable<string> GetSourceAttributeValues(AttributeChange attributeChange, ReferencedAttribute reference, ValueModificationType modificationType)
        {
            IEnumerable<ValueChange> valueChanges = attributeChange.ValueChanges.Where(t => t.ModificationType == (modificationType == ValueModificationType.Unconfigured ? ValueModificationType.Add : modificationType));

            foreach (ValueChange valueChange in valueChanges)
            {
                if (attributeChange.DataType == AttributeType.Reference)
                {
                    yield return reference.PreReferenceString + this.GetReferencedDNComponent(valueChange.Value.ToString(), reference) + reference.PostReferenceString;
                }
                else
                {
                    yield return reference.PreReferenceString + valueChange.Value.ToString() + reference.PostReferenceString;
                }
            }

            yield break;
        }

        /// <summary>
        /// Gets the specific DN component referenced in the ReferencedAttribute object
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="attribute">The ReferencedAttribute containing the DN and extraction parameters</param>
        /// <returns>A string containing the components of the DN referenced in the ReferenceAttribute object</returns>
        private string GetReferencedDNComponent(CSEntry csentry, ReferencedAttribute attribute)
        {
            return this.GetReferencedDNComponent(csentry.DN.ToString(), attribute);
        }

        /// <summary>
        /// Gets the specific DN component referenced in the ReferencedAttribute object
        /// </summary>
        /// <param name="csentry">The source object</param>
        /// <param name="attribute">The ReferencedAttribute containing the DN and extraction parameters</param>
        /// <returns>A string containing the components of the DN referenced in the ReferenceAttribute object</returns>
        private string GetReferencedDNComponent(CSEntryChange csentry, ReferencedAttribute attribute)
        {
            return this.GetReferencedDNComponent(csentry.DN, attribute);
        }

        /// <summary>
        /// Gets the specific DN component referenced in the ReferencedAttribute object
        /// </summary>
        /// <param name="dn">The DN string of an object</param>
        /// <param name="attribute">The ReferencedAttribute containing the DN and extraction parameters</param>
        /// <returns>A string containing the components of the DN referenced in the ReferenceAttribute object</returns>
        private string GetReferencedDNComponent(string dn, ReferencedAttribute attribute)
        {
            string valueToInsert = string.Empty;

            if (attribute.Count == 0)
            {
                valueToInsert = dn;
            }
            else
            {
                string[] split = DetachedUtils.SplitDN(dn);

                if (attribute.Count - 1 > split.Length)
                {
                    throw new ArgumentException("The referenced attribute specifies a DN component that doesnt exist: " + attribute.Declaration);
                }

                string rdn = split[attribute.Count - 1];

                if (attribute.Modifier == "+")
                {
                    valueToInsert = rdn;
                }
                else if (attribute.Modifier == "$")
                {
                    int indexOfEquals = rdn.IndexOf("=");
                    valueToInsert = rdn.Remove(0, indexOfEquals + 1);
                }
            }

            return valueToInsert;
        }
        
        /// <summary>
        /// Extracts the reference declarations referenced in the specified text
        /// </summary>
        /// <param name="value">The string to extract the attributes from</param>
        private void ExtractAttributeReferences(string value)
        {
            this.AttributeReferences.Clear();

            Regex regex = new Regex(@"\[(?<preText>[^\]\[]*?)\{((?<referenceAttributeName>\w+)->)?(?<attributeName>\w+)(\:(?<modifier>([^\d]))?(?<count>\d?))?\}(?<postText>.*?)\]|\{((?<referenceAttributeName>\w+)->)?(?<attributeName>\w+)(\:(?<modifier>([^\d]))?(?<count>\d?))?\}", RegexOptions.ExplicitCapture);
            MatchCollection matches = regex.Matches(value);

            foreach (Match match in matches)
            {
                ReferencedAttribute reference = new ReferencedAttribute();

                if (match.Groups["attributeName"] != null)
                {
                    MASchema.ThrowOnMissingAttribute(match.Groups["attributeName"].Value);
                    reference.AttributeName = match.Groups["attributeName"].Value;

                    if (match.Groups["count"] != null && !string.IsNullOrWhiteSpace(match.Groups["count"].Value))
                    {
                        if (MASchema.GetAttributeType(reference.AttributeName) != AttributeType.Reference)
                        {
                            throw new ArgumentException("A count value is only valid on a 'dn' or reference attribute");
                        }

                        reference.Count = int.Parse(match.Groups["count"].Value);
                    }

                    if (match.Groups["modifier"] != null && !string.IsNullOrWhiteSpace(match.Groups["modifier"].Value))
                    {
                        if (MASchema.GetAttributeType(reference.AttributeName) != AttributeType.Reference)
                        {
                            throw new ArgumentException("A modifier value is only valid on a 'dn' or reference attribute");
                        }

                        reference.Modifier = match.Groups["modifier"].Value;
                    }

                    if (match.Groups["referenceAttributeName"] != null && !string.IsNullOrWhiteSpace(match.Groups["referenceAttributeName"].Value))
                    {
                        throw new NotSupportedException("This MA does not support referenced attribute declarations");
                    }

                    if (match.Groups["preText"] != null)
                    {
                        reference.PreReferenceString = match.Groups["preText"].Value;
                    }

                    if (match.Groups["postText"] != null)
                    {
                        reference.PostReferenceString = match.Groups["postText"].Value;
                    }

                    reference.Declaration = match.Value;
                    this.AttributeReferences.Add(reference);
                }
            }
        }

        /// <summary>
        /// Populates the object based on an XML representation
        /// </summary>
        /// <param name="node">The XML representation of the object</param>
        private void FromXml(XmlNode node)
        {
            this.DeclarationText = node.InnerText;
        }
    }
}
