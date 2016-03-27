using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using MyFileDB.Core.Messages;
using System;

namespace MyFileDB.Core.Actors
{
    public class FileUpdateBridgeCoOrdinatorActor : ReceiveActor
    {
        private IActorRef FileUpdateBridgeActorRef { set; get; }

        public FileUpdateBridgeCoOrdinatorActor()
        {
            FileUpdateBridgeActorRef = Context.System.ActorOf(Context.System.DI().Props<FileUpdateBridgeActor>().WithRouter(SystemActor.CommonRouterConfig));

            Receive<UpdateFilesMessage>(message =>
            {
                foreach (var updateOneFileMessage in message.UpdateOneFileMessages)
                {
                    FileUpdateBridgeActorRef.Forward(updateOneFileMessage);
                }

                Sender.Tell(new FilesUpdatedMessage());
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(10, TimeSpan.FromSeconds(30), x => Directive.Restart);
        }
    }
}