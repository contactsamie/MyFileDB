using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFileDB.Common.Services;
using MyFileDB.DBApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFileDB.Tests
{
    [TestClass]
    public class DatabaseTests : TestKit
    {
        [TestMethod]
        public void it_should_be_able_to_do_a_query_without_waiting()
        {

            var store = new FleStore()
            {
                FolderName = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            var documentStore = new MyFileDbDocumentStore(store, TestDependencyResolver.GetContainer());
            using (var session = documentStore.OpenSession())
            {
                var id = session.Create(new Order() { OrderNumber = 1000 }).Id;

                Order document = null;
                document = session.Load<Order>(id);
                Assert.IsTrue(document != null);

                document.OrderComments = new List<string>() { "wow" };
                session.SaveChanges();
                Order document2 = null;
                document2 = session.Load<Order>(id);
                Assert.IsTrue(document2 != null);

                Assert.AreEqual(document2.OrderComments.First(), document.OrderComments.First());

                Order document3 = null;
                AssertAwait(() =>
                {
                    document3 = session.Query<Order>(query => query.Where(x => x.OrderNumber == 1000)).First();
                });

                Assert.AreEqual(document3.OrderComments.First(), document.OrderComments.First());
            }
        }

        [TestMethod]
        public void it_should_be_able_to_do_a_query_without_waiting2()
        {
            

            var store = new FleStore()
            {
                FolderName = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            var documentStore = new MyFileDbDocumentStore(store, TestDependencyResolver.GetContainer());
            using (var session = documentStore.OpenSession())
            {
                decimal numberOfOrders = 1000;

                var ids = new List<string>();

                for (var i = 0; i < numberOfOrders; i++)
                {
                    ids.Add(session.Create(new Order() { OrderNumber = i * 1000, OrderComments = new List<string>() { "init-" + i } }).Id);
                }
                var id = ids[(int)Math.Ceiling((decimal)(numberOfOrders / 2))];
                Order document = null;

                AssertAwait(() =>
                {
                    //wait until consistent
                    var currentCount = session.Query<Order>().ToList().Count;
                    Assert.AreEqual(currentCount, numberOfOrders);
                });
                document = session.Load<Order>(id);
                Assert.IsTrue(document != null);
                document.OrderComments = new List<string>() { "wow" };
                session.SaveChanges();
                Order document2 = null;
                document2 = session.Load<Order>(id);
                Assert.IsTrue(document2 != null);

                Assert.AreEqual(document2.OrderComments.First(), document.OrderComments.First());

                List<Order> documentlist = null;
                AssertAwait(() =>
                {
                    documentlist = session.Query<Order>().ToList();
                });

                Assert.AreEqual(documentlist.Count, numberOfOrders);
            }
        }

        [TestMethod]
        public void it_should_be_able_to_do_a_query()
        {
            var store = new FleStore()
            {
                FolderName = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            var documentStore = new MyFileDbDocumentStore(store, TestDependencyResolver.GetContainer());
            using (var session = documentStore.OpenSession())
            {
                var id = session.Create(new Order() { OrderNumber = 1000 }, true).Id;

                Order document = null;
                AssertAwait(() =>
                {
                    document = session.Load<Order>(id);
                    Assert.IsTrue(document != null);
                });

                document.OrderComments = new List<string>() { "wow" };
                session.SaveChanges(true);
                Order document2 = null;
                AssertAwait(() =>
                {
                    document2 = session.Load<Order>(id);
                    Assert.IsTrue(document2 != null);
                });

                Assert.AreEqual(document2.OrderComments.First(), document.OrderComments.First());

                Order document3 = null;
                AssertAwait(() =>
                {
                    document3 = session.Query<Order>(query => query.Where(x => x.OrderNumber == 1000)).First();
                });

                Assert.AreEqual(document3.OrderComments.First(), document.OrderComments.First());
            }
        }

        public void AssertAwait(Action assert)
        {
            const int maxCount = 10000;
            var counter = 0;
            while (counter < maxCount)
            {
                System.Threading.Thread.Sleep(1000 * counter);
                try
                {
                    assert();
                    Console.WriteLine("Successfull assert after waiting for " + 50 * counter + " Milliseconds");
                    return;
                }
                catch (Exception)
                {
                    counter++;
                }
            }
            throw new Exception("Waited for " + 50 * counter + " Milliseconds but assert failed " + assert);
        }

        public class Order
        {
            public int OrderNumber { set; get; }
            public List<string> OrderComments { set; get; }
        }
    }
}