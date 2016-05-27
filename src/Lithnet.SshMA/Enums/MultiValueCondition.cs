// -----------------------------------------------------------------------
// <copyright file="MultiValueCondition.cs" company="Lithnet">
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
    /// Conditions to apply to a rule that is applied to a multi-valued attribute
    /// </summary>
    public enum MultiValueCondition
    {
        /// <summary>
        /// Any of the values of a multi-valued attribute can match the specified operator
        /// </summary>
        Any,

        /// <summary>
        /// All of the values of a multi-valued attribute must match the specified operator
        /// </summary>
        All,

        /// <summary>
        /// Only one of the values of a multi-valued attribute can match the specified operator
        /// </summary>
        One,

        /// <summary>
        /// None of the values of a multi-valued attribute can match the specified operator
        /// </summary>
        None
    }
}
