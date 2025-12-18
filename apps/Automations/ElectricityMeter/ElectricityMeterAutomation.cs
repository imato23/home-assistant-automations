namespace HomeAssistantAutomations.apps.Automations.ElectricityMeter
{
  [Focus]
  [NetDaemonApp]
  public class ElectricityMeterAutomation : Automation<ElectricityMeterAutomation>
  {
    private const double OffsetInKWh = 68907;

    private double? electricityMeterValueCorrectedInKWh;

    private double previousValueInKWh;

    public ElectricityMeterAutomation(
      IHaContext haContext,
      ILogger<ElectricityMeterAutomation> logger) : base(haContext, logger)
    {
      AddOffset();

      // On startup, include all changes since last sensor reading
      Process(previousValueInKWh, Entities.Sensor.WerkstattStromzaehlerEnergie.State);

      Entities.Sensor.WerkstattStromzaehlerEnergie
      .StateChanges()
      .Subscribe(stateChange => Process(stateChange?.Old?.State / 1000, stateChange?.New?.State / 1000));
    }

    private void Process(double? previousSensorValueInKWh, double? currentSensorValueInKWh)
    {
      if (previousSensorValueInKWh == null)
      {
        previousSensorValueInKWh = previousValueInKWh;
      }

      if (currentSensorValueInKWh == null)
      {
        Logger.LogDebug("Current sensor value is null");
        return;
      }

      CorrectOverflow(previousSensorValueInKWh.Value, currentSensorValueInKWh.Value);

      previousValueInKWh = previousSensorValueInKWh.Value;
    }

    private void AddOffset()
    {
      if (electricityMeterValueCorrectedInKWh == null)
      {
        Logger.LogDebug("Corrected value is null, adding offset ({offset} KWh)", OffsetInKWh);
        electricityMeterValueCorrectedInKWh = OffsetInKWh;
      }
    }

    private void CorrectOverflow(double previousValueInKWh, double currentValueInKWh)
    {
      const double maxValueInKWh = 838860.7;
      double? previousElectricityMeterValueCorrectedInKWh = electricityMeterValueCorrectedInKWh;

      if (previousValueInKWh > currentValueInKWh)
      {
        // Overflow occurred
        Logger.LogDebug(
          "Detected overflow, previous value is {previousValue} KWh, current value is {currentValue} KWh",
          previousValueInKWh,
          currentValueInKWh);

        electricityMeterValueCorrectedInKWh += maxValueInKWh - previousValueInKWh + currentValueInKWh;
      }
      else
      {
        electricityMeterValueCorrectedInKWh += currentValueInKWh - previousValueInKWh;
      }

      Logger.LogDebug(
        "New corrected value is {correctedValue} KWh, previous corrected value was {previousCorrectedValue} KWh" +
        "(previous sensor value: {previousValue} KWh, current sensor value: {currentValue} KWh)",
        electricityMeterValueCorrectedInKWh,
        previousElectricityMeterValueCorrectedInKWh,
        previousValueInKWh,
        currentValueInKWh);
    }
  }
}
