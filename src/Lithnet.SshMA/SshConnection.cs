// -----------------------------------------------------------------------
// <copyright file="SshConnection.cs" company="Lithnet">
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
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Renci.SshNet;

    /// <summary>
    /// Contains methods for managing the SSH connection and executing operations
    /// </summary>
    public static class SshConnection
    {
        /// <summary>
        /// The active SSH client connection
        /// </summary>
        private static SshClient client;

        /// <summary>
        /// Opens a connection to the SSH server
        /// </summary>
        /// <param name="config">The configuration parameters to use for the operation</param>
        public static void OpenSshConnection(KeyedCollection<string, ConfigParameter> config)
        {
            if (config[MAParameterNames.AuthenticationMode].Value == "Username/Password")
            {
                client = new SshClient(
                    config[MAParameterNames.HostName].Value, 
                    config[MAParameterNames.Username].Value,
                    GetPassword(config));
            }
            else
            {
                PrivateKeyFile file = GetPrivateKeyFile(config);
                client = new SshClient(
                    config[MAParameterNames.HostName].Value, 
                    config[MAParameterNames.Username].Value, 
                    file);
            }

            client.Connect();

            Logger.WriteLine("Connected to {0} as {1}", client.ConnectionInfo.Host, client.ConnectionInfo.Username);
        }
        
        /// <summary>
        /// Closes the SSH connection
        /// </summary>
        public static void CloseSshConnection()
        {
            if (client != null)
            {
                if (client.IsConnected)
                {
                    client.Disconnect();
                    Logger.WriteLine("Disconnected from " + client.ConnectionInfo.Host);
                }
            }
        }

        /// <summary>
        /// Executes the specified operation against the SSH client
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <returns>An OperationResult object containing the results of the execution</returns>
        public static OperationResult ExecuteOperation(OperationBase operation)
        {
            return ExecuteOperation(operation, null);
        }

        /// <summary>
        /// Executes the specified operation against the SSH client
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="csentry">The object to run the operation against</param>
        /// <returns>An OperationResult object containing the results of the execution</returns>
        public static OperationResult ExecuteOperation(OperationBase operation, CSEntryChange csentry)
        {
            OperationResult result = new OperationResult();
            result.ExecutedOperation = operation;

            foreach (CommandBase command in operation.Commands.Where(t => t.ShouldExecute(csentry)))
            {
                if (command is SyncCommand)
                {
                    ExecuteSyncCommand(csentry, result, command as SyncCommand);
                }
                else if (command is AsyncCommand)
                {
                    ExecuteAsyncCommand(csentry, result, command as AsyncCommand);
                }
                else
                {
                    throw new ArgumentException("Unknown command type");
                }
            }

            return result;
        }
  
        /// <summary>
        /// Executes the specified operation against the SSH client
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="csentry">The object to run the operation against</param>
        /// <param name="newPassword">The new password for the specified object</param>
        /// <returns>An OperationResult object containing the results of the execution</returns>
        public static OperationResult ExecuteOperation(PasswordSetOperation operation, CSEntry csentry, string newPassword)
        {
            return ExecuteOperation(operation, csentry, null, newPassword);
        }

        /// <summary>
        /// Executes the specified operation against the SSH client
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="csentry">The object to run the operation against</param>
        /// <param name="oldPassword">The old password for the specified object</param>
        /// <param name="newPassword">The new password for the specified object</param>
        /// <returns>An OperationResult object containing the results of the execution</returns>
        public static OperationResult ExecuteOperation(PasswordChangeOperation operation, CSEntry csentry, string oldPassword, string newPassword)
        {
            return ExecuteOperation(operation, csentry, oldPassword, newPassword);
        }

        /// <summary>
        /// Executes the specified operation against the SSH client
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="csentry">The object to run the operation against</param>
        /// <param name="oldPassword">The old password for the specified object</param>
        /// <param name="newPassword">The new password for the specified object</param>
        /// <returns>An OperationResult object containing the results of the execution</returns>
        private static OperationResult ExecuteOperation(OperationBase operation, CSEntry csentry, string oldPassword, string newPassword)
        {
            OperationResult result = new OperationResult();

            if (operation == null)
            {
                return null;
            }

            result.ExecutedOperation = operation;

            foreach (CommandBase command in operation.Commands)
            {
                if (command is SyncCommand)
                {
                    ExecuteSyncCommand(csentry, result, command as SyncCommand, oldPassword, newPassword);
                }
                else if (command is AsyncCommand)
                {
                    ExecuteAsyncCommand(csentry, result, command as AsyncCommand, oldPassword, newPassword);
                }
                else
                {
                    throw new ArgumentException("Unknown command type");
                }
            }

            return result;
        }
        
        /// <summary>
        /// Executes a synchronous command
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply to this command</param>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        private static void ExecuteSyncCommand(CSEntryChange csentry, OperationResult result, SyncCommand command)
        {
            if (command.ForEachAttribute == null)
            {
                string commandText = command.Command.ExpandDeclaration(csentry, false);
                SshCommand sshCommand = client.CreateCommand(commandText);
                Logger.WriteLine("Executing command: " + commandText);
                ExecuteSyncCommand(result, command, sshCommand);
            }
            else
            {
                ExecuteForEachSyncCommand(csentry, result, command);
            }
        }

        /// <summary>
        /// Executes a synchronous command that iterates through the values of a multi-valued attribute
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply to this command</param>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        private static void ExecuteForEachSyncCommand(CSEntryChange csentry, OperationResult result, SyncCommand command)
        {
            if (!csentry.AttributeChanges.Contains(command.ForEachAttribute.Name))
            {
                throw new ArgumentException("The for-each command could not be executed because the attribute was not present in the CSEntryChange");
            }

            foreach (string commandText in command.Command.ExpandDeclarationWithMultiValued(csentry, command.ForEachAttribute, command.ForEachValueModificationType))
            {
                SshCommand sshCommand = client.CreateCommand(commandText);
                Logger.WriteLine("Executing for-each command: " + commandText);
                ExecuteSyncCommand(result, command, sshCommand);
            }
        }

        /// <summary>
        /// Executes a synchronous command against a CSEntry object (typically a password change)
        /// </summary>
        /// <param name="csentry">The CSEntry to execute the command against</param>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        /// <param name="oldPassword">The old password, or null if performing a password set as opposed to a password change</param>
        /// <param name="newPassword">The new password</param>
        private static void ExecuteSyncCommand(CSEntry csentry, OperationResult result, SyncCommand command, string oldPassword, string newPassword)
        {
            SshCommand sshCommand = client.CreateCommand(command.Command.ExpandDeclaration(csentry, oldPassword, newPassword, false));
            Logger.WriteLine("Executing command: " + command.Command.ExpandDeclaration(csentry, oldPassword, newPassword, true));
            ExecuteSyncCommand(result, command, sshCommand);
        }

        /// <summary>
        /// Executes a synchronous command
        /// </summary>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        /// <param name="sshCommand">The SSH command object to execute</param>
        private static void ExecuteSyncCommand(OperationResult result, SyncCommand command, SshCommand sshCommand)
        {
            sshCommand.Execute();
            Logger.WriteLine("Command returned exit code {0}", sshCommand.ExitStatus.ToString());
            Logger.WriteRaw(sshCommand.Result, LogLevel.Debug);

            if (!command.SuccessCodes.Contains(sshCommand.ExitStatus))
            {
                if (!string.IsNullOrWhiteSpace(sshCommand.Error))
                {
                    throw new Microsoft.MetadirectoryServices.ExtensibleExtensionException(sshCommand.Error);
                }
                else
                {
                    throw new Microsoft.MetadirectoryServices.ExtensibleExtensionException("The command returned an exit code that was not in the list of successful exit codes: " + sshCommand.ExitStatus.ToString());
                }
            }

            if (command.HasObjects)
            {
                result.ExecutedCommandsWithObjects.Add(sshCommand);
            }

            result.ExecutedCommands.Add(sshCommand);
        }

        /// <summary>
        /// Executes an asynchronous command
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply to this command</param>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        private static void ExecuteAsyncCommand(CSEntryChange csentry, OperationResult result, AsyncCommand command)
        {
            string output = string.Empty;

            try
            {
                ShellStream shell = client.CreateShellStream("lithnet.sshma", 80, 24, 800, 600, 1024);
                output += shell.ReadLine();

                foreach (AsyncCommandSend sendCommand in command.Commands)
                {
                    if (sendCommand is AsyncCommandSendExpect)
                    {
                        AsyncCommandSendExpect expectCommand = sendCommand as AsyncCommandSendExpect;
                        string expectText = expectCommand.Expect.ExpandDeclaration(csentry, false);
                        TimeSpan timeout = new TimeSpan(0, 0, expectCommand.Timeout);

                        shell.Expect(
                            timeout,
                            new ExpectAction(
                                expectText,
                                (s) =>
                                {
                                    System.Diagnostics.Debug.WriteLine(s);
                                    output += s;
                                    shell.Write(expectCommand.Command.ExpandDeclaration(csentry, false) + "\n");
                                }));
                    }
                    else
                    {
                        shell.Write(sendCommand.Command.ExpandDeclaration(csentry, false) + "\n");
                    }
                }

                if (command.ExpectSuccess != null)
                {
                    if (!string.IsNullOrWhiteSpace(command.ExpectSuccess.Expect.DeclarationText))
                    {
                        TimeSpan timeout = new TimeSpan(0, 0, command.ExpectSuccess.Timeout);
                        if (shell.Expect(command.ExpectSuccess.Expect.ExpandDeclaration(csentry, false), timeout) == null)
                        {
                            throw new ExtensibleExtensionException("The asynchronous command did not return the expected result");
                        }
                    }
                }
            }
            finally
            {
                Logger.WriteLine("Shell session log:", LogLevel.Debug);
                Logger.WriteLine(output, LogLevel.Debug);
            }
        }

        /// <summary>
        /// Executes an asynchronous command
        /// </summary>
        /// <param name="csentry">The CSEntry to apply to this command</param>
        /// <param name="result">The result object for the current operation</param>
        /// <param name="command">The definition of the command to execute</param>
        /// <param name="oldPassword">The old password, or null if performing a password set as opposed to a password change</param>
        /// <param name="newPassword">The new password</param>
        private static void ExecuteAsyncCommand(CSEntry csentry, OperationResult result, AsyncCommand command, string oldPassword, string newPassword)
        {
            string output = string.Empty;

            try
            {
                ShellStream shell = client.CreateShellStream("lithnet.sshma", 80, 24, 800, 600, 1024);
                output += shell.ReadLine();

                foreach (AsyncCommandSend sendCommand in command.Commands)
                {
                    if (sendCommand is AsyncCommandSendExpect)
                    {
                        AsyncCommandSendExpect expectCommand = sendCommand as AsyncCommandSendExpect;
                        string expectText = expectCommand.Expect.ExpandDeclaration(csentry, oldPassword, newPassword, false);
                        TimeSpan timeout = new TimeSpan(0, 0, expectCommand.Timeout);
                        bool expectedArrived = false;

                        shell.Expect(
                            timeout,
                            new ExpectAction(
                                expectText,
                                (s) =>
                                {
                                    expectedArrived = true;
                                    System.Diagnostics.Debug.WriteLine(s);
                                    output += s;
                                    shell.Write(expectCommand.Command.ExpandDeclaration(csentry, oldPassword, newPassword, false) + "\n");
                                }));

                        if (!expectedArrived)
                        {
                            output += shell.Read();
                            throw new UnexpectedDataException("The expected value was not found in the session in the specified timeout period");
                        }
                    }
                    else
                    {
                        shell.Write(sendCommand.Command.ExpandDeclaration(csentry, oldPassword, newPassword, false) + "\n");
                    }
                }

                if (command.ExpectSuccess != null)
                {
                    if (!string.IsNullOrWhiteSpace(command.ExpectSuccess.Expect.DeclarationText))
                    {
                        TimeSpan timeout = new TimeSpan(0, 0, command.ExpectSuccess.Timeout);
                        string expectResult = shell.Expect(command.ExpectSuccess.Expect.ExpandDeclaration(csentry, oldPassword, newPassword, false), timeout);
                        output += expectResult ?? string.Empty;

                        if (expectResult == null)
                        {
                            output += shell.Read();
                            throw new Microsoft.MetadirectoryServices.ExtensibleExtensionException("The asynchronous command did not return the expected result");
                        }
                    }
                }

                output += shell.Read();
            }
            finally
            {
                Logger.WriteLine("Shell session log:", LogLevel.Debug);
                Logger.WriteLine(output, LogLevel.Debug);
            }
        }
        
        /// <summary>
        /// Gets a PrivateKeyFile object from the path specified in the configuration parameters
        /// </summary>
        /// <param name="config">The MA configuration parameters</param>
        /// <returns>A PrivateKeyFile object</returns>
        private static PrivateKeyFile GetPrivateKeyFile(KeyedCollection<string, ConfigParameter> config)
        {
            PrivateKeyFile file;

            if (!config.Contains(MAParameterNames.PasswordOrPassphrase))
            {
                FileStream stream = new FileStream(config[MAParameterNames.PrivateKeyFile].Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                file = new PrivateKeyFile(
                    stream);
            }
            else
            {
                FileStream stream = new FileStream(config[MAParameterNames.PrivateKeyFile].Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                file = new PrivateKeyFile(
                    stream,
                    GetPassword(config));
            }

            return file;
        }

        /// <summary>
        /// Gets the password from the specified parameter
        /// </summary>
        /// <param name="config">The configuration parameters</param>
        /// <returns>The unencrypted password</returns>
        private static string GetPassword(KeyedCollection<string, ConfigParameter> config)
        {
            string passphrase = string.Empty;

            if (config.Contains(MAParameterNames.PasswordOrPassphrase))
            {
                ConfigParameter parameter = config[MAParameterNames.PasswordOrPassphrase];

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