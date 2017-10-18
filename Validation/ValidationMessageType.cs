using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTNF.Library.Validation
{
    /// <summary>
    /// For classifying error message.
    /// </summary>
    public enum ValidationMessageType
    {
        /// <summary>
        /// Error
        /// </summary>
        Error = 0,
        /// <summary>
        /// Warning
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Valid
        /// </summary>
        Valid = 2
    }
}
