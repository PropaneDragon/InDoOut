using System;
using System.Runtime.Serialization;

namespace InDoOut_Networking.Shared.Commands.Exceptions
{
    public class CommandFailureException : Exception
    {
        public CommandFailureException()
        {
        }

        public CommandFailureException(string message) : base(message)
        {
        }

        public CommandFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
