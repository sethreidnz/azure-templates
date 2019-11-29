using System;
using System.Threading.Tasks;
using AzureTemplates.ServiceBus.Options;
using Microsoft.Extensions.Options;

namespace AzureTemplates.ServiceBus.Services
{
  public class MessageProcessor : IMessageProcessor
  {
    private readonly ManagedIdentityOptions _managedIdentity;

    public MessageProcessor(IOptions<ManagedIdentityOptions> managedIdentity)
    {
      _managedIdentity = managedIdentity?.Value;
    }

    public Task ProcessMessage()
    {
      throw new NotImplementedException();
    }
  }
}
