using HomeAssistantAutomations.apps.Util;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  [NetDaemonApp]
  public class OfficeWindowOpenWarningAutomation : WindowOpenWarningAutomation<OfficeWindowOpenWarningAutomation>
  {
    public OfficeWindowOpenWarningAutomation(
      IHaContext haContext,
      ILogger<OfficeWindowOpenWarningAutomation> logger,
      INotificationService piperTtsService)
    : base(haContext, logger, piperTtsService)
    {
    }

    protected override WindowMetadata Initialize()
    {
      return new WindowMetadata
      {
        WindowName = "Büro Fenster",
        WindowEntity = Entities.BinarySensor.BueroFensterStatus,
      };
    }
  }
}
