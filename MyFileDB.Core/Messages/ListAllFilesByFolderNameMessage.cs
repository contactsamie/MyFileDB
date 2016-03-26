namespace MyFileDB.Core.Messages
{
    public class ListAllFilesByFolderNameMessage
    {
        public ListAllFilesByFolderNameMessage(string rootPath, string folderName)
        {
            RootPath = rootPath;
            FolderName = folderName;
        }

        public string RootPath { private set; get; }
        public string FolderName { private set; get; }
    }
}