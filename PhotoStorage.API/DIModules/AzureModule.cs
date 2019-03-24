using Autofac;
using PhotoStorage.Service;

namespace PhotoStorage.API.DIModules
{
    public class AzureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureFileStorageService>().As<IAzureFileStorageService>();
        }
    }
}
