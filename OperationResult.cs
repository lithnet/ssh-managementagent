// -----------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Lithnet">
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
    using Renci.SshNet;

    /// <summary>
    /// The result of an operation
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the OperationResult class
        /// </summary>
        public OperationResult()
        {
            this.ExecutedCommands = new List<SshCommand>();
            this.ExecutedCommandsWithObjects = new List<SshCommand>();
        }

        /// <summary>
        /// Gets a list of commands executed during the operation
        /// </summary>
        public List<SshCommand> ExecutedCommands { get; private set; }

        /// <summary>
        /// Gets or sets the operation that contained the command set that was executed
        /// </summary>
        public OperationBase ExecutedOperation { get; set; }

        /// <summary>
        /// Gets a list of commands executed that have objects to extract
        /// </summary>
        public List<SshCommand> ExecutedCommandsWithObjects { get; private set; }
    }
}
