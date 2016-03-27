using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Actors;
using MyFileDB.Core.Messages;

namespace MyFileDB.Core
{
    public class SystemActor : ReceiveActor
    {
        public static RouterConfig CommonRouterConfig { set; get; }
        private IActorRef FileStorageBridgeCoOrdinatorRef { get; set; }
        private IActorRef FileQueryBridgeCoOrdinatorRef { set; get; }
        private IActorRef FileDeleteBridgeCoOrdinatorActorRef { set; get; }
        private IActorRef FileUpdateBridgeCoOrdinatorActorRef { set; get; }

        public SystemActor(RouterConfig routerConfig=null)
        {
            CommonRouterConfig = routerConfig?? new RoundRobinPool(5);//throughput things
            //pinginh
            Receive<PingMessage>(message => Sender.Tell(new PongMessage()));

            //storing
            FileStorageBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileStorageBridgeCoOrdinatorActor>().WithRouter(CommonRouterConfig));
            Receive<StoreFilesMessage>(message => FileStorageBridgeCoOrdinatorRef.Forward(message));
            Receive<EachFileStoredMessage>(message => { });

            //querying
            //cant do routing since it holds cache of query result
            FileQueryBridgeCoOrdinatorRef = Context.ActorOf(Context.System.DI().Props<FileQueryBridgeCoOrdinatorActor>());
            Receive<LoadAllFileContentMessage>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));
            Receive<LoadFileContentsResultMessages>(message =>{ });
            //direct file access
            Receive<LoadFileContentMessage>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));


            //bulkloading
            Receive<ListAllFilesByFolderNameMessage>(message => FileQueryBridgeCoOrdinatorRef.Forward(message));


            //deleting
            FileDeleteBridgeCoOrdinatorActorRef= Context.ActorOf(Context.System.DI().Props<FileDeleteBridgeCoOrdinatorActor>().WithRouter(CommonRouterConfig));
            Receive<DeleteFilesMessage>(message => FileDeleteBridgeCoOrdinatorActorRef.Forward(message));
            Receive<EachFileDeletedMessage>(message => { });

            //update
            FileUpdateBridgeCoOrdinatorActorRef = Context.ActorOf(Context.System.DI().Props<FileUpdateBridgeCoOrdinatorActor>().WithRouter(CommonRouterConfig));
            Receive<UpdateFilesMessage>(message => FileUpdateBridgeCoOrdinatorActorRef.Forward(message));
            Receive<EachFileUpdatedMessage>(message => { });


         
            //unknowns
            ReceiveAny(message => Sender.Tell("Unknown Message Received"));
        }
    }
}