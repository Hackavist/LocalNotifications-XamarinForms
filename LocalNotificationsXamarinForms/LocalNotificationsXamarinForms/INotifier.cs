using System;

using Xamarin.Essentials;

namespace LocalNotificationsXamarinForms
{
    public interface INotifierService
    {
        /// <summary>
        /// The Executed Behaviour when the app receives a notification  
        /// </summary>
        event EventHandler NotificationReceived;

        /// <summary>
        /// Executes the platform Specific service initiation code 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Show a local notification now
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="id">Id of the notification</param>
        void FireANotification(string title, string body, int id = 0);

        /// <summary>
        /// Show a local notification at a certain date and time 
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="notificationDateTime"> The date and time the notification should be fired </param>
        /// <param name="id">Id of the notification</param>
        void FireANotification(string title, string body, DateTime notificationDateTime, int id = 0);

        /// <summary>
        /// Show a local notification at a certain location 
        /// </summary>
        /// <param name="title">Title of the notification</param>
        /// <param name="body">Body or description of the notification</param>
        /// <param name="notificationGeolocation"> The Geolocation where the notification should be fired </param>
        /// <param name="id">Id of the notification</param>
        void FireANotification(string title, string body, Location notificationGeolocation, int id = 0);

        /// <summary>
        /// Cancel a local notification
        /// </summary>
        /// <param name="id">Id of the notification to cancel</param>
        void Cancel(int id);


        void ReceiveNotification(string title, string message);

    }
}
