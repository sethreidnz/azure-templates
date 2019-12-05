using System;
using System.IO;
using System.Threading.Tasks;
using AzureTemplates.ServiceBus.Consumer.Authentication;
using AzureTemplates.ServiceBus.Consumer.Options;
using AzureTemplates.ServiceBus.Consumer.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        .Build();

      // get configuration values
      var environment = config.GetValue<string>("Environment");
      var managedIdentityOptions = config.GetValue<string>("ManagedIdentity:ClientId");
      var managedIdentityConnectionString = environment == "local" ? null : $"RunAs=App;AppId={managedIdentityOptions}";

      // setup key vault
      var keyVaultEndpoint = config.GetValue<string>("KeyVaultEndpoint");
      var keyVaultTokenProvider = new AzureServiceTokenProvider(managedIdentityConnectionString);
      var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(keyVaultTokenProvider.KeyVaultTokenCallback));

      // get a secret from keyvault
      var secret = await keyVaultClient.GetSecretAsync(keyVaultEndpoint, "KeyVaultSecret");

      // create service collection for dependency injection
      var services = new ServiceCollection();

      // configure options from appsettings.json
      services.Configure<ManagedIdentityOptions>(config.GetSection("ManagedIdentity"));
      services.Configure<ServiceBusOptions>(config.GetSection("ServiceBus"));

      // configure logging
      services.AddLogging(logging =>
      {
        logging
          .AddConfiguration(config.GetSection("Logging"))
          .AddConsole()
          .AddDebug()
          .AddApplicationInsights(config.GetValue<string>("ApplicationInsightsKey"));
      });

      // configure services
      services.AddScoped<IMessageProcessor, MessageProcessor>();
      services.AddScoped<IMessageProducer, MessageProducer>();
      services.AddSingleton<IQueueClient>(s =>
      {
        var serviceBusOptions = s.GetService<IOptions<ServiceBusOptions>>().Value;
        return new QueueClient(
          $"{serviceBusOptions.Namespace}.{serviceBusOptions.Resource}",
          serviceBusOptions.Queues.MyMessageQueue,
          new AzureServiceBusManagedIdentityTokenProvider(managedIdentityConnectionString));
      });

      // build service provider from the service collection
      var serviceProvider = services.BuildServiceProvider();

      // get the message processor service from the service provider
      var messageProcessor = serviceProvider.GetService<IMessageProcessor>();
      var messageProducer = serviceProvider.GetService<IMessageProducer>();

      messageProcessor.Configure();

      await messageProducer.SendMessagesAsync(1);

      Console.ReadKey();
    }
  }
}
