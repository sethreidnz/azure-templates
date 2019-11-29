using System;
using System.Threading.Tasks;
using AzureTemplates.ServiceBus.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzureTemplates.ServiceBus.Services
{
  public class MessageProcessor : IMessageProcessor
  {
    private readonly ILogger<MessageProcessor> _logger;

    public MessageProcessor(ILogger<MessageProcessor> logger)
    {
      _logger = logger;
    }

    public async Task ProcessMessage()
    {
      _logger.LogInformation("Started processing message from MessageProcessor");
      await Task.Delay(1000);
      _logger.LogInformation("Finished processing message from MessageProcessor");
    }
  }
}
