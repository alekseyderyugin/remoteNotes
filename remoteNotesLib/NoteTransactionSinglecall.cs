using System;

namespace remoteNotesLib
{
    public class NoteTransactionSinglecall : MarshalByRefObject
    {
        public NoteTransactionSinglecall()
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
