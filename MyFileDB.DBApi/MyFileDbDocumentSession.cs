using Akka.Actor;
using MyFileDB.Common.Services;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.DBApi
{
    public class MyFileDbDocumentSession : IMyFileDbDocumentSession
    {
        public MyFileDbDocumentSession(MyFileDbDocumentStore documentStore)
        {
            DocumentStore = documentStore;
            CurrentLoads = new Dictionary<string, FileContentUpdateMessage>();
        }

        public MyFileDbDocumentStore DocumentStore { get; }

        public void Delete<T>(string id)
        {
            DocumentStore.MyFileDbActorRef.Tell(new DeleteFilesMessage(new List<DeleteOneFileIdentityMessage>()
            {
                new DeleteOneFileIdentityMessage(null)
                {
                    FileName = id+".json",
                    FolderName = DocumentStore.Store.FolderName,
                    RootPath = DocumentStore.Store.RootPath
                }
            }));
        }

        private Dictionary<string, FileContentUpdateMessage> CurrentLoads { set; get; }

        public T Load<T>(string id) where T : class
        {

            var result = DocumentStore.MyFileDbActorRef.Ask(new LoadFileContentMessage(DocumentStore.Store.RootPath, DocumentStore.Store.FolderName, id + ".json", null, typeof(T).AssemblyQualifiedName)).Result as FileContentUpdateMessage;

            if (CurrentLoads.ContainsKey(id))
            {
                CurrentLoads[id] = result;
            }
            else
            {
                CurrentLoads.Add(id, result);
            }
            return result?.FileContent?.Body as T;
        }

        public void Create<T>(T document, string id)
        {
            var newEtag = Guid.NewGuid().ToString();
            DocumentStore.MyFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileIdentityMessage>()
              {
                  new StoreOneFileIdentityMessage()
                  {
                      FileName =id+".json",
                      FolderName = DocumentStore.Store.FolderName,
                      RootPath =  DocumentStore.Store.RootPath,
                      FileContent =new FileContent( document,newEtag,typeof(T).AssemblyQualifiedName)
                  }
              }));
        }

        public void SaveChanges()
        {
            var updates = CurrentLoads.Select(load => new UpdateOneFileIdentityMessage(load.Value.FileContent)
            {
                FileName = load.Value.FileName,
                FolderName = load.Value.FolderName,
                RootPath = load.Value.RootPath
            }).ToList();
            if (updates.Count > 0)
            {
                DocumentStore.MyFileDbActorRef.Tell(new UpdateFilesMessage(updates));
            }
        }

        public IEnumerable<T> Query<T>(Func<IQueryable<T>, IQueryable<T>> queryFunction) where T : class
        {
            var files = DocumentStore.MyFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(DocumentStore.Store.RootPath, DocumentStore.Store.FolderName)).Result as IEnumerable<FileContentUpdateMessage>;
            var queryable = (files ?? new List<FileContentUpdateMessage>()).Select(x => x.FileContent.Body as T).AsQueryable();
            return queryFunction(queryable);
        }

        public void Dispose()
        {
          DocumentStore.Dispose();
        }
    }
}