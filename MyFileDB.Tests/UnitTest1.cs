using Akka.Actor;
using Akka.TestKit.VsTest;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFileDB.ActorSystemLib;
using MyFileDB.Common.Services;
using MyFileDB.Core;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyFileDB.Tests
{
    [TestClass]
    public class UnitTest1 : TestKit
    {
        [TestInitialize]
        public void SetUp()
        {
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(), Sys);
        }

        [TestCleanup]
        public void TearDown()
        {
            ApplicationActorSystem.ShutDown();
            InMemoryFileService.InMemoryFileStorage = new Dictionary<string, string>();
        }

        [TestMethod]
        public void it_should_respond_to_ping()
        {
            //Arrange

            // ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(), Sys);
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            //Act
            myFileDbActorRef.Tell(new PingMessage());

            //Assert
            AwaitAssert(() => ExpectMsg<PongMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_store_files_into_a_folder()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            //Act
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileIdentityMessage>()
            {
                new StoreOneFileIdentityMessage()
                {
                    FileName = "sample-"+DateTime.Now.Ticks+".json",
                    FolderName = "Akka-files",
                    RootPath =  System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FileContent = new FileContent( DateTime.Now.ToString(CultureInfo.CurrentCulture),Guid.NewGuid().ToString(),"")
                }
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<FilesStoredMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_store_a_file_into_a_folder()
        {
            //Arrange
            // ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //Act
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileIdentityMessage>()
            {
                new StoreOneFileIdentityMessage()
                {
                    FileName =fileNme,
                    FolderName = folder,
                    RootPath =  rootPath,
                    FileContent =new FileContent( fileContent,Guid.NewGuid().ToString(),"")
                }
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<EachFileStoredMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_load_a_file_from_a_folder()
        {
            //Arrange
            //  ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            myFileDbActorRef.Tell(new LoadAllFileContentMessage(new List<LoadFileContentMessage>()
            {
             new   LoadFileContentMessage(rootPath,folder, fileNme,TestActor,null)
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1 && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(20));
        }

        [TestMethod]
        public void a_file_store_should_update_file_cache()
        {
            //Arrange
            // ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1 && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(20));
        }

        [TestMethod]
        public void it_should_be_able_to_load_many_files_from_a_folder()
        {
            //Arrange
            // ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            var loadFileContentMessages = new List<LoadFileContentMessage>();
            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
                loadFileContentMessages.Add(new LoadFileContentMessage(rootPath, folder, fileNme, TestActor,null));
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(20));
        }

        private void StoreFile(IActorRef myFileDbActorRef, List<string> fileNmes, string folder, string rootPath,
            string fileContent)
        {
            foreach (var fileNme in fileNmes)
            {
                StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);
            }
        }

        private void StoreFile(IActorRef myFileDbActorRef, string fileNme, string folder, string rootPath, string fileContent)
        {
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileIdentityMessage>()
            {
                new StoreOneFileIdentityMessage()
                {
                    FileName = fileNme,
                    FolderName = folder,
                    RootPath = rootPath,
                    FileContent = new FileContent( fileContent,Guid.NewGuid().ToString(),"")
                }
            }));

            AwaitAssert(() => ExpectMsg<EachFileStoredMessage>(x => x.FileStoredMessage.FileName == fileNme && x.FileStoredMessage.FileContent.Body.ToString() == fileContent));
        }

        [TestMethod]
        public void when_file_list_is_requested_it_should_return_all()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            var loadFileContentMessages = new List<LoadFileContentMessage>();
            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
                loadFileContentMessages.Add(new LoadFileContentMessage(rootPath, folder, fileNme, TestActor,null));
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);
            /*
            actors are communicating through sending messages to each other mailboxes.
            Now think, how to implement request/response when having two inboxes in each direction.
            You'd need to scan whole mailbox for reply... or create an ultraweight actor dedicated to
            get the reply and wrap it in the task. Akka uses second option :wink:
            you can imagine, it's a lot slower than using Tell
            ... from @Horusiath
            */
            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);
        }

        [TestMethod]
        public void it_should_be_able_to_delete_a_file_from_folder()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1 && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            myFileDbActorRef.Tell(new DeleteFilesMessage(new List<DeleteOneFileIdentityMessage>()
            {
                new DeleteOneFileIdentityMessage(null)
                {
                    FileName = fileNme,
                    FolderName = folder,
                    RootPath = rootPath
                }
            }));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 0), TimeSpan.FromSeconds(60));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, 0);
        }

        [TestMethod]
        public void it_should_be_able_to_delete_a_file_among_other_files_from_folder()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            var loadFileContentMessages = new List<LoadFileContentMessage>();
            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
                loadFileContentMessages.Add(new LoadFileContentMessage(rootPath, folder, fileNme, TestActor,null));
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            myFileDbActorRef.Tell(new DeleteFilesMessage(new List<DeleteOneFileIdentityMessage>()
            {
                new DeleteOneFileIdentityMessage(null)
                {
                    FileName = fileNames[0],
                    FolderName = folder,
                    RootPath = rootPath
                }
            }));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles - 1 && !x.FileContentMessages.Exists(y => y.FileName == fileNames[0])), TimeSpan.FromSeconds(60));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles - 1);
        }

        [TestMethod]
        public void it_should_be_able_to_delete_all_files_from_folder()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            var deleteFileContentMessages = new List<DeleteOneFileIdentityMessage>();
            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
                deleteFileContentMessages.Add(new DeleteOneFileIdentityMessage(null)
                {
                    FileName = fileNme,
                    FolderName = folder,
                    RootPath = rootPath
                });
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            myFileDbActorRef.Tell(new DeleteFilesMessage(deleteFileContentMessages));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 0), TimeSpan.FromSeconds(60));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, 0);
        }

        [TestMethod]
        public void it_should_be_able_to_update_a_file_in_folder()
        {
            //Arrange
            //ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1 && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            var sampleFile = files.First();
            var oldContent = sampleFile.FileContent;
            var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");

            myFileDbActorRef.Tell(new UpdateFilesMessage(new List<UpdateOneFileIdentityMessage>()
            {
                new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath,
                   // FileContent = newContent
                }
            }));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1), TimeSpan.FromSeconds(60));

            files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert

            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, 1);
            Assert.AreEqual(files.First().FileContent.Body, newContent.Body);
            Assert.AreNotEqual(oldContent.Etag, files.First().FileContent.Etag);
            Assert.AreNotEqual(oldContent.Body, files.First().FileContent.Body);
        }

        [TestMethod]
        public void it_should_be_able_to_update_one_of_all_files_in_folder()
        {
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            var sampleFile = files.First();
            var oldContent = sampleFile.FileContent;
            var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");

            myFileDbActorRef.Tell(new UpdateFilesMessage(new List<UpdateOneFileIdentityMessage>()
            {
                new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath
                }
            }));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles), TimeSpan.FromSeconds(60));

            files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            var resultFile = files.Find(x => x.FileName == sampleFile.FileName);
            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);
            Assert.AreEqual(resultFile.FileContent.Body, newContent.Body);
            Assert.AreNotEqual(oldContent.Etag, resultFile.FileContent.Etag);
            Assert.AreNotEqual(oldContent.Body, resultFile.FileContent.Body);
        }


        [TestMethod]
        public void it_should_be_able_to_update_one_of_all_files_in_folder2()
        {
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            var sampleFile = files.First();
            var oldContent = sampleFile.FileContent;
            var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");

            myFileDbActorRef.Tell(new UpdateFilesMessage(new List<UpdateOneFileIdentityMessage>()
            {
                new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath,
                   // FileContent = newContent
                }
            }));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles), TimeSpan.FromSeconds(60));

            var resultFile = myFileDbActorRef.Ask(new LoadFileContentMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files",sampleFile.FileName,null,null)).Result as FileContentUpdateMessage;

            //Assert
           // var resultFile = files.Find(x => x.FileName == sampleFile.FileName);
            Assert.IsTrue(resultFile != null);
            Assert.AreEqual(resultFile.FileContent.Body, newContent.Body);
            Assert.AreNotEqual(oldContent.Etag, resultFile.FileContent.Etag);
            Assert.AreNotEqual(oldContent.Body, resultFile.FileContent.Body);
        }


        [TestMethod]
        public void it_should_be_able_to_update_all_files_in_folder()
        {
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            files.ForEach(x =>
            {
                Assert.IsFalse(x.FileContent.Body.ToString().EndsWith("NEW"));
            });
            files.ForEach(sampleFile =>
            {
                var oldContent = sampleFile.FileContent;
                var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");

                myFileDbActorRef.Tell(new UpdateFilesMessage(new List<UpdateOneFileIdentityMessage>()
            {
                new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath,
                   // FileContent = newContent
                }
            }));
            });

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles), TimeSpan.FromSeconds(60));

            files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert

            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);

            files.ForEach(x =>
            {
                Assert.IsTrue(x.FileContent.Body.ToString().EndsWith("NEW"));
            });
        }

        [TestMethod]
        public void it_should_be_able_to_update_some_files_in_folder()
        {
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            files.ForEach(x =>
            {
                Assert.IsFalse(x.FileContent.Body.ToString().EndsWith("NEW"));
            });

            var counter = 0;

            files.ForEach(sampleFile =>
            {
                if (counter > 4)
                {
                    return;
                }
                counter++;
                var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");

                myFileDbActorRef.Tell(new UpdateFilesMessage(new List<UpdateOneFileIdentityMessage>()
            {
                new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath,
                   // FileContent = newContent
                }
            }));
            });

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles), TimeSpan.FromSeconds(60));

            files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert

            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);
            Assert.AreEqual(files.Where(x => x.FileContent.Body.ToString().EndsWith("NEW")).ToList().Count, 5);
        }

        [TestMethod]
        public void it_should_be_able_to_bulk_update_all_files_in_folder()
        {
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var folder = "Akka-files";
            var totalNumberOfFiles = 10;

            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fileNames = new List<string>();

            for (var i = 0; i < totalNumberOfFiles; i++)
            {
                var fileNme = "sample-" + i + ".json";
                fileNames.Add(fileNme);
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent.Body.ToString() == fileContent)), TimeSpan.FromSeconds(100));

            var files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            files.ForEach(x =>
            {
                Assert.IsFalse(x.FileContent.Body.ToString().EndsWith("NEW"));
            });
            var messageList = new List<UpdateOneFileIdentityMessage>();
            files.ForEach(sampleFile =>
            {
                var oldContent = sampleFile.FileContent;
                var newContent = new FileContent(sampleFile.FileContent.Body.ToString() + "NEW", sampleFile.FileContent.Etag, "");
                messageList.Add(new UpdateOneFileIdentityMessage(newContent)
                {
                    FileName = sampleFile.FileName,
                    FolderName = sampleFile.FolderName,
                    RootPath = sampleFile.RootPath,
                  //  FileContent = newContent
                });
            });
            myFileDbActorRef.Tell(new UpdateFilesMessage(messageList));

            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles), TimeSpan.FromSeconds(60));

            files = myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert

            Assert.IsTrue(files != null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);

            files.ForEach(x =>
            {
                Assert.IsTrue(x.FileContent.Body.ToString().EndsWith("NEW"));
            });
        }
    }
}