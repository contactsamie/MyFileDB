using Akka.Actor;
using MyFileDB.Common.Services;

namespace MyFileDB.Core.Messages
{
    public class FileContentUpdateMessage
    {
        public FileContentUpdateMessage(string fileName, FileContent fileContent, IActorRef callBackActorRef, string folderName, string rootPath)
        {
            FileName = fileName;

            FileContent = fileContent;
            CallBackActorRef = callBackActorRef;
            FolderName = folderName;
            RootPath = rootPath;
        }

        public IActorRef CallBackActorRef { private set; get; }
        public string FileName { private set; get; }
        public string FolderName { private set; get; }
        public string RootPath { private set; get; }
        public FileContent FileContent { get; private set; }
    }
}