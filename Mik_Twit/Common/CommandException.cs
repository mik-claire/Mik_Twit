using System;

namespace Mik_Twit.Common
{
    public class CommandException : Exception
    {
        public CommandException(string message)
        {
            this.message = message;
        }

        public CommandException(string message, string stackTrace)
        {
            this.message = message;
            this.stackTrace = stackTrace;
        }

        private string message = string.Empty;
        public override string Message
        {
            get { return this.message; }
        }

        private string stackTrace = string.Empty;
        public override string StackTrace
        {
            get { return this.StackTrace; }
        }
    }
}
