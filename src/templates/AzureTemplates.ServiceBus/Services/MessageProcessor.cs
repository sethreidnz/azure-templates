using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace AzureTemplates.ServiceBus.Consumer.Services
{
  public class MessageProcessor : IMessageProcessor
  {
    private readonly ILogger<MessageProcessor> _logger;
    private readonly IQueueClient _queueClient;

    public MessageProcessor(
      ILogger<MessageProcessor> logger,
      IQueueClient queueClient)
    {
      _logger = logger;
      _queueClient = queueClient;
    }

    public void Configure()
    {
      var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
      {
        MaxConcurrentCalls = 1,
        AutoComplete = false
      };

      // Register the function that processes messages.
      _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
    }

    public async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
      if (message == null)
      {
        return;
      }

      _logger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

      // process the message here
      await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
      _logger.LogCritical($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
      var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
      _logger.LogCritical("Exception context for troubleshooting:");
      _logger.LogCritical($"- Endpoint: {context.Endpoint}");
      _logger.LogCritical($"- Entity Path: {context.EntityPath}");
      _logger.LogCritical($"- Executing Action: {context.Action}");
      return Task.CompletedTask;
    }
  }
}
