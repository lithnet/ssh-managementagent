// -----------------------------------------------------------------------
// <copyright file="MAParameterNames.cs" company="Lithnet">
// The Microsoft Public License (Ms-PL) governs use of the accompanying software. 
// If you use the software, you accept this license. 
// If you do not accept the license, do not use the software.
// http://go.microsoft.com/fwlink/?LinkID=131993// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains a list of parameter names used in the MA
    /// </summary>
    public static class MAParameterNames
    {
        /// <summary>
        /// The host name parameter name
        /// </summary>
        public const string HostName = "Host name";

        /// <summary>
        /// The username parameter name
        /// </summary>
        public const string Username = "Username";

        /// <summary>
        /// The port parameter name
        /// </summary>
        public const string Port = "Port";

        /// <summary>
        /// The configuration file parameter name
        /// </summary>
        public const string MAConfigurationFile = "MA configuration file";

        /// <summary>
        /// The private key file parameter name
        /// </summary>
        public const string PrivateKeyFile = "Private key file";

        /// <summary>
        /// The private key passphrase parameter name
        /// </summary>
        public const string PasswordOrPassphrase = "Password or private key passphrase";

        /// <summary>
        /// The authentication mode parameter name
        /// </summary>
        public const string AuthenticationMode = "Authentication mode";

        /// <summary>
        /// The debug mode parameter name
        /// </summary>
        public const string DebugEnabled = "Debug mode";

        /// <summary>
        /// The debugger launch on exception parameter name
        /// </summary>
        public const string LaunchDebuggerOnException = "Launch debugger on exception";

        /// <summary>
        /// The log file path parameter name
        /// </summary>
        public const string LogPath = "Log path";
    }
}
