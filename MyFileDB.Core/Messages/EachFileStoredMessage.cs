namespace MyFileDB.Core.Messages
{
    public class EachFileStoredMessage
    {

        public EachFileStoredMessage(StoreOneFileIdentityMessage fileStoredMessage)
        {
            FileStoredMessage = fileStoredMessage;
        }

        public StoreOneFileIdentityMessage FileStoredMessage { private set; get; }
    }
}