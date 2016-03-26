using Akka.Actor;
using Akka.TestKit.VsTest;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFileDB.ActorSystemLib;
using MyFileDB.Core;
using MyFileDB.Core.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyFileDB.Tests
{
    [TestClass]
    public class UnitTest1 : TestKit
    {
        private readonly Action<ContainerBuilder> _builderMethod = (builder) => builder.RegisterType<SystemActor>();

        [TestMethod]
        public void it_should_respond_to_ping()
        {
            //Arrange

            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(), Sys);
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
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            //Act
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileMessage>()
            {
                new StoreOneFileMessage()
                {
                    FileName = "sample-"+DateTime.Now.Ticks+".json",
                    FolderName = "Akka-files",
                    RootPath =  System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture)
                }
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<FilesStoredMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_store_a_file_into_a_folder()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //Act
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileMessage>()
            {
                new StoreOneFileMessage()
                {
                    FileName =fileNme,
                    FolderName = folder,
                    RootPath =  rootPath,
                    FileContent = fileContent
                }
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<EachFileStoredMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_load_a_file_from_a_folder()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            myFileDbActorRef.Tell(new LoadAllFileContentMessage(new List<LoadFileContentMessage>()
            {
             new   LoadFileContentMessage(rootPath,folder, fileNme,TestActor)
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x=>x.FileContentMessages.Count==1 && x.FileContentMessages.TrueForAll(y=>y.FileContent== fileContent) ),TimeSpan.FromSeconds(20));
        }

        [TestMethod]
        public void a_file_store_should_update_file_cache()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;
            var fileContent = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var fileNme = "sample-" + DateTime.Now.Ticks + ".json";
            var folder = "Akka-files";
            var rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //Act
            StoreFile(myFileDbActorRef, fileNme, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == 1 && x.FileContentMessages.TrueForAll(y => y.FileContent == fileContent)), TimeSpan.FromSeconds(20));
        }


        [TestMethod]
        public void it_should_be_able_to_load_many_files_from_a_folder()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

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
                loadFileContentMessages.Add(new LoadFileContentMessage(rootPath, folder, fileNme, TestActor));
            }

            //Act
            StoreFile(myFileDbActorRef, fileNames, folder, rootPath, fileContent);

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages.Count == totalNumberOfFiles && x.FileContentMessages.TrueForAll(y => y.FileContent == fileContent)), TimeSpan.FromSeconds(20));
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
            myFileDbActorRef.Tell(new StoreFilesMessage(new List<StoreOneFileMessage>()
            {
                new StoreOneFileMessage()
                {
                    FileName = fileNme,
                    FolderName = folder,
                    RootPath = rootPath,
                    FileContent = fileContent
                }
            }));

            AwaitAssert(() => ExpectMsg<EachFileStoredMessage>());
        }


        [TestMethod]
        public void when_file_list_is_requested_it_should_return_all()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

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
                loadFileContentMessages.Add(new LoadFileContentMessage(rootPath, folder, fileNme, TestActor));
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
            var files =  myFileDbActorRef.Ask(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files")).Result as List<FileContentUpdateMessage>;

            //Assert
            Assert.IsTrue(files!=null);
            Assert.AreEqual(files.Count, totalNumberOfFiles);
        }
    }
}