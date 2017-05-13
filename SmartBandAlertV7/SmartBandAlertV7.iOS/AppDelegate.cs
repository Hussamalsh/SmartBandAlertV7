using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using UserNotifications;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using SmartBandAlertV7.Data;
using SmartBandAlertV7.Messages;
using SmartBandAlertV7.iOS.Services;
using Microsoft.WindowsAzure.MobileServices;
using Acr.UserDialogs;
using ImageCircle.Forms.Plugin.iOS;

namespace SmartBandAlertV7.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            Xamarin.FormsMaps.Init();


            ImageCircleRenderer.Init();


            /*
            // define useragent android like
            string userAgent = "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 5 Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/43.0.2357.65 Mobile Safari/537.36";

            // set default useragent
            NSDictionary dictionary = NSDictionary.FromObjectAndKey(NSObject.FromObject(userAgent), NSObject.FromObject("UserAgent"));
            NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);
            */

            // Register for push notifications.
            var settings = UIUserNotificationSettings.GetSettingsForTypes
                                        (UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();


            // Get Shared User Defaults
            var plist = NSUserDefaults.StandardUserDefaults;
            // Get value
            var useHeader = plist.StringForKey("PrefName");
            if (!String.IsNullOrEmpty(useHeader))
                App.IsLoggedIn = true;


            App.Init((IAuthenticate)this);


            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width * 4;
            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height * 2;
            App.ScreenDPI = 600;



            // check for a notification
            if (options != null)
            {
                // check for a local notification
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localNotification != null)
                    {
                        UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        var window = UIApplication.SharedApplication.KeyWindow;
                        window.RootViewController.PresentViewController(okayAlertController, true, null);

                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });

                // Watch for notifications while app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settingss = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settingss);
            }


            WireUpLongRunningTask();


            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }


        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            var window = UIApplication.SharedApplication.KeyWindow;
            window.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }


        IOSLongRunningTaskExample longRunningTaskExample;

        void WireUpLongRunningTask()
        {
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, "StartLongRunningTaskMessage", async message => {
                longRunningTaskExample = new IOSLongRunningTaskExample();
                await longRunningTaskExample.Start();
            });

            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, "StopLongRunningTaskMessage", message => {
                longRunningTaskExample.Stop();
            });
        }

        public Task<bool> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        public bool LogoutAsync()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            var account = plist.StringForKey("PrefName");
            if (account != null)
            {
                plist.SetString("", "PrefName");
                // Sync changes to database
                plist.Synchronize();
                return true;
            }
            return false;
        }




        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

            JObject templates = new JObject();
            templates["genericMessage"] = new JObject
         {
           {"body", templateBodyAPNS}
         };

            // Register for push with your mobile app
            var push = SBAManager.DefaultManager.CurrentClient.GetPush();
            push.RegisterAsync(deviceToken, templates);

            var plist = NSUserDefaults.StandardUserDefaults;
            // Save value
            plist.SetString(deviceToken.ToString(), "regid");
            // Sync changes to database
            plist.Synchronize();


        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,
                                                                                    Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();

            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
            }
        }

    }
}
