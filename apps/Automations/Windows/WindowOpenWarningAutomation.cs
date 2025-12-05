using HomeAssistantAutomations.apps.Util;
using NetDaemon.HassModel.Entities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAssistantAutomations.apps.Automations.Windows
{
    public abstract class WindowOpenWarningAutomation<T> : Automation<T>, IAsyncInitializable where T : class
    {
        private INotificationService _notificationService;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly WindowMetadata windowData;
        private Stopwatch openingTime = new Stopwatch();

        protected WindowOpenWarningAutomation(
            IHaContext haContext,
            ILogger<T> logger,
            INotificationService notificationService) : base(haContext, logger)
        {
            _notificationService = notificationService;
            windowData = Initialize();
        }

        protected abstract WindowMetadata Initialize();

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            SubscribeToNotifyResponse();
            await Start();
        }

        private async Task Start()
        {
            windowData.WindowEntity
                .StateChanges()
                .SubscribeAsync(async stateChange =>
                {
                    if (stateChange.New.IsOn())
                    {
                        openingTime.Reset();
                        openingTime.Start();
                        Logger.LogInformation("{window} has been opened", windowData.WindowName);

                        _cancellationTokenSource = new CancellationTokenSource();
                        _ = StartTimerAndNotify(
                            windowData.DefaultWarningInterval,
                            windowData.DefaultWarningInterval,
                            _cancellationTokenSource.Token)
                        .ContinueWith(x => Logger.LogDebug("Notify for {window} has been cancelled", windowData.WindowName));
                    }
                    else
                    {
                        openingTime.Reset();
                        Logger.LogInformation("{window} has been closed", windowData.WindowName);
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

            while (windowData.WindowEntity.EntityState.IsOn())
            {
                TimeSpan interval = isFirstIntervall ? firstInterval : followingInterval;

                Logger.LogInformation("Starting open-warning timer for {window} with {warningIntervall} min", windowData.WindowName, interval.TotalMinutes);
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

            Logger.LogInformation("Sending open-warning message for {window}", windowData.WindowName);

            string tag = $"window_open_warning_{windowData.WindowName}";
            string article = windowData.WindowName.ToLowerInvariant().EndsWith("fenster") ? "Das" : "Die";
            string title = $"{article} {windowData.WindowName} ist noch geöffnet";
            string message = $"{article} {windowData.WindowName} ist bereits seit {Math.Round(openingTime.Elapsed.TotalMinutes)} min geöffnet";

            List<Util.NotificationAction> actions =
            [
              new(
                $"{windowData.WindowName}|{NotificationAction.Mute}",
                "Stummschalten"),
              new(
                $"{windowData.WindowName}|{NotificationAction.SnoozeShort}",
                $"{windowData.ShortWarningInterval.TotalMinutes} min Pausieren"),
              new(
                $"{windowData.WindowName}|{NotificationAction.SnoozeLong}",
                $"{windowData.LongWarningInterval.TotalMinutes} min Pausieren")
            ];

            _notificationService.NotifySmartphone(Smartphone.All, title, message, tag, 0, actions);

            _notificationService.SpeakOnAllMediaPlayers(
                $"Du hast ausreichend gelüftet, bitte schließe {article} {windowData.WindowName}");
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

                    string[] split = action.Split("|");
                    string windowName = split[0];
                    string actionName = split[1];

                    if (windowName != windowData.WindowName)
                    {
                        return;
                    }

                    Logger.LogDebug("Received smartphone notification response event {event} for {window}", action, windowData.WindowName);

                    switch (actionName)
                    {
                        case NotificationAction.Mute:
                            _cancellationTokenSource.Cancel();
                            break;

                        case NotificationAction.SnoozeShort:
                            _cancellationTokenSource.Cancel();
                            _cancellationTokenSource = new CancellationTokenSource();
                            _ = StartTimerAndNotify(
                                windowData.ShortWarningInterval,
                                windowData.DefaultWarningInterval,
                                _cancellationTokenSource.Token)
                                    .ContinueWith(x => Logger.LogDebug("Notify for {window} has been cancelled", windowData.WindowName)); ;
                            break;

                        case NotificationAction.SnoozeLong:
                            _cancellationTokenSource.Cancel();
                            _cancellationTokenSource = new CancellationTokenSource();
                            _ = StartTimerAndNotify(
                                windowData.LongWarningInterval,
                                windowData.DefaultWarningInterval,
                                _cancellationTokenSource.Token);
                            break;

                        default:
                            Logger.LogWarning("Received unknown action {action} for {window}", action, windowData.WindowName);
                            break;
                    }
                });

            return subscription;
        }
    }
}
