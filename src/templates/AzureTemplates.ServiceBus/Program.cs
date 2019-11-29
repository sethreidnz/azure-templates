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

      var services = new ServiceCollection();

      // configure options from appsettings.json
      services.Configure<ManagedIdentityOptions>(config.GetSection("ManagedIdentity"));

      // configure services
      services.AddSingleton<IMessageProcessor, MessageProcessor>();

      var serviceProvider = services.BuildServiceProvider();

      // process the message
      var messageProcessor = serviceProvider.GetService<IMessageProcessor>();
      await Console.Out.WriteAsync("Processing Message");
      await messageProcessor.ProcessMessage();
      await Console.Out.WriteAsync("Successfully consumed message");
    }
  }
}
