// -----------------------------------------------------------------------
// <copyright file="ReferencedObjectNotPresentException.cs" company="Lithnet">
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
    /// Thrown when one object that is referenced by another is not present 
    /// </summary>
    public class ReferencedObjectNotPresentException : Exception
    {
    }
}
