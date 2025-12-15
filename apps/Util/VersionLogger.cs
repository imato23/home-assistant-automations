public class VersionLogger : IVersionLogger
{
  private readonly ILogger<VersionLogger> _logger;

  public VersionLogger(ILogger<VersionLogger> logger)
  {
    _logger = logger;
  }

  public void LogVersion()
  {
    Version? version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    _logger.LogInformation("Application version is: {version}", version);
  }
}
