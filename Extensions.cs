// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Lithnet">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.SshMA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generic extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a secure string to an unsecure string
        /// </summary>
        /// <param name="secureString">The secure string to convert</param>
        /// <returns>The unsecured value of the secure string</returns>
        public static string ToUnsecureString(this SecureString secureString)
        {
            if (secureString == null)
            {
                return null;
            }
            
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Replaces a value within a string, ignoring the case of the strings
        /// </summary>
        /// <param name="originalString">The string containing the value to replace</param>
        /// <param name="oldValue">The value to find</param>
        /// <param name="newValue">The value to insert</param>
        /// <returns>A new string containing the specified string substitutions</returns>
        public static string ReplaceIgnoreCase(this string originalString, string oldValue, string newValue)
        {
            string result = Regex.Replace(originalString, Regex.Escape(oldValue), newValue, RegexOptions.IgnoreCase);

            return result;
        }

        /// <summary>
        /// Determines is one string contains another, ignoring the case of the strings
        /// </summary>
        /// <param name="originalString">The original string</param>
        /// <param name="value">The value to find</param>
        /// <returns>A value indicating whether the value was found in the original string</returns>
        public static bool ContainsIgnoreCase(this string originalString, string value)
        {
            return 0 <= originalString.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
