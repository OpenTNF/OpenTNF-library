using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTNF.Library
{
    /// <summary>
    /// Exception class to be used when appropriate exception class does not exists
    /// </summary>
    public class OpenTnfException : Exception
    {
        /// <summary>
        /// Base constructor
        /// </summary>
        public OpenTnfException() : this(String.Empty) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public OpenTnfException(string message) : base(message) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public OpenTnfException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgParams"></param>
        public OpenTnfException(string message, params object[] msgParams) : this(String.Format(message, msgParams)) { }
    }
}
