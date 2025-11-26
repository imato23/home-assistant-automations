using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Automations.Lights
{
    [NetDaemonApp]
    public class LivingRoomMainLightDimmerAutomation
    {
        private const int BrightnessStepInPercent = 10;
        private readonly int[] _colorTemperaturesInKelvin = [2202, 3649, 5102, 6535];
        private readonly ColorTemperatureSwitcher _colorTemperatureSwitcher;
        private readonly Entities _entities;
        private readonly ILogger<LivingRoomMainLightDimmerAutomation> _logger;

        public LivingRoomMainLightDimmerAutomation(
            IHaContext haContext,
            ILogger<LivingRoomMainLightDimmerAutomation> logger)
        {
            _entities = new Entities(haContext);
            _logger = logger;

            _colorTemperatureSwitcher = new ColorTemperatureSwitcher(_colorTemperaturesInKelvin);

            Start();
        }

        private void Start()
        {
            _entities.Sensor.WohnzimmerDimmerDeckenleuchteAction.StateChanges()
                .Subscribe(stateChange =>
                {
                    if (stateChange.New == null || string.IsNullOrEmpty(stateChange.New.State))
                    {
                        return;
                    }

                    HandleStateChange(stateChange.New.State);
                });
        }

        private void HandleStateChange(string action)
        {
            _logger.LogDebug("Dimmer action {DimmerAction} received for light {light}",
                action,
                "Wohnzimmer Deckenleuchte");

            LightEntity light = _entities.Light.WohnzimmerLeuchteDeckenleuchte;

            switch (action)
            {
                case DimmerAction.OnPress:
                    // Toggles light
                    light.Toggle();
                    break;
                case DimmerAction.UpPress:
                    // Increases brightness by 10 percent
                    light.TurnOn(brightnessStepPct: BrightnessStepInPercent);
                    break;
                case DimmerAction.DownPress:
                    // Decreases brightness by 10 percent
                    light.TurnOn(brightnessStepPct: -BrightnessStepInPercent);
                    break;
                case DimmerAction.OffPress:
                    // Changes color temperature with every action
                    light.TurnOn(colorTempKelvin: _colorTemperatureSwitcher.GetNextColorTemperature());
                    break;
            }
        }
    }
}
