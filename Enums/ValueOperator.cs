// -----------------------------------------------------------------------
// <copyright file="ValueOperator.cs" company="Lithnet">
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
    /// A list of possible comparison operators
    /// </summary>
    public enum ValueOperator
    {
        /// <summary>
        /// No comparison type is specified
        /// </summary>
        None,

        /// <summary>
        /// The values being compared must be the same
        /// </summary>
        Equals,

        /// <summary>
        /// The values being compared must not be the same 
        /// </summary>
        NotEquals,
        
        /// <summary>
        /// The value being compared must be greater than the value it is being compared to
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The value being compared must be less than the value it is being compared to
        /// </summary>
        LessThan,

        /// <summary>
        /// The value being compared must be greater than or equal to the value it is being compared to
        /// </summary>
        GreaterThanOrEq,

        /// <summary>
        /// The value being compared must be less than or equal to the value it is being compared to
        /// </summary>
        LessThanOrEq,

        /// <summary>
        /// The value must not be null, empty, or missing
        /// </summary>
        IsPresent,

        /// <summary>
        /// The value must be null, empty, or missing
        /// </summary>
        NotPresent,

        /// <summary>
        /// The value being compared must be found in the value it is being compared to
        /// </summary>
        Contains,

        /// <summary>
        /// The value being compared must not be found in the value it is being compared to
        /// </summary>
        NotContains,

        /// <summary>
        /// The value being compared must start with the value it is being compared to
        /// </summary>
        StartsWith,

        /// <summary>
        /// The value being compared must end with the value it is being compared to
        /// </summary>
        EndsWith,

        /// <summary>
        /// The values are compared with a logical AND
        /// </summary>
        And,

        /// <summary>
        /// The values are compared with a logical OR
        /// </summary>
        Or,

        /// <summary>
        /// The selector should take the largest value found
        /// </summary>
        Smallest,

        /// <summary>
        /// The selector should take the smallest value found
        /// </summary>
        Largest
    }
}
