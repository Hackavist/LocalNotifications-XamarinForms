using System;
using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using LocalNotificationsXamarinForms.Droid;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(NotifierImplementation))]
namespace LocalNotificationsXamarinForms.Droid
{
    public class NotifierImplementation : INotifierService
    {
        private readonly string channelId = $"{AndroidApp.Context.PackageName}.general"; 
        private bool channelInitialized;
        private NotificationManager NotificationManager => (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);
        public NotifierImplementation()
        {
            Initialize();
        }

        public event EventHandler NotificationReceived;

        void CreateNotificationChannel()
        {

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string channelName = "DefaultChannel";
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
                {
                    Description = "The default channel for notifications."
                };
                NotificationManager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }

        public void Cancel(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 0, intent, PendingIntentFlags.CancelCurrent);

            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);

            var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(AndroidApp.Context);
            notificationManager.Cancel(id);
        }

        public void Initialize()
        {
            if (!channelInitialized) CreateNotificationChannel();
        }

        public void Notify(string title, string body, int id = 0)
        {
            if (!channelInitialized) Initialize();

            var resultIntent = GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(AndroidApp.Context);
            stackBuilder.AddNextIntent(resultIntent);
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentIntent(resultPendingIntent)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.melon_notification_icon))
                .SetSmallIcon(Resource.Drawable.melon_notification_icon)
                .SetDefaults((int)NotificationDefaults.All);

            var notification = builder.Build();
            NotificationManager.Notify(id, notification);
        }

        public void Notify(string title, string body, DateTime notificationDateTime, int id = 0)
        {
            var intent = CreateIntent(id);

            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Id = id;
            localNotification.NotifyTime = notificationDateTime;
            localNotification.IconId = Resource.Drawable.melon_notification_icon;

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            var pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, 0, intent, PendingIntentFlags.CancelCurrent);
            var triggerTime = NotifyTimeInMilliseconds(notificationDateTime);
            var alarmManager = GetAlarmManager();

            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message,
            };
            NotificationReceived?.Invoke(null, args);
        }
        private static Intent GetLauncherActivity()
        {
            return AndroidApp.Context.PackageManager.GetLaunchIntentForPackage(AndroidApp.Context.PackageName);
        }

        private Intent CreateIntent(int id)
        {
            return new Intent(AndroidApp.Context, typeof(ScheduledAlarmHandler)).SetAction("LocalNotifierIntent" + id);
        }
        private AlarmManager GetAlarmManager()
        {
            return AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
        }
        private string SerializeNotification(LocalNotification notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, notification);
            return stringWriter.ToString();
        }
        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }
    }
}
