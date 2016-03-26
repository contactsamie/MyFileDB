using System;
using Autofac;
using MyFileDB.Common.Services;


namespace MyFileDB.Tests
{
    public class TestDependencyResolver
    {
        private static Autofac.IContainer Container { set; get; }

        public static IContainer GetContainer(Action<ContainerBuilder> builderFunc = null)
        {
            if (Container != null)
            {
                return Container;
            }
            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryFileService>().As<IFileService>();
            if (builderFunc != null)
            {
                builderFunc(builder);
            }

            Container = builder.Build();
            return Container;
        }
    }
}