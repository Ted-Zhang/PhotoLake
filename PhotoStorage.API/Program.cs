using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace PhotoStorage.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    if (ctx.HostingEnvironment.IsProduction())
                    {
                        var config = builder.Build();
                        var keyVaultName = config["KeyVaultName"];

                        if (!string.IsNullOrEmpty(keyVaultName))
                        {
                            var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            var keyVaultClient = new KeyVaultClient(
                                new KeyVaultClient.AuthenticationCallback(
                                    azureServiceTokenProvider.KeyVaultTokenCallback));
                            builder.AddAzureKeyVault(
                                $"https://{keyVaultName}.vault.azure.net/",
                                keyVaultClient, 
                                new DefaultKeyVaultSecretManager());
                        }
                    }
                })
                .UseStartup<Startup>();
    }
}
