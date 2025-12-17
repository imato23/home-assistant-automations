namespace HomeAssistantAutomations.apps.Automations.LocationTracker
{
  public class GeoPoint
  {
    public GeoPoint(double latitude, double longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }

    public double Latitude { get; }

    public double Longitude { get; }

  }
}
