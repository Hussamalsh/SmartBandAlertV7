using Backendt1.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace Backendt1.Models
{

    public class TaskDMtimer
    {
        public RegisteredWaitHandle handle = null;
        public string OtherInfo = "DEFAULT";
    }
    public class DangerMode
    {

        public bool isDangerModeOn
        {
            get;
            set;
        }

        public String FBID
        {
            get;
            set;
        }

        public bool islive
        {
            get;
            set;
        }

        public string UserName
        {
            set;
            get;
        }
        public String Latitude { get; set; }

        public String Longitude { get; set; }

        public TaskDMtimer ti;
        public void isAppLive()
        {
            var waitHandle = new AutoResetEvent(false);

            ti = new TaskDMtimer();
            ti.OtherInfo = "Userid = " +FBID;
            ti.handle = ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                // Method to execute
                (state, timeout) =>
                {
                    // TODO: implement the functionality you want to be executed
                    // on every 5 seconds here
                    // Important Remark: This method runs on a worker thread drawn 
                    // from the thread pool which is also used to service requests
                    // so make sure that this method returns as fast as possible or
                    // you will be jeopardizing worker threads which could be catastrophic 
                    // in a web application. Make sure you don't sleep here and if you were
                    // to perform some I/O intensive operation make sure you use asynchronous
                    // API and IO completion ports for increased scalability


                    if (!islive && isDangerModeOn)
                    {
                        sendpushNotificationAsync();
                        stopTimer();
                    }

                    islive = false;

                },
                // optional state object to pass to the method
                null,
                // Execute the method after 5 seconds
                TimeSpan.FromSeconds(120),
                // Set this to false to execute it repeatedly every 5 seconds
                false
            );


        }

        public void stopTimer()
        {
            ti.handle.Unregister(null);
        }


        public async System.Threading.Tasks.Task sendpushNotificationAsync()
        {
            var user = FBID;

            string to_tag  = FBID +"T";

            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;


            /////////////////////////////////////////////////////////here for every friend//App.FacebookId+"T"
            FriendsPersistence fp = new FriendsPersistence();
            ArrayList friends = fp.getFriends(FBID);
            string pns = "gcm";

            bool firsttime = true;

            foreach (Friends f in friends)
            {
                if (f.Status == 1)
                {
                    switch (pns.ToLower())
                    {
                        case "wns":
                            // Windows 8.1 / Windows Phone 8.1
                            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                        "From " + user + ": " + "wns=message" + "</text></binding></visual></toast>";
                            outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, to_tag);
                            break;
                        case "apns":
                            // iOS
                            var alert = "{\"aps\":{\"alert\":\"" + "From " + UserName + " Need Help from you. The User ID =[" + FBID
                                            + "] [" + Latitude + "] [" + Longitude + "]\"}}";
                            outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, f.FriendFBID + "T");
                            if (firsttime)
                            {
                                var alert1 = "{\"aps\":{\"alert\":\"" + "Your alarm was successfully sent to all your friends ID =[" + FBID
                                + "] [" + Latitude + "] [" + Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert1, FBID + "T");
                                firsttime = false;
                            }
                            break;
                        case "gcm":
                            // Android
                            //{"data":{"message":"From Hussi Need Help from you. The User ID =[132569873917640] [56.6642811] [12.8778527]"}}

                            if (f.FriendFBID.Equals(FBID))
                            {
                                var notif = "{ \"data\" : {\"message\":\"" + "From yourfriend" 
                                + " Need Help from you. The User ID =[" + f.UserFBID
                                            + "] [" + Latitude + "] [" +Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, f.UserFBID + "T");
                            }
                            else
                            {
                                var notif = "{ \"data\" : {\"message\":\"" + "From " + UserName
                                + " Need Help from you. The User ID =[" + FBID
                                            + "] [" + Latitude + "] [" + Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, f.FriendFBID + "T");
                            }

                            if (firsttime)
                            {
                                System.Threading.Thread.Sleep(500);
                                var notif = "{ \"data\" : {\"message\":\"" + "Your alarm was successfully sent to all your friends ID =[" + FBID
                                            + "] [" + Latitude + "] [" + Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, FBID + "T");
                                firsttime = false;
                            }


                            break;
                    }

                    if (outcome != null)
                    {
                        if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                            (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                        {
                            ret = HttpStatusCode.OK;
                        }
                    }
                }

            }
        }





    }
}