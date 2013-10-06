// -----------------------------------------------------------------------
// <copyright file="UnexpectedModificationTypeAction.cs" company="Lithnet">
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
    /// The action to take when an unexpected modification type is encountered
    /// </summary>
    public enum UnexpectedModificationTypeAction
    {
        /// <summary>
        /// The record should be ignored and dropped from the import list
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// The record should trigger an error
        /// </summary>
        Error = 1
    }
}
