// -----------------------------------------------------------------------
// <copyright file="MAParameterValidator.cs" company="Lithnet">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using System.IO;
    using Renci.SshNet;

    /// <summary>
    /// Exposes the parameters provided by the configuration page user interfaces
    /// </summary>
    public class MAParameters
    {
        private KeyedCollection<string, ConfigParameter> config;

        public MAParameters(KeyedCollection<string, ConfigParameter> config)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets a value indicating whether debug mode is enabled
        /// </summary>
        public bool DebugEnabled
        {
            get
            {
                if (this.config.Contains(MAParameterNames.DebugEnabled))
                {
                    return this.config[MAParameterNames.DebugEnabled].Value == "1" ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the location of the log file
        /// </summary>
        public string LogPath => this.config[MAParameterNames.LogPath].Value;

        /// <summary>
        /// Gets the host name
        /// </summary>
        public string HostName => this.config[MAParameterNames.HostName].Value;

        /// <summary>
        /// Gets the user name
        /// </summary>
        public string Username => this.config[MAParameterNames.Username].Value;

        /// <summary>
        /// Gets the authentication mode to use
        /// </summary>
        public string AuthenticationMode => this.config[MAParameterNames.AuthenticationMode].Value;

        /// <summary>
        /// Gets the port
        /// </summary>
        public string Port => this.config[MAParameterNames.Port].Value;

        /// <summary>
        /// Gets the path to this MA's configuration file
        /// </summary>
        public string MAConfigurationFilePath
        {
            get
            {
                string file = this.config[MAParameterNames.MAConfigurationFile].Value;

                if (System.IO.Path.IsPathRooted(file))
                {
                    return file;
                }
                else
                {
                    return System.IO.Path.Combine(Utils.ExtensionsDirectory, file);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the debugger should be launched when an exception occurs
        /// </summary>
        public bool LaunchDebuggerOnException => this.config[MAParameterNames.LaunchDebuggerOnException].Value == "1" ? true : false;


        /// <summary>
        /// Gets a PrivateKeyFile object from the path specified in the configuration parameters
        /// </summary>
        /// <returns>A PrivateKeyFile object</returns>
        public PrivateKeyFile GetPrivateKeyFile()
        {
            PrivateKeyFile file;

            if (!this.config.Contains(MAParameterNames.PasswordOrPassphrase))
            {
                FileStream stream = new FileStream(this.config[MAParameterNames.PrivateKeyFile].Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                file = new PrivateKeyFile(
                    stream);
            }
            else
            {
                FileStream stream = new FileStream(this.config[MAParameterNames.PrivateKeyFile].Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                file = new PrivateKeyFile(
                    stream,
                    this.GetPassword());
            }

            return file;
        }

        /// <summary>
        /// Gets the password from the specified parameter
        /// </summary>
        /// <returns>The unencrypted password</returns>
        public string GetPassword()
        {
            string passphrase = string.Empty;

            if (this.config.Contains(MAParameterNames.PasswordOrPassphrase))
            {
                ConfigParameter parameter = this.config[MAParameterNames.PasswordOrPassphrase];

                if (parameter.IsEncrypted)
                {
                    passphrase = parameter.SecureValue.ToUnsecureString();
                }
                else
                {
                    passphrase = parameter.Value;
                }
            }

            return passphrase;
        }
    }
}
