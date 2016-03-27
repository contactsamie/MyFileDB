using MyFileDB.Common.Services;

namespace MyFileDB.Core.Messages
{
    public class StoreOneFileIdentityMessage : AFileIdentityMessage
    {
        public FileContent FileContent { get; set; }
    }
}