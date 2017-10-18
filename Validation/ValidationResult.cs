using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTNF.Library.Validation
{
    /// <summary>
    /// Contains validation errors and warnings.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Validation messages, errors and warnings
        /// </summary>
        public IEnumerable<ValidationMessage> Messages { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationResult() : this(new List<ValidationMessage>()) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationResult(ValidationMessage message) : this(new[] { message }) { }

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationResult(IEnumerable<ValidationMessage> messages)
        {
            Messages = messages;
        }

        /// <summary>
        /// True if any of the contained messages is an error.
        /// </summary>
        /// <returns></returns>
        public bool HasErrors()
        {
            return Messages.Any(r => r.MessageType == ValidationMessageType.Error);
        }

        /// <summary>
        /// True if any of the contained messages is a warning.
        /// </summary>
        /// <returns></returns>
        public bool HasWarnings()
        {
            return Messages.Any(r => r.MessageType == ValidationMessageType.Warning);
        }

        /// <summary>
        /// Returns true if there are no errors or warnings.
        /// </summary>
        public bool Ok
        {
            get { return !HasErrors() && !HasWarnings(); }
        }

        /// <summary>
        /// Concatenates all messages into a comma separated string.
        /// Returns empty string if there are no messages.
        /// </summary>
        public override string ToString()
        {
            return Messages.Any()
                ? string.Join(", ", Messages.Select(r => r.ToString()))
                : string.Empty;
        }
    }
}
