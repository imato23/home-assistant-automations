using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Util;

/// <summary>
/// TTS service for piper speach engine
/// </summary>
public interface IPiperTtsService
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
}