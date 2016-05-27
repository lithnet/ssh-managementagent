// -----------------------------------------------------------------------
// <copyright file="IEvaluableRuleObject.cs" company="Lithnet">
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
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// Defines and interface for rule objects
    /// </summary>
    public interface IEvaluableRuleObject
    {
        /// <summary>
        /// Gets the ID of the rule object
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Evaluates the rule
        /// </summary>
        /// <param name="csentry">The CSEntryChange to evaluate the rule against</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        bool Evaluate(CSEntryChange csentry);
    }
}
