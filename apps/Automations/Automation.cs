using HomeAssistantGenerated;

namespace HomeAssistantAutomations.apps.Automations
{
    public abstract class Automation<T> where T : class
    {
        protected IEntities Entities { get; }
        protected IHaContext HaContext { get; }
        protected ILogger<T> Logger { get; }
        protected IServices Services { get; }

        protected Automation(IHaContext haContext, ILogger<T> logger)
        {
            HaContext = haContext;
            Logger = logger;
            Entities = new Entities(haContext);
            Services = new Services(haContext);
        }
    }
}
