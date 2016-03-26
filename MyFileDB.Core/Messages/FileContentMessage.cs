namespace MyFileDB.Core.Messages
{
    public class FileContentMessage
    {
        public FileContentMessage(string fileName, string fileContent)
        {
            FileName = fileName;
            FileContent = fileContent;
        }

        public string FileName { private set; get; }
        public string FileContent { get; private set; }
    }
}