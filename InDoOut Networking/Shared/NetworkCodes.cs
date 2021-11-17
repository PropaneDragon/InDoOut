namespace InDoOut_Executable_Core.Networking
{
    public class NetworkCodes
    {
        /// <summary>
        /// A message identifier to put at the beginning of any network message to indicate
        /// that it is the start of a new message.
        /// </summary>
        public static readonly string MESSAGE_BEGIN_IDENTIFIER = "\uafff\ua001\ua001\uafff";
        /// <summary>
        /// A message identifier to put at the end of any network message to indicate that
        /// it is the start of a message.
        /// </summary>
        public static readonly string MESSAGE_END_IDENTIFIER = "\uafff\ua001\ua002\uafff";
        /// <summary>
        /// A ping identifier to indicate that this message is just a status update ping.
        /// </summary>
        public static readonly string MESSAGE_PING_IDENTIFIER = "\uafff\ua001\ua003\uafff";

        /// <summary>
        /// An identifier to split a message ID and message data from each other.
        /// </summary>
        public static readonly string MESSAGE_ID_COMMAND_SPLITTER = "\uafff\ua001\ua004\uafff";

        /// <summary>
        /// Command message data to indicate a general success. Use <see cref="COMMAND_DATA_GENERIC_SPLITTER"/>
        /// to split up data between this identifier and the success message.
        /// </summary>
        public static readonly string COMMAND_SUCCESS_IDENTIFIER = "\uafff\ua001\ua005\uafff";
        /// <summary>
        /// Command message data to indicate a general failure. Use <see cref="COMMAND_DATA_GENERIC_SPLITTER"/>
        /// to split up data between this identifier and the failure message.
        /// </summary>
        public static readonly string COMMAND_FAILURE_IDENTIFIER = "\uafff\ua001\ua006\uafff";

        /// <summary>
        /// A data specific splitter between the command name and command data. This is
        /// different to <see cref="MESSAGE_ID_COMMAND_SPLITTER"/> as that has a specific
        /// random identifier to link back to a specific message, and this targets a specific
        /// response command on the receiver.
        /// </summary>
        public static readonly string COMMAND_NAME_DATA_SPLITTER = "\uafff\ua001\ua007\uafff";
        /// <summary>
        /// A generic splitter between elements of data in the command data.
        /// </summary>
        public static readonly string COMMAND_DATA_GENERIC_SPLITTER = "\uafff\ua001\ua008\uafff";
    }
}
