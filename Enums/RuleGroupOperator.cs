// -----------------------------------------------------------------------
// <copyright file="RuleGroupOperator.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The type of comparison to make between rules in a group to determine if the group result is successful
    /// </summary>
    public enum RuleGroupOperator
    {
        /// <summary>
        /// No rules are allowed to pass for the group to pass
        /// </summary>
        None,

        /// <summary>
        /// All rules in the group must pass for the group to pass
        /// </summary>
        And,

        /// <summary>
        /// At least one rule in the group must pass in order for the group to pass
        /// </summary>
        Or,

        /// <summary>
        /// Exactly one rule in the group must pass for the group to pass
        /// </summary>
        Xor
    }
}
