using System.Net.Http;
using System.Net.Http.Json;
using HomeAssistantGenerated;
using NetDaemon.HassModel.Entities;

namespace HomeAssistantAutomations.apps.Automations.LocationTracker
{
  [NetDaemonApp]
  public partial class LocationTrackerAutomation : Automation<LocationTrackerAutomation>
  {
    private readonly HttpClient _httpClient;
    private readonly DeviceTrackerEntity _deviceTrackerEntity;
    private readonly InputNumberEntity _etaMinutesSensor;
    private readonly InputDatetimeEntity _etaTimeSensor;
    private readonly InputNumberEntity _etaDistanceSensor;

    public LocationTrackerAutomation(
      IHaContext haContext,
      ILogger<LocationTrackerAutomation> logger,
      IHttpClientFactory httpFactory)
    : base(haContext, logger)
    {
      _httpClient = httpFactory.CreateClient();
      _httpClient.BaseAddress = new Uri("http://tripoli:8989/route");

      _deviceTrackerEntity = Entities.DeviceTracker.SmartphoneSabine;
      _etaMinutesSensor = Entities.InputNumber.GraphhopperEtaMinutenSabine;
      _etaTimeSensor = Entities.InputDatetime.GraphhopperEtaUhrzeitSabine;
      _etaDistanceSensor = Entities.InputNumber.GraphhopperEntfernungSabine;

      Calculate(_deviceTrackerEntity.EntityState);

      _deviceTrackerEntity.StateChanges()
        .Subscribe(stateChange => Calculate(stateChange.New));
    }

    private async void Calculate(EntityState? state)
    {
      if (state?.Attributes == null)
      {
        return;
      }

      Logger.LogInformation("Geolocation has been changed for {Entity}", _deviceTrackerEntity.EntityId);

      GeoPoint currentPosition = new(
        Convert.ToDouble(state.Attributes["latitude"].ToString()),
        Convert.ToDouble(state.Attributes["longitude"].ToString()));

      double? homeZoneLatitude = Entities?.Zone?.Home?.Attributes?.Latitude;
      double? homeZoneLongitude = Entities?.Zone?.Home?.Attributes?.Longitude;

      if (homeZoneLatitude == null || homeZoneLongitude == null)
      {
        throw new InvalidOperationException("Latitude or Longitude of home zone is null");
      }

      GeoPoint homePosition = new(homeZoneLatitude.Value, homeZoneLongitude.Value);

      string url =
              $"?point={currentPosition.Latitude},{currentPosition.Longitude}" +
              $"&point={homePosition.Latitude},{homePosition.Longitude}" +
              $"&profile=car&locale=de&points_encoded=false";
      GraphHopperResponse? response = await _httpClient.GetFromJsonAsync<GraphHopperResponse>(url);

      if ((response?.Paths) == null || response.Paths.Length == 0)
      {
        Logger.LogError("Couldn't read response from GrassHopper API");
        return;
      }

      double distanceInKm = response.Paths[0].Distance / 1000;
      double etaInMinutes = response.Paths[0].Time / 1000 / 60;

      double etaMinutes = Math.Round(etaInMinutes);
      DateTime etaTime = DateTime.Now.AddMinutes(etaInMinutes);

      _etaDistanceSensor.SetValue(distanceInKm);
      _etaMinutesSensor.SetValue(etaMinutes);
      _etaTimeSensor.SetDatetime(time: new TimeOnly(etaTime.Hour, etaTime.Minute));
    }
  }
}
