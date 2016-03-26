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
            AwaitAssert(() => ExpectMsg<EachFileStoredMessage>());
        }

        [TestMethod]
        public void it_should_be_able_to_load_a_file_from_a_folder()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);

            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            //Act
            myFileDbActorRef.Tell(new LoadFileContentMessages(new List<LoadFileContentMessage>()
            {
             new   LoadFileContentMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Akka-files", "sample-" + DateTime.Now.Ticks + ".json")
            }));

            //Assert
            AwaitAssert(() => ExpectMsg<LoadFileContentsResultMessages>(x => x.FileContentMessages != null));
        }

        [TestMethod]
        public void when_file_list_is_requested_it_should_return_all()
        {
            //Arrange
            ApplicationActorSystem.Create<SystemActor>(TestDependencyResolver.GetContainer(_builderMethod), Sys);
            var myFileDbActorRef = ApplicationActorSystem.ActorReferences.ApplicationActorRef;

            //Act
            myFileDbActorRef.Tell(new ListAllFilesByFolderNameMessage(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Akka-files"));

            //Assert
            AwaitAssert(() => ExpectMsg<FilesByFolderResultMessage>());
        }
    }
}