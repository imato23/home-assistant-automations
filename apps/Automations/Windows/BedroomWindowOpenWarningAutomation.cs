using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class BedroomWindowOpenWarningAutomation : WindowOpenWarningAutomation<BedroomWindowOpenWarningAutomation>
  {
    public BedroomWindowOpenWarningAutomation(
      IHaContext haContext,
      ILogger<BedroomWindowOpenWarningAutomation> logger,
      INotificationService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "Schlafzimmer Fenster",
        WindowEntity = Entities.BinarySensor.SchlafzimmerFensterStatus,
      };
    }
  }
}
