// -----------------------------------------------------------------------
// <copyright file="TriggerEvents.cs" company="Lithnet">
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
