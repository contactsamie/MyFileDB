using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class StoreFilesMessage
    {
        public StoreFilesMessage(List<StoreOneFileMessage> storeOneFileMessages)
        {
            StoreOneFileMessages = storeOneFileMessages ?? new List<StoreOneFileMessage>();
        }

        public List<StoreOneFileMessage> StoreOneFileMessages { private set; get; }
    }
}