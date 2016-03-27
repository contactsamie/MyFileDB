using Akka.Actor;

namespace MyFileDB.Core.Messages
{
    public class FileContentDeleteMessage
    {
        public FileContentDeleteMessage(string fileName,  IActorRef callBackActorRef, string folderName, string rootPath)
        {
            FileName = fileName;
            CallBackActorRef = callBackActorRef;
            FolderName = folderName;
            RootPath = rootPath;
        }

        public IActorRef CallBackActorRef { private set; get; }
        public string FileName { private set; get; }
        public string FolderName { private set; get; }
        public string RootPath { private set; get; }
    }
}