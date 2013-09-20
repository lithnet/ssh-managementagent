// -----------------------------------------------------------------------
// <copyright file="IRule.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
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
    public interface IRule
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
