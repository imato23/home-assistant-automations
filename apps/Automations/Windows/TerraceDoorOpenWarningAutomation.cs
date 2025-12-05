using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class TerraceDoorOpenWarningAutomation : WindowOpenWarningAutomation<TerraceDoorOpenWarningAutomation>
  {
    public TerraceDoorOpenWarningAutomation(
      IHaContext haContext,
      ILogger<TerraceDoorOpenWarningAutomation> logger,
      INotificationService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "Terrassentür",
        WindowEntity = Entities.BinarySensor.WohnzimmerTerrassentuerStatus
      };
    }
  }
}
