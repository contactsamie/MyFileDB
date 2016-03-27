using Akka.Actor;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;
using System.Collections.Generic;

namespace MyFileDB.Core.Actors
{
    public class FileUpdateBridgeActor : ReceiveActor
    {
        public FileUpdateBridgeActor(IFileService fileService)
        {
            Receive<UpdateOneFileIdentityMessage>(message =>
            {
                //todo store file
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                fileService.Update(directory + message.FileName, message.FileContent);
                Sender.Tell(new EachFileUpdatedMessage(message));

                //tell to update file cache
                ApplicationActorSystem.ActorReferences.ApplicationActorRef.Tell(new LoadAllFileContentMessage(new List<LoadFileContentMessage>()
                {
                    new LoadFileContentMessage(message.RootPath,message.FolderName,message.FileName,Sender,message.FileContent.DataType)
                }));
            });
        }
    }
}