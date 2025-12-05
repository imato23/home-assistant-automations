using System.Collections.Generic;
using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Util;

/// <summary>
/// TTS service for piper speach engine
/// </summary>
public interface INotificationService
{
  /// <summary>
  /// Speaks a text on an media player
  /// </summary>
  /// <param name="mediaPlayerEntityId">The media player entity ID</param>
  /// <param name="text">The text to speak</param>
  void Speak(string mediaPlayerEntityId, string text);

  /// <summary>
  /// Speaks a text on an media player
  /// </summary>
  /// <param name="mediaPlayerEntity">The media player entity</param>
  /// <param name="text">The text to speak</param>
  void Speak(MediaPlayerEntity mediaPlayerEntity, string text);

  /// <summary>
  /// Speaks a text on all media players
  /// </summary>
  /// <param name="text">The text to speak</param>
  void SpeakOnAllMediaPlayers(string text);

  /// <summary>
  /// Speaks a text on a smart phone
  /// </summary>
  /// <param name="smartphone">The smart phone</param>
  /// <param name="text">The texxt</param>
  /// <param name="useAlarmStream">If true, uses the alarm stream; else false</param>
  void SpeakOnSmartphone(Smartphone smartphone, string text, bool useAlarmStream = false);

  /// <summary>
  /// Sends a notification message to a smart phone
  /// </summary>
  /// <param name="smartphone">The smart phone</param>
  /// <param name="title">The message title</param>
  /// <param name="message">The message text</param>
  /// <param name="tag">An optional string, if set previous messages with the same tag will be removed from the smart phone</param>
  /// <param name="timeoutInSeconds">If set, the message will be removed after the timout timespan</param>
  /// <param name="notificationActions">The notification actions</param>
  void NotifySmartphone(
    Smartphone smartphone,
    string title,
    string message,
    string? tag = null,
    uint timeoutInSeconds = 0,
    IEnumerable<NotificationAction>? notificationActions = null);
}
