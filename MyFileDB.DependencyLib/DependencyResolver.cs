using Autofac;
using System;
using MyFileDB.Core.Services;

namespace MyFileDB.DependencyLib
{
    public class DependencyResolver
    {
        private static Autofac.IContainer Container { set; get; }

        public static IContainer GetContainer(Action<ContainerBuilder> builderFunc = null)
        {
            if (Container != null)
            {
                return Container;
            }
            var builder = new ContainerBuilder();
            builder.RegisterType<FileService>().As<IFileService>();
            if (builderFunc != null)
            {
                builderFunc(builder);
            }

            Container = builder.Build();
            return Container;
        }
    }
}