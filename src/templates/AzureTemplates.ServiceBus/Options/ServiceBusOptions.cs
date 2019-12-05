namespace AzureTemplates.ServiceBus.Options
{
  public class ServiceBusOptions
  {
    public string Resource { get; set; }

    public string Namespace { get; set; }

    public Queues Queues { get; set; }
  }

  public class Queues
  {
    public string MyMessageQueue { get; set; }
  }
}
