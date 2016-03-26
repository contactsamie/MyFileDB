using System.Collections.Generic;

namespace MyFileDB.Core.Messages
{
    public class FilesByFolderResultMessage
    {
        public FilesByFolderResultMessage(List<string> fileNames)
        {
            FileNames = fileNames;
        }

        public List<string> FileNames { private set; get; }
    }
}