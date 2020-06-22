using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Xamarin.Forms;

namespace LocalNotificationsXamarinForms.Droid
{
    /// <summary>
    /// Broadcast receiver
    /// </summary>
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {
        /// <summary>
        /// 
        /// </summary>
        public const string LocalNotificationKey = "LocalNotification";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);
            DependencyService.Get<INotifierService>().Notify(notification.Title, notification.Body, 12);
        }

        private LocalNotification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
            using var stringReader = new StringReader(notificationString);
            var notification = (LocalNotification)xmlSerializer.Deserialize(stringReader);
            return notification;
        }
    }
}