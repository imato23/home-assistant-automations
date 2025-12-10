using HomeAssistantAutomations.apps.Util;
using Microsoft.Extensions.DependencyInjection;

public static class CustomServiceCollectionExtensions
{
  public static IServiceCollection AddCustomRegistrations(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddScoped<INotificationService, NotificationService>();
    serviceCollection.AddScoped<IDeviceTrackerService, DeviceTrackerService>();

    return serviceCollection;
  }
}
