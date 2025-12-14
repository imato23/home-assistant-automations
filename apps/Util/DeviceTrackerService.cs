using System.Collections;
using System.Collections.Generic;
using HomeAssistantAutomations.apps.Util;
using HomeAssistantGenerated;

internal class DeviceTrackerService : IDeviceTrackerService
{
  private readonly IEntities _entities;

  public DeviceTrackerService(IHaContext haContext)
  {
    _entities = new Entities(haContext);
  }

  public bool IsThomasAtHome
  {
    get { return _entities.DeviceTracker.SmartphoneThomas.State == "home"; }
  }

  public bool IsSabineAtHome
  {
    get { return _entities.DeviceTracker.SmartphoneSabine.State == "home"; }
  }

  public bool IsAnyoneAtHome
  {
    get { return IsThomasAtHome || IsSabineAtHome; }
  }

  public bool AreAllAtHome
  {
    get { return IsThomasAtHome && IsSabineAtHome; }
  }

  public IEnumerable<Smartphone> GetPersonsAtHome(bool returnEmptyWhenNoneHome)
  {
    if (!IsAnyoneAtHome && !returnEmptyWhenNoneHome)
    {
      yield return Smartphone.Thomas;
      yield return Smartphone.Sabine;
    }

    if (IsThomasAtHome)
    {
      yield return Smartphone.Thomas;
    }

    if (IsSabineAtHome)
    {
      yield return Smartphone.Sabine;
    }
  }
}
