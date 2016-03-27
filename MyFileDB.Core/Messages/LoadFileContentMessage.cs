using Akka.Actor;
using MyFileDB.Common.Services;

namespace MyFileDB.Core.Messages
{
    public class LoadFileContentMessage
    {
        public LoadFileContentMessage(string rootPath, string folderName, string fileName, IActorRef callBackActorRef, string fileContentBodyType)
        {
            RootPath = rootPath;
            FolderName = folderName;
            FileName = fileName;
            CallBackActorRef = callBackActorRef;
            FileContentBodyType = fileContentBodyType;
        }

        public IActorRef CallBackActorRef { get; private set; }
        public string RootPath { private set; get; }
        public string FolderName { private set; get; }
        public string FileName { private set; get; }
        public string FileContentBodyType { private set; get; }
    }
}