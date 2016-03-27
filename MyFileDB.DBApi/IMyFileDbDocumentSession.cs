using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.DBApi
{
    public interface IMyFileDbDocumentSession:IDisposable
    {
        MyFileDbDocumentStore DocumentStore { get; }

        void Delete<T>(string id);

        T Load<T>(string id) where T : class;

        void Create<T>(T document,string id);

        void SaveChanges();

        IEnumerable<T> Query<T>(Func<IQueryable<T>, IQueryable<T>> queryFunction ) where T : class;
    }
}