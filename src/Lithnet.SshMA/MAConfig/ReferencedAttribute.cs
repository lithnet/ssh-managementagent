// -----------------------------------------------------------------------
// <copyright file="ReferencedAttribute.cs" company="Lithnet">
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

    /// <summary>
    /// Represents an reference referenced in a value declaration
    /// </summary>
    public class ReferencedAttribute
    {
        /// <summary>
        /// Gets or sets the raw reference declaration 
        /// </summary>
        public string Declaration { get; set; }

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// Gets or sets the reference name that references another object
        /// </summary>
        public string ReferencedObject { get; set; }

        /// <summary>
        /// Gets or sets the value or character count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the modifier value
        /// </summary>
        public string Modifier { get; set; }
        
        /// <summary>
        /// Gets or sets the text that precedes the attribute declaration when an optional declaration is used
        /// </summary>
        public string PreReferenceString { get; set; }

        /// <summary>
        /// Gets or sets the text that follows the attribute declaration when an optional declaration is used
        /// </summary>
        public string PostReferenceString { get; set; }

        /// <summary>
        /// Gets a value indicating whether the declaration has optional pre and post declaration text
        /// </summary>
        public bool IsOptional
        {
            get
            {
                return this.Declaration.StartsWith("[") && this.Declaration.EndsWith("]");
            }
        }
    }
}
