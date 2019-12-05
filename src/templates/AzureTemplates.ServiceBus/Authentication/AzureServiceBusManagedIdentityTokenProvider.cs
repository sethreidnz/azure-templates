using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;

namespace AzureTemplates.ServiceBus.Consumer.Authentication
{
  public class AzureServiceBusManagedIdentityTokenProvider : ITokenProvider
  {
    private readonly string _managedIdentityConnectionString;

    public AzureServiceBusManagedIdentityTokenProvider(string managedIdentityConnectionString)
    {
      _managedIdentityConnectionString = managedIdentityConnectionString;
    }

    public async Task<SecurityToken> GetTokenAsync(string appliesTo, TimeSpan timeout)
    {
      string accessToken = await GetAccessToken("https://servicebus.azure.net/");
      return new JsonSecurityToken(accessToken, appliesTo);
    }

    public async Task<string> GetAccessToken(string resource)
    {
      var authProvider = new AzureServiceTokenProvider(_managedIdentityConnectionString);
      return await authProvider.GetAccessTokenAsync(resource);
    }
  }
}
