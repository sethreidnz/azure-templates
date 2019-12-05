using System.Threading.Tasks;

namespace AzureTemplates.ServiceBus.Services
{
  public interface IMessageProducer
  {
    Task SendMessagesAsync(int numberOfMessagesToSend);
  }
}
