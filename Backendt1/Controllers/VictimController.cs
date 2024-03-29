﻿using Backendt1.DataObjects;
using Backendt1.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Backendt1.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class VictimController : ApiController
    {
        // GET: api/Victim
        public ArrayList Get()
        {
            VictimPersistence pp = new VictimPersistence();
            return pp.getVictims();
        }

        // GET: api/Victim/5
        [Route("api/Victim/{id}")]
        [HttpGet]
        public Victim Get(String id)
        {
            VictimPersistence pp = new VictimPersistence();
            Victim p = pp.getVictim(id);
            if (p == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return p;
        }

        // POST: api/Victim
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post(string pns, [FromBody]Victim value, string to_tag)
        {

            var user = value.FBID;


            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;


            /////////////////////////////////////////////////////////here for every friend//App.FacebookId+"T"
            FriendsPersistence fp = new FriendsPersistence();
            ArrayList friends = fp.getFriends(value.FBID);

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
                            var alert = "{\"aps\":{\"alert\":\"" + "From " + value.UserName + " Need Help from you. The User ID =[" + value.FBID
                                            + "] [" + value.Latitude + "] [" + value.Longitude + "]\"}}";
                            outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, f.FriendFBID + "T");
                            if (firsttime)
                            {
                                var alert1 = "{\"aps\":{\"alert\":\"" + "Your alarm was successfully sent to all your friends ID =[" + value.FBID
                                + "] [" + value.Latitude + "] [" + value.Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert1, value.FBID + "T");
                                firsttime = false;
                            }
                            break;
                        case "gcm":
                            // Android
                            //{"data":{"message":"From Hussi Need Help from you. The User ID =[132569873917640] [56.6642811] [12.8778527]"}}


                            if (f.FriendFBID.Equals(value.FBID))
                            {
                                var notif = "{ \"data\" : {\"message\":\"" + "From yourfriend"
                                + " Need Help from you. The User ID =[" + value.FBID
                                            + "] [" + value.Latitude + "] [" + value.Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, f.UserFBID + "T");
                            }
                            else
                            {
                                var notif = "{ \"data\" : {\"message\":\"" + "From " + value.UserName
                                + " Need Help from you. The User ID =[" + value.FBID
                                            + "] [" + value.Latitude + "] [" + value.Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, f.FriendFBID + "T");
                            }


                            if (firsttime)
                            {
                                var notif = "{ \"data\" : {\"message\":\"" + "Your alarm was successfully sent to all your friends ID =[" + value.FBID
                                            + "] [" + value.Latitude + "] [" + value.Longitude + "]\"}}";
                                outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, value.FBID + "T");
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
            // return Request.CreateResponse(ret);
            /////////////////////////////////////////////////////////here for every friend-End



            VictimPersistence vp = new VictimPersistence();
            String id;
            id = vp.saveUser(value);
            /*
            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = value.UserName + " Need Help from you. The User ID =" + value.FBID;

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }
            */
            //value.FBID = id;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, String.Format("/user/{0}", id));
            return response;

        }


        // PUT: api/Victim/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Victim/5
        public void Delete(int id)
        {


        }



        // POST: api/Victim
        [Route("api/Victim/{value}")]
        [HttpPost]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post(string value)
        {

            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString]
                                                                    .ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["messageParam"] = value + " Need Help from you. The User ID =";

            try
            {
                // Send the push notification and log the results.
                var result = await hub.SendTemplateNotificationAsync(templateParams);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Request.RequestUri, value);
            return response;

        }

        // POST: api/Victim
        [Route("api/Victim/activatedm/")]
        [HttpPost]
        public Object[] Post(bool value, string id, [FromBody]Victim victimOBJ)
        {
            DangerMode dmObj = new DangerMode();
            if (value)
            {
                VictimPersistence vp = new VictimPersistence();
                vp.saveUser(victimOBJ);
                dmObj.isDangerModeOn = true;
                dmObj.islive = true;
                dmObj.FBID = id;
                dmObj.Latitude = victimOBJ.Latitude;
                dmObj.Longitude = victimOBJ.Longitude;
                dmObj.isAppLive();
                AllUsersDM.audmInstance.addnewUser(dmObj); //listan
            }
            else
            {
                AllUsersDM.audmInstance.removeUser(id);

            }

            return AllUsersDM.audmInstance.userslist.ToArray();
        }


        [Route("api/Victim/islive")]
        [HttpPost]
        public void Post(bool value, string id, string username)
        {

            AllUsersDM.audmInstance.updateLiveValue(value, id, username);


        }



    }
}
