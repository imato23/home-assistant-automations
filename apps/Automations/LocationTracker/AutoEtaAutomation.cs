using System.Net.Http;
using System.Net.Http.Json;
using HomeAssistantAutomations.apps.Automations;
using HomeAssistantAutomations.apps.Automations.LocationTracker;
using HomeAssistantGenerated;
using NetDaemon.HassModel.Entities;

[Focus]
[NetDaemonApp]
public partial class AutoEtaApp : Automation<AutoEtaApp>
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<AutoEtaApp> _logger;

  private readonly DeviceTrackerEntity _deviceTrackerEntity;
  private readonly InputNumberEntity _etaMinSensor;
  private readonly InputDatetimeEntity _etaTimeSensor;
  private readonly InputNumberEntity _etaDistanceSensor;

  public AutoEtaApp(IHaContext haContext, ILogger<AutoEtaApp> logger, IHttpClientFactory httpFactory)
  : base(haContext, logger)
  {
    _httpClient = httpFactory.CreateClient();
    _httpClient.BaseAddress = new Uri("http://tripoli:8989/route");
    _logger = logger;

    _deviceTrackerEntity = Entities.DeviceTracker.SmartphoneSabine;
    _etaMinSensor = Entities.InputNumber.GraphhopperEtaMinutenSabine;
    _etaTimeSensor = Entities.InputDatetime.GraphhopperEtaUhrzeitSabine;
    _etaDistanceSensor = Entities.InputNumber.GraphhopperEntfernungSabine;

    ProcessEta(_deviceTrackerEntity.EntityState);

    _deviceTrackerEntity.StateChanges()
      .Subscribe(stateChange => ProcessEta(stateChange.New));
  }

  private async void ProcessEta(EntityState? state)
  {
    if (state?.Attributes == null)
    {
      return;
    }

    Logger.LogInformation("Geolocation has been changed for {Entity}", _deviceTrackerEntity.EntityId);

    GeoPoint currentPosition = new(
      Convert.ToDouble(state.Attributes["latitude"].ToString()),
      Convert.ToDouble(state.Attributes["longitude"].ToString()));


    if (Entities?.Zone?.Home?.Attributes?.Latitude == null || Entities?.Zone?.Home?.Attributes?.Longitude == null)
    {
      throw new InvalidOperationException("Latitude or Longitude of home zone is null");
    }

    GeoPoint homePosition = new(
      Entities.Zone.Home.Attributes.Latitude.Value,
      Entities.Zone.Home.Attributes.Longitude.Value
    );

    string url =
            $"?point={currentPosition.Latitude},{currentPosition.Longitude}" +
            $"&point={homePosition.Latitude},{homePosition.Longitude}" +
            $"&profile=car&locale=de&points_encoded=false";
    try
    {
      GraphHopperResponse? response = await _httpClient.GetFromJsonAsync<GraphHopperResponse>(url);

      if ((response?.Paths) == null)
      {
        Logger.LogError("Couldn't read response from GrassHopper API");
        return;
        // State("sensor.auto_eta").SetAttribute("minutes", etaMin);
        // State("sensor.auto_eta").SetAttribute("eta_time",
        //     DateTime.Now.AddMinutes(etaMin).ToString("HH:mm"));
        // State("sensor.auto_eta").State = $"{etaMin:F0} min";
      }

      double distanceInKm = response.Paths[0].Distance / 1000;
      double etaInMinutes = response.Paths[0].Time / 1000 / 60;

      double etaMin = Math.Round(etaInMinutes);
      DateTime etaTime = DateTime.Now.AddMinutes(etaInMinutes);

      _etaDistanceSensor.SetValue(distanceInKm);
      _etaMinSensor.SetValue(etaMin);
      _etaTimeSensor.SetDatetime(time: new TimeOnly(etaTime.Hour, etaTime.Minute));
    }
    catch (Exception ex) { _logger.LogError(ex, "ETA Fehler"); }
  }
  public class Path
  {
    public double Time { get; set; }
    public double Distance { get; set; }
  }
}
