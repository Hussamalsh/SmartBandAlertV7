﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using SmartBandAlertV7.Models;
using System.Net.Http.Headers;

namespace SmartBandAlertV7.Data
{
    public class RestService : IRestService
    {

        HttpClient client;

        public List<User> Items { get; private set; }

        public List<FriendsList> Friends { get; private set; }

        public RestService()

        {

            //var authData = string.Format("{0}:{1}", Constants.Username, Constants.Password);

            //var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            client = new HttpClient();
            client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.MaxResponseContentBufferSize = 256000;

            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("APIKey", "5567GGH67225HYVGG");

        }



        public async Task<List<User>> RefreshDataAsync()

        {

            var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/user").Result;
            var users = JsonConvert.DeserializeObject<List<User>>(data);

            return users;

        }


        public async Task<List<FriendsList>> RefreshDataAsyncFriends()

        {

            // Friends = new List<FriendsList>();


            var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/friends/" + App.FacebookId).Result;
            var friends = JsonConvert.DeserializeObject<List<FriendsList>>(data);

            return friends;

        }

        public async Task SaveTodoItemAsync(User item, bool isNewItem = false)

        {

            if (isNewItem)
            {

                var obj = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/user/");
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");

                var data = client.SendAsync(request).Result;

            }
            else
            {

                //response = await client.PutAsync(uri, content);

            }

        }



        public async Task DeleteTodoItemAsync(String userid, String friendid)

        {

            var request = new HttpRequestMessage(HttpMethod.Delete, "http://sbat1.azurewebsites.net/api/friends/" + userid + "/" + friendid);
            var data = client.SendAsync(request).Result;

        }

        public async Task<List<User>> SearchUsersAsync(string text)
        {
            //Items = new List<User>();

            var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/search/" + text).Result;
            var users = JsonConvert.DeserializeObject<List<User>>(data);

            return users;
        }




        public async Task SaveVictimAsync(Victim item, bool isNewItem = false)

        {

            if (isNewItem)
            {

                var obj = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/victim/?pns=gcm&to_tag=" + item.FBID + "T");
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");

                var data = client.SendAsync(request).Result;

            }
            else
            {

                //response = await client.PutAsync(uri, content);

            }

        }

        public async Task<Victim> SearchVictimAsync(string text)
        {

            var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/victim/" + text).Result;
            var victim = JsonConvert.DeserializeObject<Victim>(data);

            return victim;
        }

        public async Task SaveTodoItemAsyncFriend(FriendsList item, bool isNewItem)
        {
            if (isNewItem)
            {

                var obj = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/friends/");
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");

                var data = client.SendAsync(request).Result;

            }
            else
            {

                //response = await client.PutAsync(uri, content);

            }
        }

        public void SaveUserLocationAsync(Location userloc)
        {
           var obj = JsonConvert.SerializeObject(userloc, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
           var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/location/");
           request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
           var data = client.SendAsync(request).Result;
        }

        public void editUserLocationAsync(Location userloc)
        {
            var obj = JsonConvert.SerializeObject(userloc, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(HttpMethod.Put, "https://sbat1.azurewebsites.net/api/location/");
            request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
            var data = client.SendAsync(request).Result;
        }





    }
}
