using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Util;

/// <summary>
/// TTS service for piper speech engine
/// </summary>
public class NotificationService : INotificationService
{
  private readonly IHaContext _haContext;
  private readonly ILogger<NotificationService> _logger;
  private readonly IEntities _entities;
  private readonly IServices _services;
  private const string Domain = "tts";
  private const string Service = "speak";
  private const string ServiceTarget = "tts.piper";

  /// <summary>
  /// Creates a new instance of the <see cref="NotificationService"/>
  /// </summary>
  /// <param name="haContext">The HomeAssistant context</param>
  public NotificationService(IHaContext haContext, ILogger<NotificationService> logger)
  {
    _haContext = haContext;
    _logger = logger;
    _entities = new Entities(haContext);
    _services = new Services(haContext);
  }

  /// <summary>
  /// Speaks a text on an media player
  /// </summary>
  /// <param name="mediaPlayerEntityId">The media player entity ID</param>
  /// <param name="text">The text to speak</param>
  public void Speak(string mediaPlayerEntityId, string text)
  {
    ArgumentException.ThrowIfNullOrEmpty(mediaPlayerEntityId);
    ArgumentException.ThrowIfNullOrEmpty(text);

    _haContext.CallService(
      Domain,
      Service,
      NetDaemon.HassModel.Entities.ServiceTarget.FromEntity(ServiceTarget),
      new
      {
        cache = true,
        media_player_entity_id = mediaPlayerEntityId,
        message = text
      });
  }

  /// <summary>
  /// Speaks a text on an media player
  /// </summary>
  /// <param name="mediaPlayerEntity">The media player entity</param>
  /// <param name="text">The text to speak</param>
  public void Speak(MediaPlayerEntity mediaPlayerEntity, string text)
  {
    ArgumentNullException.ThrowIfNull(mediaPlayerEntity);
    ArgumentException.ThrowIfNullOrEmpty(text);

    Speak(mediaPlayerEntity.EntityId, text);
  }

  /// <summary>
  /// Speaks a text on all media players
  /// </summary>
  /// <param name="text">The text to speak</param>
  public void SpeakOnAllMediaPlayers(string text)
  {
    IEnumerable<HomeAssistantGenerated.MediaPlayerEntity> mediaPlayerEntities = [
                _entities.MediaPlayer.KucheLautsprecher,
                _entities.MediaPlayer.BrowserWandTablet];

    Parallel.ForEach(mediaPlayerEntities, mediaPlayerEntity =>
        {
          Speak(mediaPlayerEntity, text);
        });
  }

  /// <summary>
  /// Speaks a text on a smartphone
  /// </summary>
  /// <param name="smartphone">The smartphone</param>
  /// <param name="text">The texxt</param>
  /// <param name="useAlarmStream">If true, uses the alarm stream; else false</param>
  public void SpeakOnSmartphone(Smartphone smartphone, string text, bool useAlarmStream = false)
  {
    dynamic data = new ExpandoObject();
    data.tts_text = text;

    if (useAlarmStream)
    {
      data.media_stream = "alarm_stream";
    }

    switch (smartphone)
    {
      case Smartphone.All:

        _services.Notify.MobileAppSmartphoneSabine(
            message: "TTS",
            data: data);

        _services.Notify.MobileAppSmartphoneThomas(
            message: "TTS",
            data: data);
        break;

      case Smartphone.Sabine:
        _services.Notify.MobileAppSmartphoneSabine(
              message: "TTS",
              data: data);
        break;

      case Smartphone.Thomas:
        _services.Notify.MobileAppSmartphoneThomas(
              message: "TTS",
              data: data);
        break;
    }
  }

  /// <summary>
  /// Sends a notification message to a smart phone
  /// </summary>
  /// <param name="smartphone">The smart phone</param>
  /// <param name="title">The message title</param>
  /// <param name="message">The message text</param>
  /// <param name="tag">An optional string, if set previous messages with the same tag will be removed from the smart phone</param>
  /// <param name="timeoutInSeconds">If set, the message will be removed after the timout timespan</param>
  /// <param name="notificationActions">The notification actions</param>
  public void NotifySmartphone(
    Smartphone smartphone,
    string title,
    string message,
    string? tag = null,
    uint timeoutInSeconds = 0,
    IEnumerable<NotificationAction>? notificationActions = null)
  {
    dynamic data = new ExpandoObject();

    if (tag != null)
    {
      data.tag = tag;
    }

    if (timeoutInSeconds > 0)
    {
      data.timeout = timeoutInSeconds;
    }

    if (notificationActions != null)
    {
      data.actions = BuildNotificationActions(notificationActions);
    }

    switch (smartphone)
    {
      case Smartphone.All:
        _services.Notify.MobileAppSmartphoneSabine(
          message: message,
          title: title,
          data: data);

        _services.Notify.MobileAppSmartphoneThomas(
          message: message,
          title: title,
          data: data);
        break;

      case Smartphone.Sabine:
        _services.Notify.MobileAppSmartphoneSabine(
          message: message,
          title: title,
          data: data);
        break;

      case Smartphone.Thomas:
        _services.Notify.MobileAppSmartphoneThomas(
            message: message,
            title: title,
            data: data);
        break;
    }
  }

  /// <summary>
  /// Clears a notification on a smart phone
  /// </summary>
  /// <param name="smartphone">The smart phone</param>
  /// <param name="tag"></param>
  public void ClearSmartphoneNotification(Smartphone smartphone, string tag)
  {
    string message = "clear_notification";

    dynamic data = new ExpandoObject();
    data.tag = tag;

    switch (smartphone)
    {
      case Smartphone.All:
        _services.Notify.MobileAppSmartphoneSabine(
          message: message,
          data: data);

        _services.Notify.MobileAppSmartphoneThomas(
          message: message,
          data: data);
        break;

      case Smartphone.Sabine:
        _services.Notify.MobileAppSmartphoneSabine(
          message: message,
          data: data);
        break;

      case Smartphone.Thomas:
        _services.Notify.MobileAppSmartphoneThomas(
            message: message,
            data: data);
        break;
    }
  }

  private object[] BuildNotificationActions(IEnumerable<NotificationAction> notificationActions)
  {
    bool hasDuplicationIds = notificationActions.Any(item => notificationActions.Count(x => x == item) > 1);

    if (hasDuplicationIds)
    {
      _logger.LogWarning("The specified action list has duplicate Ids");
    }

    return notificationActions
      .Select(notificationAction => new
      {
        action = notificationAction.Id,
        title = notificationAction.Title
      })
      .ToArray<object>();
  }
}
