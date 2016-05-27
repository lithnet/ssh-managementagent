// -----------------------------------------------------------------------
// <copyright file="MAParameterValidator.cs" company="Monash University">
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

    /// <summary>
    /// Validates the parameters provided by the configuration page user interfaces
    /// </summary>
    public static class MAParameterValidator
    {
        /// <summary>
        /// Validates the configuration file parameters, and ensures the configuration file is valid and can be opened
        /// </summary>
        /// <param name="configParameters">The configuration parameters from the user interface</param>
        /// <param name="page">The configuration page</param>
        /// <returns>A ParameterValidationResult containing the validation status</returns>
        public static ParameterValidationResult ValidateConfigFile(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            ParameterValidationResult myResults = new ParameterValidationResult();

            string path = string.Empty;

            if (!configParameters.Contains(MAParameterNames.MAConfigurationFile))
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "A configuration file must be specified";
                myResults.ErrorParameter = MAParameterNames.MAConfigurationFile;
                return myResults;
            }
            else
            {
                path = configParameters[MAParameterNames.MAConfigurationFile].Value;

                if (!System.IO.Path.IsPathRooted(path))
                {
                    path = System.IO.Path.Combine(Utils.ExtensionsDirectory, path);
                }
            }

            if (!System.IO.File.Exists(configParameters[MAParameterNames.MAConfigurationFile].Value))
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "The configuration file does not exist";
                myResults.ErrorParameter = MAParameterNames.MAConfigurationFile;
                return myResults;
            }
            else
            {
                try
                {
                    MAConfig.Load(path);
                }
                catch (Exception ex)
                {
                    myResults.Code = ParameterValidationResultCode.Failure;
                    myResults.ErrorMessage = string.Format("The configuration file could not be loaded\n{0}\n{1}", ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message);
                    myResults.ErrorParameter = MAParameterNames.MAConfigurationFile;
                    return myResults;
                }
            }

            myResults.Code = ParameterValidationResultCode.Success;

            return myResults;
        }

        /// <summary>
        /// Validates the host parameters and ensures a connection can be made to the server
        /// </summary>
        /// <param name="configParameters">The configuration parameters from the user interface</param>
        /// <param name="page">The configuration page</param>
        /// <returns>A ParameterValidationResult containing the validation status</returns>
        public static ParameterValidationResult ValidateHost(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            ParameterValidationResult myResults = new ParameterValidationResult();

            if (string.IsNullOrWhiteSpace(configParameters[MAParameterNames.HostName].Value))
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "The hostname cannot be blank";
                myResults.ErrorParameter = MAParameterNames.HostName;
                return myResults;
            }

            int result = 0;
            if (!int.TryParse(configParameters[MAParameterNames.Port].Value, out result))
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "The port number must be an integer";
                myResults.ErrorParameter = MAParameterNames.Port;
                return myResults;
            }
            else if (result <= 0)
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "The port number must be an integer greater than 0";
                myResults.ErrorParameter = MAParameterNames.Port;
                return myResults;
            }

            try
            {
                SshConnection.OpenSshConnection(configParameters);
                SshConnection.CloseSshConnection();
            }
            catch (Exception ex)
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = "Could not connect to the target server\n" + ex.Message;
                myResults.ErrorParameter = MAParameterNames.HostName;
                return myResults;
            }
            
            myResults.Code = ParameterValidationResultCode.Success;

            return myResults;
        }
    }
}
