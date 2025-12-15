[NetDaemonApp]
public class VersionLoggerAutomation
{
  private readonly IVersionLogger _versionLogger;

  public VersionLoggerAutomation(
      IVersionLogger versionLogger)
  {
    _versionLogger = versionLogger;
    _versionLogger.LogVersion();
  }
}
