using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AzureTemplates.ServiceBus.Options;
using AzureTemplates.ServiceBus.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AzureTemplates.ServiceBus.Consumer
{
  public class Program
  {
    public static async Task Main()
    {
      // setup configuration
      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddUserSecrets(Assembly.GetExecutingAssembly())
        .Build();

      // setup keyvault
      var keyVaultEndpoint = config.GetValue<string>("KeyVaultEndpoint");
      var environment = config.GetValue<string>("Environment");
      var managedIdentityClientId = config.GetValue<string>("ManagedIdentity:ClientId");
      var azureServiceTokenProvider = new AzureServiceTokenProvider(environment == "local" ? null : $"RunAs=App;AppId={managedIdentityClientId}");
      var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

      // get a secret from keyvault
      var secret = await keyVaultClient.GetSecretAsync(keyVaultEndpoint, "KeyVaultSecret");

      // create service collection for dependency injection
      var services = new ServiceCollection();

      // configure options from appsettings.json
      services.Configure<ManagedIdentityOptions>(config.GetSection("ManagedIdentity"));

      // configure logging
      services.AddLogging(logging =>
      {
        logging.AddConfiguration(config.GetSection("Logging"));
        logging.AddConsole();
        logging.AddDebug();
        logging.AddApplicationInsights(config.GetValue<string>("ApplicationInsightsKey"));
      });

      // configure services
      services.AddSingleton<IMessageProcessor, MessageProcessor>();

      // build service provider from the service collection
      var serviceProvider = services.BuildServiceProvider();

      // get the message processor service from the service provider
      var messageProcessor = serviceProvider.GetService<IMessageProcessor>();

      // process the message
      await messageProcessor.ProcessMessage();
    }
  }
}
