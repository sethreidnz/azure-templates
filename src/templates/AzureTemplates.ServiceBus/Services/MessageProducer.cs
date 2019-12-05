using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace AzureTemplates.ServiceBus.Consumer.Services.
{
  public class MessageProducer : IMessageProducer
  {
    private readonly ILogger<MessageProcessor> _logger;
    private readonly IQueueClient _queueClient;

    public MessageProducer(
      ILogger<MessageProcessor> logger,
      IQueueClient queueClient)
    {
      _logger = logger;
      _queueClient = queueClient;
    }

    public async Task SendMessagesAsync(int numberOfMessagesToSend)
    {
      try
      {
        for (var i = 0; i < numberOfMessagesToSend; i++)
        {
          string messageBody = $"Message {i}";
          var message = new Message(Encoding.UTF8.GetBytes(messageBody));
          _logger.LogInformation($"Sending message: {messageBody}");
          await _queueClient.SendAsync(message);
        }
      }
      catch (Exception exception)
      {
        _logger.LogCritical($"{DateTime.Now} :: Exception: {exception.Message}");
      }
    }
  }
}
