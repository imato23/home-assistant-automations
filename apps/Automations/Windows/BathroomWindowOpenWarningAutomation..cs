using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class BathroomWindowOpenWarningAutomation : WindowOpenWarningAutomation<BathroomWindowOpenWarningAutomation>
  {
    public BathroomWindowOpenWarningAutomation(
      IHaContext haContext,
      ILogger<BathroomWindowOpenWarningAutomation> logger,
      IPiperTtsService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "Badezimmer Fenster",
        WindowEntity = Entities.BinarySensor.BadezimmerFensterStatus,
      };
    }
  }
}
