using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
  public class WindowMetadata
  {
    public required BinarySensorEntity WindowEntity { get; set; }
    public required string WindowName { get; set; }
    public TimeSpan DefaultWarningInterval { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan ShortWarningInterval { get; set; } = TimeSpan.FromMinutes(60);
    public TimeSpan LongWarningInterval { get; set; } = TimeSpan.FromMinutes(120);
  }
}
