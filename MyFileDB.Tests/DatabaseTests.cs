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
        public void it_should_be_able_to_do_a_query()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var id = Guid.NewGuid().ToString();
                session.Create(new Order() { OrderNumber = 1000 }, id);

                Order document = null;
                AssertAwait(() =>
                 {
                     document = session.Load<Order>(id);
                     Assert.IsTrue(document != null);
                 });

                document.OrderComments = new List<string>() { "wow" };
                session.SaveChanges();
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
            const int maxCount = 100;
            var counter = 0;
            while (counter < maxCount)
            {
                System.Threading.Thread.Sleep(50 * counter);
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

        private MyFileDbDocumentStore DocumentStore { set; get; }

        [TestInitialize]
        public void SetUp()
        {
            var store = new FleStore()
            {
                FolderName = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            DocumentStore = new MyFileDbDocumentStore(store, TestDependencyResolver.GetContainer());
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        public class Order
        {
            public int OrderNumber { set; get; }
            public List<string> OrderComments { set; get; }
        }
    }
}