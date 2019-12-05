using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace AzureTemplates.ServiceBus.Consumer.Services
{
  public interface IMessageProcessor
  {
    void Configure();

    Task ProcessMessagesAsync(Message message, CancellationToken token);
  }
}
