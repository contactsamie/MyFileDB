namespace MyFileDB.Core.Messages
{
    public class LoadFileContentMessage
    {
        public LoadFileContentMessage(string rootPath, string folderName, string fileName)
        {
            RootPath = rootPath;
            FolderName = folderName;
            FileName = fileName;
        }

        public string RootPath { private set; get; }
        public string FolderName { private set; get; }
        public string FileName { private set; get; }
    }
}