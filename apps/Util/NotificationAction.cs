namespace HomeAssistantAutomations.apps.Util;

public class NotificationAction
{
  public string Id { get; }
  public string Title { get; }

  public NotificationAction(string id, string title)
  {
    Id = id;
    Title = title;
  }
}
