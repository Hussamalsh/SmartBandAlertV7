using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using SmartBandAlertV7.Models;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Threading;

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

        public  IObservable<FriendsList[]> RefreshDataAsyncFriends()
        {
            return Observable.Create<FriendsList[]>(ob =>
            {
                var cancelSrc = new CancellationTokenSource();
                var data = client.GetStringAsync("http://sbat1.azurewebsites.net/api/friends/" + App.FacebookId).Result;
                var friends = JsonConvert.DeserializeObject<List<FriendsList>>(data);
                ob.OnNext(friends.ToArray());
                ob.OnCompleted();
                return () =>
                {
                    //connected?.Dispose();
                    cancelSrc.Dispose();
                };
            });
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

        public IObservable<Location[]> SaveUserLocationAsync()
        {
            return Observable.Create<Location[]>(ob =>
            {
                var cancelSrc = new CancellationTokenSource();


                var data = client.GetStringAsync("https://sbat1.azurewebsites.net/api/location?latitude="
                                           + App.Latitude.ToString().Replace(',','.') + "&longitude=" + App.Longitude.ToString().Replace(',', '.')).Result;
                var list = JsonConvert.DeserializeObject<List<Location>>(data);

                ob.OnNext(list.ToArray());
                ob.OnCompleted();

                return () =>
                {
                    //connected?.Dispose();

                    cancelSrc.Dispose();
                };
            });
        }

        public IObservable<Location[]> test()
        {
            return Observable.Create<Location[]>(ob =>
            {
                var cancelSrc = new CancellationTokenSource();


                var data = client.GetStringAsync("https://sbat1.azurewebsites.net/api/location?latitude="
                                           + App.Latitude + "&longitude=" + App.Longitude).Result;
                var list = JsonConvert.DeserializeObject<List<Location>>(data);

                ob.OnNext(list.ToArray());
                ob.OnCompleted();

                return () =>
                {
                    //connected?.Dispose();
                    
                    cancelSrc.Dispose();
                };
            });



        }


        public async void editUserLocationAsync(Location userloc)
        {
            var obj = JsonConvert.SerializeObject(userloc, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            if (!App.HaveSmartBand)
            {
                var request1 = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/Location");
                request1.Content = new StringContent(obj, Encoding.UTF8, "application/json");
                var data = client.SendAsync(request1).Result;
                App.HaveSmartBand = true; //change the name of the variabel-)
            }
            else
            {
                var request = new HttpRequestMessage(HttpMethod.Put, "https://sbat1.azurewebsites.net/api/Location");
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
                //data = client.SendAsync(request).Result;
                var response1 = await client.PutAsync("https://sbat1.azurewebsites.net/api/Location", request.Content);
            }


            

        }

        public void ActivateDangerMode(Victim item, bool isNewItem)
        {
            //https://sbat1.azurewebsites.net/api/Victim/activatedm?value=true&id=132569873917640
            var obj = JsonConvert.SerializeObject(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/Victim/activatedm?value=true&id="
                                                                      +App.FacebookId);
            if (isNewItem)
             {
                request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/Victim/activatedm?value=true&id="
                                                                          + App.FacebookId);
            }
             else
             {
                request = new HttpRequestMessage(HttpMethod.Post, "https://sbat1.azurewebsites.net/api/Victim/activatedm?value=false&id="
                                                                          + App.FacebookId);
            }
             request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
             var data = client.SendAsync(request).Result;
        }

        public void setAlive()
        {
            //http://localhost:61212/api/Victim/islive?value=true&id=132569873917640&username=waddod
           
            var obj = JsonConvert.SerializeObject("", new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            var request = new HttpRequestMessage(HttpMethod.Post,
                                "https://sbat1.azurewebsites.net/api/Victim/islive?value=true&id="+App.FacebookId
                                +"&username=" + App.FacebookName);
            request.Content = new StringContent(obj, Encoding.UTF8, "application/json");

            var data = client.SendAsync(request).Result;
        }
    }
}
