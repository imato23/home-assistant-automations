using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Util;

/// <summary>
/// TTS service for piper speech engine
/// </summary>
public class PiperTtsService : IPiperTtsService
{
  private readonly IHaContext _haContext;
  private const string Domain = "tts";
  private const string Service = "speak";
  private const string ServiceTarget = "tts.piper";

  /// <summary>
  /// Creates a new instance of the <see cref="PiperTtsService"/>
  /// </summary>
  /// <param name="haContext">The HomeAssistant context</param>
  public PiperTtsService(IHaContext haContext)
  {
    _haContext = haContext;
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
}