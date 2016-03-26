namespace MyFileDB.Core.Messages
{
    public class StoreOneFileMessage
    {
        public string RootPath { set; get; }
        public string FolderName { set; get; }
        public string FileName { set; get; }
        public string FileContent { get; set; }
    }
}