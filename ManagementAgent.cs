// -----------------------------------------------------------------------
// <copyright file="ManagementAgent.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Renci.SshNet;

    /// <summary>
    /// The IMAExtensible2 management agent called by the FIM Synchronization Service
    /// </summary>
    public class ManagementAgent :
        IMAExtensible2CallExport,
        IMAExtensible2CallImport,
        IMAExtensible2GetSchema,
        IMAExtensible2GetParameters,
        IMAExtensible2Password,
        IMAExtensible2GetCapabilitiesEx
    {
        /// <summary>
        /// The import page size specified in the run step
        /// </summary>
        private int importPageSize;

        /// <summary>
        /// The enumeration used for import operations
        /// </summary>
        private IEnumerator<CSEntryChange> importEnumerator;

        /// <summary>
        /// The schema types specified for the current import run step
        /// </summary>
        private KeyedCollection<string, SchemaType> schemaTypes;
        
        /// <summary>
        /// The default import page size
        /// </summary>
        private int importDefaultPageSize = 500;

        /// <summary>
        /// The maximum import page size
        /// </summary>
        private int importMaxPageSize = 1000;

        /// <summary>
        /// The default export page size
        /// </summary>
        private int exportDefaultPageSize = 100;

        /// <summary>
        /// The maximum export page size
        /// </summary>
        private int exportMaxPageSize = 200;

        /// <summary>
        /// The configuration parameters assigned to this run job
        /// </summary>
        private KeyedCollection<string, ConfigParameter> configParameters;
        
        /// <summary>
        /// Initializes a new instance of the ManagementAgent class
        /// </summary>
        public ManagementAgent()
        {
        }
        
        /// <summary>
        /// Gets the maximum import page size
        /// </summary>
        int IMAExtensible2CallImport.ImportMaxPageSize
        {
            get
            {
                return this.importMaxPageSize;
            }
        }

        /// <summary>
        /// Gets the default import page size
        /// </summary>
        int IMAExtensible2CallImport.ImportDefaultPageSize
        {
            get
            {
                return this.importDefaultPageSize;
            }
        }

        /// <summary>
        /// Gets the default export page size
        /// </summary>
        int IMAExtensible2CallExport.ExportDefaultPageSize
        {
            get
            {
                return this.exportDefaultPageSize;
            }
        }

        /// <summary>
        /// Gets the maximum export page size
        /// </summary>
        int IMAExtensible2CallExport.ExportMaxPageSize
        {
            get
            {
                return this.exportMaxPageSize;
            }
        }

        /// <summary>
        /// Gets or sets the type of import
        /// </summary>
        private OperationType ImportType { get; set; }

        /// <summary>
        /// Gets a value indicating whether debug mode is enabled
        /// </summary>
        private bool DebugEnabled
        {
            get
            {
                if (this.configParameters.Contains(MAParameterNames.DebugEnabled))
                {
                    return this.configParameters[MAParameterNames.DebugEnabled].Value == "1" ? true : false;
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
        private string LogPath
        {
            get
            {
                return this.configParameters[MAParameterNames.LogPath].Value;
            }
        }

        /// <summary>
        /// Gets the host name
        /// </summary>
        private string HostName
        {
            get
            {
                return this.configParameters[MAParameterNames.HostName].Value;
            }
        }

        /// <summary>
        /// Gets the port
        /// </summary>
        private string Port
        {
            get
            {
                return this.configParameters[MAParameterNames.Port].Value;
            }
        }

        /// <summary>
        /// Gets the path to this MA's configuration file
        /// </summary>
        private string MAConfigurationFilePath
        {
            get
            {
                return this.configParameters[MAParameterNames.MAConfigurationFile].Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the debugger should be launched when an exception occurs
        /// </summary>
        private bool LaunchDebuggerOnException
        {
            get
            {
                return this.configParameters[MAParameterNames.LaunchDebuggerOnException].Value == "1" ? true : false;
            }
        }

        /// <summary>
        /// Gets the capabilities of this management agent
        /// </summary>
        /// <param name="configParameters">The current configuration parameters for this MA</param>
        /// <returns>The capabilities of this management agent</returns>
        MACapabilities IMAExtensible2GetCapabilitiesEx.GetCapabilitiesEx(KeyedCollection<string, ConfigParameter> configParameters)
        {
            this.configParameters = configParameters;
            this.LoadConfiguration();

            MACapabilities capabilities = new MACapabilities();
            capabilities.ConcurrentOperation = true;
            capabilities.ObjectRename = MAConfig.Capabilities.ObjectRenameAllowed;
            capabilities.DeleteAddAsReplace = MAConfig.Capabilities.DeleteAddAsReplace;
            capabilities.ExportType = MAConfig.Capabilities.ObjectUpdateMode;
            capabilities.DeltaImport = MAConfig.Capabilities.DeltaImport;
            capabilities.DistinguishedNameStyle = MADistinguishedNameStyle.Generic;
            capabilities.NoReferenceValuesInFirstExport = false;
            capabilities.Normalizations = MANormalizations.None;
            capabilities.IsDNAsAnchor = false;
            return capabilities;
        }
        
        /// <summary>
        /// Gets the list of configuration parameters for the management agent
        /// </summary>
        /// <param name="configParameters">The list of configuration parameters</param>
        /// <param name="page">The page to get the configuration parameters for</param>
        /// <returns>A list of ConfigParameterDefinition objects</returns>
        IList<ConfigParameterDefinition> IMAExtensible2GetParameters.GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> configParametersDefinitions = new List<ConfigParameterDefinition>();

            switch (page)
            {
                case ConfigParameterPage.Connectivity:
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.HostName, string.Empty, string.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.Port, string.Empty, "22"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.MAConfigurationFile, string.Empty, "Lithnet.SSHMA.config.xml"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDropDownParameter(
                        MAParameterNames.AuthenticationMode, 
                        new string[] { "Username/Password", "Username/Key" },
                        false));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.PrivateKeyFile, string.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.Username, string.Empty, string.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(MAParameterNames.PasswordOrPassphrase, string.Empty, string.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(MAParameterNames.LogPath, string.Empty, "%temp%\\lithnet.sshma.log"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(MAParameterNames.DebugEnabled, false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(MAParameterNames.LaunchDebuggerOnException, false));
                    break;

                case ConfigParameterPage.Global:
                    break;

                case ConfigParameterPage.Partition:
                    break;

                case ConfigParameterPage.RunStep:
                    break;
            }

            return configParametersDefinitions;
        }

        /// <summary>
        /// Validates the values provided in the UI for the supplied configuration parameters
        /// </summary>
        /// <param name="configParameters">The list of configuration parameters</param>
        /// <param name="page">The page to get the configuration parameters for</param>
        /// <returns>A ParameterValidationResult object containing the results of the validation</returns>
        ParameterValidationResult IMAExtensible2GetParameters.ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            ParameterValidationResult myResults = new ParameterValidationResult();

            myResults = MAParameterValidator.ValidateConfigFile(configParameters, page);

            if (myResults.Code != ParameterValidationResultCode.Success)
            {
                return myResults;
            }

            myResults = MAParameterValidator.ValidateHost(configParameters, page);

            if (myResults.Code != ParameterValidationResultCode.Success)
            {
                return myResults;
            }

            myResults.Code = ParameterValidationResultCode.Success;
            return myResults;
        }

        /// <summary>
        /// Gets the schema that applies to the objects in this management agent
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <returns>The Schema defining objects and attributes applicable to this management agent</returns>
        Schema IMAExtensible2GetSchema.GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            try
            {
                this.configParameters = configParameters;
                Logger.LogPath = this.LogPath;
                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Loading Schema");

                this.LoadConfiguration();

                Schema schema = Schema.Create();

                foreach (MASchemaObject schemaObject in MASchema.Objects)
                {
                    SchemaType schemaType = SchemaType.Create(schemaObject.ObjectClass, true);
                    schemaType.Attributes.Add(SchemaAttribute.CreateAnchorAttribute("entry-dn", AttributeType.String, AttributeOperation.ImportOnly));

                    foreach (MASchemaAttribute attribute in schemaObject.Attributes)
                    {
                        if (attribute.IsMultiValued)
                        {
                            schemaType.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(attribute.Name, attribute.Type, attribute.Operation));
                        }
                        else
                        {
                            schemaType.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(attribute.Name, attribute.Type, attribute.Operation));
                        }
                    }

                    schema.Types.Add(schemaType);
                }

                Logger.WriteLine("Schema loaded successfully");
                return schema;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("The schema could not be loaded: " + ex.Message, ex);
            }
        }
      
        /// <summary>
        /// Configures the import session at the beginning of an import
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <param name="types">The schema types that apply to this import run</param>
        /// <param name="importRunStep">The definition of the current run step</param>
        /// <returns>Results of the import setup</returns>
        OpenImportConnectionResults IMAExtensible2CallImport.OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenImportConnectionRunStep importRunStep)
        {
            try
            {
                this.configParameters = configParameters;

                if (this.DebugEnabled)
                {
                    System.Diagnostics.Debugger.Launch();
                }

                Logger.LogPath = this.LogPath;
                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting Import");

                this.LoadConfiguration();

                SshConnection.OpenSshConnection(this.configParameters);
                OperationBase operation;
                this.ImportType = importRunStep.ImportType;
                this.schemaTypes = types.Types;
                this.importPageSize = importRunStep.PageSize;

                if (importRunStep.ImportType == OperationType.Delta)
                {
                    operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ImportDeltaStartOperation);
                }
                else
                {
                    operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ImportFullStartOperation);
                }

                if (operation != null)
                {
                    try
                    {
                        SshConnection.ExecuteOperation(operation);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform import start operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Import start operation failed", ex);
                    }
                }

                this.importEnumerator = CSEntryImport.GetObjects(this.schemaTypes).GetEnumerator();
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the open import connection operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the open import connection operation", ex);
            }

            OpenImportConnectionResults results = new OpenImportConnectionResults();
            return new OpenImportConnectionResults();
        }
      
        /// <summary>
        /// Gets a batch of entries from the database and returns them to the synchronization service
        /// </summary>
        /// <param name="importRunStep">The current run step</param>
        /// <returns>The results of the import batch</returns>
        GetImportEntriesResults IMAExtensible2CallImport.GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            List<CSEntryChange> csentries = new List<CSEntryChange>();
            int count = 0;
            bool mayHaveMore = false;

            while (this.importEnumerator.MoveNext())
            {
                CSEntryChange csentry = this.importEnumerator.Current;
                csentries.Add(csentry);

                if (csentry.ErrorCodeImport == MAImportError.ImportErrorCustomStopRun)
                {
                    break;
                }

                count++;

                if (count >= this.importPageSize)
                {
                    mayHaveMore = true;
                    break;
                }
            }

            GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();
            importReturnInfo.MoreToImport = mayHaveMore;
            importReturnInfo.CSEntries = csentries;
            return importReturnInfo;
        }

        /// <summary>
        /// Closes the import session
        /// </summary>
        /// <param name="importRunStepInfo">The current run step</param>
        /// <returns>Results of the import session close</returns>
        CloseImportConnectionResults IMAExtensible2CallImport.CloseImportConnection(CloseImportConnectionRunStep importRunStepInfo)
        {
            try
            {
                OperationBase operation;
                if (this.ImportType == OperationType.Delta)
                {
                    operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ImportDeltaEndOperation);
                }
                else
                {
                    operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ImportFullEndOperation);
                }

                if (operation != null)
                {
                    try
                    {
                        SshConnection.ExecuteOperation(operation);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform import end operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Import end operation failed", ex);
                    }
                }

                try
                {
                    SshConnection.CloseSshConnection();
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Could not close SSH connection");
                    Logger.WriteException(ex);
                }

                Logger.WriteLine("Import Complete");
                Logger.WriteSeparatorLine('*');
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the open export connection operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the open export connection operation", ex);
            }

            return new CloseImportConnectionResults();
        }

        /// <summary>
        /// Begins an export session
        /// </summary>
        /// <param name="configParameters">The configuration parameters supplied to this management agent</param>
        /// <param name="types">The schema types that apply to this export run</param>
        /// <param name="exportRunStep">The definition of the current run step</param>
        void IMAExtensible2CallExport.OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            try
            {
                this.configParameters = configParameters;

                if (this.DebugEnabled)
                {
                    System.Diagnostics.Debugger.Launch();
                }

                Logger.LogPath = this.LogPath;
                this.LoadConfiguration();
                SshConnection.OpenSshConnection(this.configParameters);

                OperationBase operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ExportStartOperation);

                if (operation != null)
                {
                    try
                    {
                        SshConnection.ExecuteOperation(operation);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform export start operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Export start operation failed", ex);
                    }
                }

                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting Export");
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the open export connection operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the open export connection operation", ex);
            }
        }

        /// <summary>
        /// Ends an export session
        /// </summary>
        /// <param name="exportRunStep">The results of the export session close</param>
        void IMAExtensible2CallExport.CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            OperationBase operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is ExportEndOperation);

            if (operation != null)
            {
                try
                {
                    SshConnection.ExecuteOperation(operation);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Could not perform export end operation");
                    Logger.WriteException(ex);
                    throw new ExtensibleExtensionException("Export end operation failed", ex);
                }
            }

            try
            {
                SshConnection.CloseSshConnection();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Could not close SSH connection");
                Logger.WriteException(ex);
            }
            
            Logger.WriteLine("Export Complete");
            Logger.WriteSeparatorLine('*');
        }

        /// <summary>
        /// Exports a batch of entries to the database
        /// </summary>
        /// <param name="csentries">A list of changes to export</param>
        /// <returns>The results of the batch export</returns>
        PutExportEntriesResults IMAExtensible2CallExport.PutExportEntries(IList<CSEntryChange> csentries)
        {
            PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();

            foreach (CSEntryChange csentry in csentries)
            {
                try
                {
                    List<AttributeChange> anchorchanges = new List<AttributeChange>();
                    bool referenceRetryRequired;
                    anchorchanges.AddRange(CSEntryExport.PutExportEntry(csentry, out referenceRetryRequired));

                    if (referenceRetryRequired)
                    {
                        Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentry.DN));
                        exportEntriesResults.CSEntryChangeResults.Add(CSEntryChangeResult.Create(csentry.Identifier, anchorchanges, MAExportError.ExportActionRetryReferenceAttribute));
                    }
                    else
                    {
                        exportEntriesResults.CSEntryChangeResults.Add(CSEntryChangeResult.Create(csentry.Identifier, anchorchanges, MAExportError.Success));
                    }
                }
                catch (Exception ex)
                {
                    if (exportEntriesResults.CSEntryChangeResults.Contains(csentry.Identifier))
                    {
                        exportEntriesResults.CSEntryChangeResults.Remove(csentry.Identifier);
                    }

                    exportEntriesResults.CSEntryChangeResults.Add(this.GetExportChangeResultFromException(csentry, ex));
                }
            }

            return exportEntriesResults;
        }

        /// <summary>
        /// Determines the security level of the connection
        /// </summary>
        /// <returns>Returns a ConnectionSecurityLevel value that determines the level of security to the server</returns>
        ConnectionSecurityLevel IMAExtensible2Password.GetConnectionSecurityLevel()
        {
            return ConnectionSecurityLevel.Secure;
        }

        /// <summary>
        /// Begins a password connection to the server
        /// </summary>
        /// <param name="configParameters">The collection of configuration parameters</param>
        /// <param name="partition">The partition details on which the password operation should occur</param>
        void IMAExtensible2Password.OpenPasswordConnection(KeyedCollection<string, ConfigParameter> configParameters, Partition partition)
        {
            try
            {
                this.configParameters = configParameters;

                if (this.DebugEnabled)
                {
                    System.Diagnostics.Debugger.Launch();
                }

                Logger.LogPath = this.LogPath;
                this.LoadConfiguration();
                SshConnection.OpenSshConnection(this.configParameters);

                OperationBase operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is PasswordStartOperation);

                if (operation != null)
                {
                    try
                    {
                        SshConnection.ExecuteOperation(operation);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform password start operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Password start operation failed", ex);
                    }
                }

                Logger.WriteSeparatorLine('*');
                Logger.WriteLine("Starting password operation");
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the open password connection operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the open password connection operation", ex);
            }
        }

        /// <summary>
        /// Ends a connection to the server. It is called after processing is completed with the server to release resources
        /// </summary>
        void IMAExtensible2Password.ClosePasswordConnection()
        {
            OperationBase operation = MAConfig.GlobalOperations.FirstOrDefault(t => t is PasswordEndOperation);

            if (operation != null)
            {
                try
                {
                    SshConnection.ExecuteOperation(operation);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Could not perform password end operation");
                    Logger.WriteException(ex);
                    throw new ExtensibleExtensionException("Password end operation failed", ex);
                }
            }

            try
            {
                SshConnection.CloseSshConnection();
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Could not close SSH connection");
                Logger.WriteException(ex);
            }
            
            Logger.WriteLine("Ending password operation");
            Logger.WriteSeparatorLine('*');
        }

        /// <summary>
        /// Changes the password for the connector space object
        /// </summary>
        /// <param name="csentry">The CSEntry object for the user</param>
        /// <param name="oldPassword">The old password</param>
        /// <param name="newPassword">The new password</param>
        void IMAExtensible2Password.ChangePassword(CSEntry csentry, System.Security.SecureString oldPassword, System.Security.SecureString newPassword)
        {
            try
            {
                PasswordChangeOperation operation = MAConfig.OperationGroups[csentry.ObjectClass[0].ToString()].ObjectOperations.FirstOrDefault(t => t is PasswordChangeOperation) as PasswordChangeOperation;

                if (operation != null)
                {
                    try
                    {
                        Logger.WriteLine("Attempting to change password for {0}", csentry.DN.ToString());
                        SshConnection.ExecuteOperation(operation, csentry, oldPassword.ToUnsecureString(), newPassword.ToUnsecureString());
                        Logger.WriteLine("Changed password for {0} successfully", csentry.DN.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform password change operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Set password change operation failed", ex);
                    }
                }
                else
                {
                    Logger.WriteLine("Could not change password for {0} as no password change operation was defined for this object type", csentry.DN.ToString());
                }
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the password change operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the password change operation", ex);
            }
        }

        /// <summary>
        /// Sets the password for the connector space object
        /// </summary>
        /// <param name="csentry">The CSEntry object for the user</param>
        /// <param name="newPassword">The new password</param>
        /// <param name="options">A PasswordOptions object specifying the options for setting the password</param>
        void IMAExtensible2Password.SetPassword(CSEntry csentry, System.Security.SecureString newPassword, PasswordOptions options)
        {
            try
            {
                Logger.LogLevel = LogLevel.Debug;
                PasswordSetOperation operation = MAConfig.OperationGroups[csentry.ObjectClass[0].ToString()].ObjectOperations.FirstOrDefault(t => t is PasswordSetOperation) as PasswordSetOperation;

                if (operation != null)
                {
                    try
                    {
                        Logger.WriteLine("Attempting to set password for {0}", csentry.DN.ToString());
                        SshConnection.ExecuteOperation(operation, csentry, newPassword.ToUnsecureString());
                        Logger.WriteLine("Set password for {0} successfully", csentry.DN.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Could not perform set password operation");
                        Logger.WriteException(ex);
                        throw new ExtensibleExtensionException("Set password set operation failed", ex);
                    }
                }
                else
                {
                    Logger.WriteLine("Could not set password for {0} as no password set operation was defined for this object type", csentry.DN.ToString());
                }
            }
            catch (ExtensibleExtensionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("A exception occured during the password set operartion");
                Logger.WriteException(ex);
                throw new ExtensibleExtensionException("An exception occured during the password set operation", ex);
            }
        }

        /// <summary>
        /// Constructs a CSEntryChangeResult object appropriate to the specified exception
        /// </summary>
        /// <param name="csentryChange">The CSEntryChange object that triggered the exception</param>
        /// <param name="ex">The exception that was caught</param>
        /// <returns>A CSEntryChangeResult object with the correct error code for the exception that was encountered</returns>
        private CSEntryChangeResult GetExportChangeResultFromException(CSEntryChange csentryChange, Exception ex)
        {
            if (this.LaunchDebuggerOnException)
            {
                System.Diagnostics.Debugger.Launch();
            }

            if (ex is NoSuchObjectException)
            {
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorConnectedDirectoryMissingObject);
            }
            else if (ex is ReferencedObjectNotPresentException)
            {
                Logger.WriteLine(string.Format("Reference attribute not available for csentry {0}. Flagging for retry", csentryChange.DN));
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportActionRetryReferenceAttribute);
            }
            else
            {
                Logger.WriteLine(string.Format("An unexpected exception occured for csentry attributeChange {0} with DN {1}", csentryChange.Identifier.ToString(), csentryChange.DN ?? string.Empty));
                Logger.WriteException(ex);
                return CSEntryChangeResult.Create(csentryChange.Identifier, null, MAExportError.ExportErrorCustomContinueRun, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Loads the MA configuration
        /// </summary>
        private void LoadConfiguration()
        {
            if (System.IO.Path.IsPathRooted(this.MAConfigurationFilePath))
            {
                MAConfig.Load(this.MAConfigurationFilePath);
            }
            else
            {
                MAConfig.Load(System.IO.Path.Combine(Utils.ExtensionsDirectory, this.MAConfigurationFilePath));
            }
        }
    }
}