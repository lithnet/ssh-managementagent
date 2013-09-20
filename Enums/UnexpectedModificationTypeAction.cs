// -----------------------------------------------------------------------
// <copyright file="UnexpectedModificationTypeAction.cs" company="Lithnet">
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
