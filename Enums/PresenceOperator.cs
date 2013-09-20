// -----------------------------------------------------------------------
// <copyright file="PresenceOperator.cs" company="Lithnet">
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
    /// A list of possible presence comparison operators
    /// </summary>
    public enum PresenceOperator
    {
        /// <summary>
        /// The value must not be null, empty, or missing
        /// </summary>
        IsPresent,

        /// <summary>
        /// The value must be null, empty, or missing
        /// </summary>
        NotPresent
    }
}
