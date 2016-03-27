using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Messages;
using System;

namespace MyFileDB.Core.Actors
{
    public class FileDeleteBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileDeleteBridgeActorRef { set; get; }

        public FileDeleteBridgeCoOrdinatorActor()
        {
            FileDeleteBridgeActorRef = Context.System.ActorOf(Context.System.DI().Props<FileDeleteBridgeActor>().WithRouter(SystemActor.CommonRouterConfig));

            Receive<DeleteFilesMessage>(message =>
            {
                foreach (var deleteOneFileMessage in message.DeleteOneFileMessages)
                {
                    FileDeleteBridgeActorRef.Forward(deleteOneFileMessage);
                }

                Sender.Tell(new FilesDeletedMessage());
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x => Directive.Restart);
        }
    }
}