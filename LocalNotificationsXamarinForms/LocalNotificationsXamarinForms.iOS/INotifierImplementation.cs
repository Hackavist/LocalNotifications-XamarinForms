using System;
using System.Diagnostics;
using Foundation;
using LocalNotificationsXamarinForms.iOS;
using UserNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotifierImplementation))]
namespace LocalNotificationsXamarinForms.iOS
{
    public class NotifierImplementation : INotifierService
    {
        private bool _hasNotificationPermissions;

        public NotifierImplementation()
        {
            Initialize();
        }

        public void Initialize()
        {
            // Ask the user for permission to get notifications on iOS 10.0+
            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, error) => { _hasNotificationPermissions = approved; });
        }

        public event EventHandler NotificationReceived;

        /// <summary>
        ///     Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new[] {CreateRequestIdForm(id)});
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new[] {CreateRequestIdForm(id)});
        }


        /// <summary>
        ///     Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        public void Notify(string title, string body, int id = 0)
        {
            if (!_hasNotificationPermissions) return;
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = body
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(.1, false);

            var request = UNNotificationRequest.FromIdentifier(CreateRequestIdForm(id), content, trigger);

            BaseNotify(id, content, trigger);
        }

        /// <summary>
        ///     Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notificationDateTime">Time to show notification</param>
        public void Notify(string title, string body, DateTime notificationDateTime, int id = 0)
        {
            if (!_hasNotificationPermissions) return;
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = body,
                Sound = UNNotificationSound.Default
            };
            var dateComponent = new NSDateComponents
            {
                Month = notificationDateTime.Month,
                Day = notificationDateTime.Day,
                Year = notificationDateTime.Year,
                Hour = notificationDateTime.Hour,
                Minute = notificationDateTime.Minute,
                Second = notificationDateTime.Second
            };
            var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponent, false);
            BaseNotify(id, content, trigger);
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message
            };
            NotificationReceived?.Invoke(null, args);
        }

        /// <summary>
        ///     Code base logic of showing a notification
        /// </summary>
        /// <param name="content">the content of the notification</param>
        /// <param name="trigger">the conditions that trigger the notification</param>
        private void BaseNotify(int id, UNMutableNotificationContent content, UNNotificationTrigger trigger)
        {
            var request = UNNotificationRequest.FromIdentifier(CreateRequestIdForm(id), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, err =>
            {
                if (err != null)
                {
                    Debug.WriteLine("Adding Notification Failed " + err.DebugDescription);
                    Debug.WriteLine("Adding Notification Failed " + err.LocalizedFailureReason);
                }
            });
            var requests = UNUserNotificationCenter.Current.GetPendingNotificationRequestsAsync().Result;
        }

        /// <summary>
        ///     returns the correct notification request id form
        /// </summary>
        private string CreateRequestIdForm(int id)
        {
            return $"Notification_{id}";
        }
    }
}