using System;

namespace remoteNotesLib
{
    public class NotesTransactionSinglecall : MarshalByRefObject
    {
        public NotesTransactionSinglecall()
        {
            Console.WriteLine("NoteTransactionSinglecall was created");
        }

        public void commit(NotesClientActivated clientCache)
        {
            
        }

        public void rollback(NotesClientActivated clientCache)
        {
            clientCache.clear();
        }
    }
}
