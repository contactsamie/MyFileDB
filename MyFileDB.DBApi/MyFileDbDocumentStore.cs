using Akka.Actor;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core;
using MyFileDB.DependencyLib;
using System;
using Akka.Routing;
using Autofac;

namespace MyFileDB.DBApi
{
    public class MyFileDbDocumentStore : IDisposable
    {
        internal IActorRef MyFileDbActorRef { set; get; }

        internal IFleStore Store { set; get; }

        internal IMyFileDbDocumentSession Session { set; get; }

        public IMyFileDbDocumentSession OpenSession()
        {
            if (Session == null) throw new Exception(nameof(Session)+" Not Set");
            return Session;
        }

        private const string RootDbName = "MyFileDB";

        public MyFileDbDocumentStore(IFleStore store,IContainer container, RouterConfig routerConfig=null)
        {
            //todo allow passing of routerConfig
            if (store == null) throw new ArgumentNullException(nameof(store));
            store.RootPath = (store.RootPath ?? "") + "\\" + RootDbName;
            store.FolderName = store.FolderName ?? "";
            Store = store;

            if (ApplicationActorSystem.ActorReferences.ApplicationActorRef != null) return;
            ApplicationActorSystem.Create<SystemActor>(container);
            MyFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            Session=new MyFileDbDocumentSession(this);
        }



        public void Dispose()
        {
            ApplicationActorSystem.ShutDown();
        }
    }
}