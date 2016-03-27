using Akka.Actor;
using Akka.DI.Core;
using MyFileDB.Core.Actors;
using MyFileDB.Core.Messages;

namespace MyFileDB.Core
{
    public class SystemActor : ReceiveActor
    {
        private IActorRef FileStorageBridgeCoOrdinatorRef { get; set; }
        private IActorRef FileQueryBridgeCoOrdinatorRef { set; get; }
        private IActorRef FileDeleteBridgeCoOrdinatorActorRef { set; get; }
    

        public SystemActor()
        {
            //pinginh
            Receive<PingMessage>(message => Sender.Tell(new PongMessage()));

            //storing
            FileStorageBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileStorageBridgeCoOrdinatorActor>());
            Receive<StoreFilesMessage>(message => FileStorageBridgeCoOrdinatorRef.Forward(message));
            Receive<EachFileStoredMessage>(message => { });

            //querying
            FileQueryBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileQueryBridgeCoOrdinatorActor>());
            Receive<LoadAllFileContentMessage>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));
            Receive<LoadFileContentsResultMessages>(message =>{ });

            //bulkloading
            Receive<ListAllFilesByFolderNameMessage>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));


            //deleting
            FileDeleteBridgeCoOrdinatorActorRef= Context.ActorOf(Context.System.DI().Props<FileDeleteBridgeCoOrdinatorActor>());
            Receive<DeleteFilesMessage>(message => FileDeleteBridgeCoOrdinatorActorRef.Forward(message));
            Receive<EachFileDeletedMessage>(message => { });

            //unknowns
            ReceiveAny(message => Sender.Tell("Unknown Message Received"));
        }
    }
}