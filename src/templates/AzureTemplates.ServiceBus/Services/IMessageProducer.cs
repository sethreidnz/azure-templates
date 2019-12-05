using System.Threading.Tasks;

namespace AzureTemplates.ServiceBus.Consumer.Services
{
  public interface IMessageProducer
  {
    Task SendMessagesAsync(int numberOfMessagesToSend);
  }
}
