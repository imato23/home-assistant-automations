using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class GuestroomWindowOpenWarningAutomation : WindowOpenWarningAutomation<GuestroomWindowOpenWarningAutomation>
  {
    public GuestroomWindowOpenWarningAutomation(
      IHaContext haContext,
      ILogger<GuestroomWindowOpenWarningAutomation> logger,
      INotificationService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "Gästezimmer Fenster",
        WindowEntity = Entities.BinarySensor.GaestezimmerFensterStatus,
      };
    }
  }
}
