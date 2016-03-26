using Akka.Actor;

namespace MyFileDB.Core.Messages
{
    public class LoadFileContentMessage
    {
        public LoadFileContentMessage(string rootPath, string folderName, string fileName, IActorRef callBackActorRef)
        {
            RootPath = rootPath;
            FolderName = folderName;
            FileName = fileName;
            CallBackActorRef = callBackActorRef;
        }

        public IActorRef CallBackActorRef { get; private set; }
        public string RootPath { private set; get; }
        public string FolderName { private set; get; }
        public string FileName { private set; get; }
    }
}