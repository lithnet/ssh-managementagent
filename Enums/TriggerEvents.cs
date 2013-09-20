// -----------------------------------------------------------------------
// <copyright file="TriggerEvents.cs" company="Lithnet">
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
    /// Types of events that can trigger actions on a constructor
    /// </summary>
    [Flags]
    public enum TriggerEvents
    {
        /// <summary>
        /// The event is triggered on an add operation
        /// </summary>
        Add = 1,

        /// <summary>
        /// The event is triggered on an update operation
        /// </summary>
        Update = 2,

        /// <summary>
        /// The event is triggered on a delete operation
        /// </summary>
        Delete = 4,
    }
}
