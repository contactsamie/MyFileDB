using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class StoreFilesMessage
    {
        public StoreFilesMessage(List<StoreOneFileIdentityMessage> storeOneFileMessages)
        {
            StoreOneFileMessages = storeOneFileMessages ?? new List<StoreOneFileIdentityMessage>();
        }

        public List<StoreOneFileIdentityMessage> StoreOneFileMessages { private set; get; }
    }
}