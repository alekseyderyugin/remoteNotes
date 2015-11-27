using System;

namespace remoteNotesLib
{
    public class TransactionException : Exception
    {
        public string message;
        public Note note;

        public TransactionException(string message, Note note)
        {
            this.message = message;
            this.note = note;
        }
    }
}
