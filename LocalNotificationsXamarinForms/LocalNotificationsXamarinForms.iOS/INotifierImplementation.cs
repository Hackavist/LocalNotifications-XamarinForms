using System;
using System.Diagnostics;
using Foundation;
using UIKit;
using UserNotifications;

namespace LocalNotificationsXamarinForms.iOS
{
    public class INotifierImplementation : INotifierService
    {
        private bool HasNotificationPermissions;
        public INotifierImplementation()
        {
            Initialize();
        }
        public void Initialize()
        {
            // Ask the user for permission to get notifications on iOS 10.0+
            UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { HasNotificationPermissions = approved; });
        }

        public event EventHandler NotificationReceived;

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        public void Cancel(int id)
        {
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new string[] { CreateRequestIdForm(id) });
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new string[] { CreateRequestIdForm(id) });
        }


        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        public void Notify(string title, string body, int id = 0)
        {
            if (!HasNotificationPermissions) return;
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = body,
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(.1, false);

            var request = UNNotificationRequest.FromIdentifier(CreateRequestIdForm(id), content, trigger);

            BaseNotify(id, content, trigger);
        }

        /// <summary>
        /// Show a local notification
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        /// <param name="notificationDateTime">Time to show notification</param>
        public void Notify(string title, string body, DateTime notificationDateTime, int id = 0)
        {
            if (!HasNotificationPermissions) return;
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = body,
                Sound = UNNotificationSound.Default
            };
            var datecomponent = new NSDateComponents
            {
                Month = notificationDateTime.Month,
                Day = notificationDateTime.Day,
                Year = notificationDateTime.Year,
                Hour = notificationDateTime.Hour,
                Minute = notificationDateTime.Minute,
                Second = notificationDateTime.Second
            };
            var trigger = UNCalendarNotificationTrigger.CreateTrigger(datecomponent, false);
            BaseNotify(id, content, trigger);
        }

        /// <summary>
        /// Code base logic of showing a notification
        /// </summary>
        /// <param name="content">the content of the notification</param>
        /// <param name="trigger">the conditions that trigger the notification</param>
        private void BaseNotify(int id, UNMutableNotificationContent content, UNNotificationTrigger trigger)
        {
            var request = UNNotificationRequest.FromIdentifier(CreateRequestIdForm(id), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    Debug.WriteLine("Adding Notitifaction Failed " + err.DebugDescription);
                    Debug.WriteLine("Adding Notitifaction Failed " + err.LocalizedFailureReason);

                }
            });
        }
        /// <summary>
        /// returns the correct notification request id form
        /// </summary>
        private string CreateRequestIdForm(int id)
        {
            return $"Notification_{id}";
        }

        public void ReceiveNotification(string title, string message)
        {
            throw new NotImplementedException();
        }
    }
}
