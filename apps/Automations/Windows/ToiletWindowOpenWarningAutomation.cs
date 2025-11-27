using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class ToiletWindowOpenWarningAutomation : WindowOpenWarningAutomation<ToiletWindowOpenWarningAutomation>
  {
    public ToiletWindowOpenWarningAutomation(
      IHaContext haContext,
      ILogger<ToiletWindowOpenWarningAutomation> logger,
      IPiperTtsService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "WC Fenster",
        WindowEntity = Entities.BinarySensor.WcFensterStatus,
      };
    }
  }
}
