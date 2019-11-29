using System.Threading.Tasks;

namespace AzureTemplates.ServiceBus.Services
{
  public interface IMessageProcessor
  {
    Task ProcessMessage();
  }
}
