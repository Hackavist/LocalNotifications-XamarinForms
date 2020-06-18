﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;

using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace LocalNotificationsXamarinForms.iOS
{
    public class IosNotificationReceiver : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            DependencyService.Get<INotifierService>().ReceiveNotification(notification.Request.Content.Title, notification.Request.Content.Body);

            // alerts are always shown for demonstration but this can be set to "None"
            // to avoid showing alerts if the app is in the foreground
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}