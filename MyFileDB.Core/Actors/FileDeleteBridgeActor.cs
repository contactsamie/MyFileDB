using System.Collections.Generic;
using Akka.Actor;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;

namespace MyFileDB.Core.Actors
{
    public class FileDeleteBridgeActor : ReceiveActor
    {
        public FileDeleteBridgeActor(IFileService fileService)
        {
            
            Receive<DeleteOneFileIdentityMessage>(message =>
            {
                //todo store file
                var directory = message.RootPath + "/" + message.FolderName + "/";
                fileService.CreateDirectoryIfItDoesntExist(directory);
                fileService.Delete(directory + message.FileName);
                Sender.Tell(new EachFileDeletedMessage(message));

                //tell to update file cache
                ApplicationActorSystem.ActorReferences.ApplicationActorRef.Tell(new LoadAllFileContentMessage(new List<LoadFileContentMessage>()
                {
                    new LoadFileContentMessage(message.RootPath,message.FolderName,message.FileName,Sender)
                }));
            });
        }
    }
}