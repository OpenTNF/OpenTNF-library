using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTNF.Library.Validation
{
    /// <summary>
    /// Validation message, with a type (error or warning) and a text.
    /// </summary>
    public class ValidationMessage
    {
        /// <summary>
        /// Type of message, error or warning.
        /// </summary>
        public ValidationMessageType MessageType { get; private set; }
        /// <summary>
        /// Error or warning details.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        public ValidationMessage(ValidationMessageType messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }

        /// <summary>
        /// Returns string presenting type of message (error or warning) and the explaining message text.
        /// </summary>
        public override string ToString()
        {
            switch (MessageType)
            {
                case ValidationMessageType.Error:
                    return String.Format(ValidationResources.ValidationErrorMessage, Message);
                case ValidationMessageType.Warning:
                    return String.Format(ValidationResources.ValidationWarningMessage, Message);
                default:
                    return Message;
            }
        }
    }
}
