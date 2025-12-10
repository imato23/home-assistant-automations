using System.Collections.Generic;

namespace HomeAssistantAutomations.apps.Util
{
  public interface IDeviceTrackerService
  {
    /// <summary>
    /// Determines if Thomas is currently at home
    /// </summary>
    bool IsThomasAtHome { get; }

    /// <summary>
    /// Determines if Sabine is currently at home
    /// </summary>
    bool IsSabineAtHome { get; }

    /// <summary>
    /// Determines if anyone is currently at home
    /// </summary>
    bool IsAnyoneAtHome { get; }

    /// <summary>
    /// Determines if all persons currently at home
    /// </summary>
    bool AreAllAtHome { get; }

    /// <summary>
    /// Gets the persons currently at home
    /// </summary>
    /// <param name="returnEmptyWhenNoneHome">If true, returns an empty list if no one is at home;
    /// otherwise returns all persons</param>
    /// <returns>The persons</returns>
    IEnumerable<Smartphone> GetPersonsAtHome(bool returnEmptyWhenNoneHome);
  }
}
