using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AzureTemplates.ServiceBus.Options;
using AzureTemplates.ServiceBus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureTemplates.ServiceBus.Consumer
{
  public static class Program
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

      // create service collection for dependency injection
      var services = new ServiceCollection();

      // configure options from appsettings.json
      services.Configure<ManagedIdentityOptions>(config.GetSection("ManagedIdentity"));

      // configure services
      services.AddSingleton<IMessageProcessor, MessageProcessor>();

      // build service provider from the service collection
      var serviceProvider = services.BuildServiceProvider();

      // get the message processor service from the service provider
      var messageProcessor = serviceProvider.GetService<IMessageProcessor>();

      // process the message
      await Console.Out.WriteAsync("Processing Message");
      await messageProcessor.ProcessMessage();
      await Console.Out.WriteAsync("Successfully consumed message");
    }
  }
}
