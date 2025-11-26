using HomeAssistantAutomations.apps.Util;
using HomeAssistantGenerated;
using NetDaemon.HassModel.Entities;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
    [NetDaemonApp]
    public class WindowOpenWarningAutomation : Automation<WindowOpenWarningAutomation>, IAsyncInitializable
    {
        private IPiperTtsService _piperTtsService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly BinarySensorEntity _windowEntity;
        private readonly TimeSpan _defaultWarningInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _shortWarningInterval = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _longWarningInterval = TimeSpan.FromMinutes(3);
        private readonly string windowName = "Terassentür";

        private Stopwatch openingTime = new Stopwatch();

        public WindowOpenWarningAutomation(
            IHaContext haContext,
            ILogger<WindowOpenWarningAutomation> logger,
            IPiperTtsService piperTtsService) : base(haContext, logger)
        {
            _windowEntity = Entities.BinarySensor.WohnzimmerTerrassentuerStatus;
            _piperTtsService = piperTtsService;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            SubscribeToNotifyResponse();
            await Start();
        }

        private async Task Start()
        {
            _windowEntity
                .StateChanges()
                .SubscribeAsync(async stateChange =>
                {
                    if (stateChange.New.IsOn())
                    {
                        openingTime.Reset();
                        openingTime.Start();
                        Logger.LogInformation("{window} has been opened", windowName);

                        _cancellationTokenSource = new CancellationTokenSource();
                        _ = StartTimerAndNotify(
                            _defaultWarningInterval,
                            _defaultWarningInterval,
                            _cancellationTokenSource.Token)
                        .ContinueWith(x => Logger.LogDebug("Notify for {window} has been cancelled", windowName));
                    }
                    else
                    {
                        openingTime.Reset();
                        Logger.LogInformation("{window} has been closed", windowName);
                        if (_cancellationTokenSource != null)
                        {
                            _cancellationTokenSource.Cancel();
                        }
                    }
                });
        }

        private async Task StartTimerAndNotify(TimeSpan firstInterval, TimeSpan followingInterval, CancellationToken cancellationToken)
        {
            bool isFirstIntervall = true;

            while (_windowEntity.EntityState.IsOn())
            {
                TimeSpan interval = isFirstIntervall ? firstInterval : followingInterval;

                Logger.LogInformation("Starting open-warning timer for {window} with {warningIntervall} min", windowName, interval.Minutes);
                await Task.Delay(interval, cancellationToken);
                await Notify(cancellationToken);
                isFirstIntervall = false;
            }
        }

        private async Task Notify(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Logger.LogInformation("Sending open-warning message for {window}", windowName);

            var data = new
            {
                actions = new[]
                {
                    new
                    {
                        action = NotificationAction.Mute,
                        title = "Stummschalten",
                    },
                    new
                    {
                        action = NotificationAction.SnoozeShort,
                        title = $"{_shortWarningInterval.TotalMinutes} min Pausieren"
                    },
                    new
                    {
                        action = NotificationAction.SnoozeLong,
                        title = $"{_longWarningInterval.TotalMinutes} min Pausieren"
                    }
                }
            };

            string article = windowName.ToLowerInvariant().EndsWith("fenster") ? "Das" : "Die";
            string title = $"{article} {windowName} ist noch geöffnet";
            string message = $"{article} {windowName} ist bereits seit {openingTime.Elapsed.Minutes} min geöffnet";

            Services.Notify.MobileAppSmartphoneThomas(
                message: message,
                title: title,
                data: data);

            // _piperTtsService.Speak(
            //     Entities.MediaPlayer.KucheLautsprecher,
            //     $"Du hast ausreichend gelüftet, bitte schließe {article} {windowName}");
        }

        private IDisposable SubscribeToNotifyResponse()
        {
            IDisposable subscription = HaContext.Events.Where(e => e.EventType == "mobile_app_notification_action")
                .Subscribe(e =>
                {
                    string? action = e.DataElement?.GetProperty("action").GetString();

                    if (action == null)
                    {
                        return;
                    }

                    Logger.LogDebug("Received smartphone notification response event {event} for {window}", action, windowName);

                    switch (action)
                    {
                        case NotificationAction.Mute:
                            _cancellationTokenSource.Cancel();
                            break;

                        case NotificationAction.SnoozeShort:
                            _cancellationTokenSource.Cancel();
                            _cancellationTokenSource = new CancellationTokenSource();
                            _ = StartTimerAndNotify(
                                _shortWarningInterval,
                                _defaultWarningInterval,
                                _cancellationTokenSource.Token)
                                    .ContinueWith(x => Logger.LogDebug("Notify for {window} has been cancelled", windowName)); ;
                            break;

                        case NotificationAction.SnoozeLong:
                            _cancellationTokenSource.Cancel();
                            _cancellationTokenSource = new CancellationTokenSource();
                            _ = StartTimerAndNotify(
                                _longWarningInterval,
                                _defaultWarningInterval,
                                _cancellationTokenSource.Token);
                            break;

                        default:
                            Logger.LogWarning("Received unknown action {action} for {window}", action, windowName);
                            break;
                    }
                });

            return subscription;
        }
    }
}
