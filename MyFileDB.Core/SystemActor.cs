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

        public SystemActor()
        {
            Receive<PingMessage>(message => Sender.Tell(new PongMessage()));

            FileStorageBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileStorageBridgeCoOrdinatorActor>());
            Receive<StoreFilesMessage>(message => FileStorageBridgeCoOrdinatorRef.Forward(message));
            Receive<EachFileStoredMessage>(message => { });

            FileQueryBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileQueryBridgeCoOrdinatorActor>());
            Receive<LoadFileContentMessages>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));
            Receive<LoadFileContentsResultMessages>(message =>
            {
                
            });
        }
    }
}