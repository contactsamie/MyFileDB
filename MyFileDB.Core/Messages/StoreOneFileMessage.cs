namespace MyFileDB.Core.Messages
{
    public class StoreOneFileIdentityMessage: AFileIdentityMessage
    {
        public string FileContent { get; set; }
    }
}