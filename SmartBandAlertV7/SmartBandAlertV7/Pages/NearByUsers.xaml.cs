﻿using Acr.UserDialogs;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV7.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NearByUsers : ContentPage
    {
        private Map map;
        public NearByUsers()
        {
            InitializeComponent();


            /*GPSLocation gpsloc = new GPSLocation();
            gpsloc.getLocationAsync(false);*/
            
            map = new Map(MapSpan.FromCenterAndRadius(new Position(56.6713889, 12.8555556), Distance.FromKilometers(20)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;

        }


        protected async override void OnAppearing()
        {

            base.OnAppearing();
            try
            {
                Location[] list;

                using (var cancelSrc = new CancellationTokenSource())
                {
                    using (var cancelSrc1 = new CancellationTokenSource()/*UserDialogs.Instance.Loading("Hämtar plats data", cancelSrc.Cancel, "Cancel")*/)
                    {
                        using (UserDialogs.Instance.Loading("Hämtar platsdata", cancelSrc.Cancel, "Avbryt"))
                        {
                            GPSLocation gpsloc = new GPSLocation();
                            await gpsloc.getUserLocation().ToTask(cancelSrc1.Token);
                            //Task.Delay(6000);
                            list = await App.UserManager.saveUserLocation().ToTask(cancelSrc.Token);
                        }
                    }
                }


                map = new Map(MapSpan.FromCenterAndRadius(new Position(App.Latitude, App.Longitude), Distance.FromKilometers(20)))
                {
                    IsShowingUser = true,
                    HeightRequest = 100,
                    WidthRequest = 960,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                var stack = new StackLayout { Spacing = 0 };
                stack.Children.Add(map);

                Content = stack;

                var slider = new Slider(1, 18, 1);
                slider.ValueChanged += (sender, e) =>
                {
                    var zoomLevel = e.NewValue; // between 1 and 18
                    var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
                    map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, latlongdegrees, latlongdegrees));
                };


                if (list.Length != 0)
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        var loc = list.ElementAt(i);
                        try
                        {/*pin.Clicked += async (sender, e) =>          
                            {                 await DisplayAlert("This is a tag", "Click click, click", "Cancel");  */
                            var pin = new Pin()
                            {
                                Position = new Position(Convert.ToDouble(loc.latitude), Convert.ToDouble(loc.longitude)),
                                Label = "Adda som beskyddare",
                                Type = PinType.SavedPin,
                                BindingContext = loc
                               
                            };
                            pin.Clicked += async (sender, e) =>
                            {
                                var button = sender as Pin;
                                Location todo = button.BindingContext as Location;
                                FriendsList f = new FriendsList()
                                {
                                    FriendFBID = todo.fbid, UserName = todo.userName, ImgLink = null, UserFBID = App.FacebookId
                                };

                                await App.FriendsManager.SaveTaskAsync(f, true);

                                await DisplayAlert("Beskydare", "Du har Addat en ny Beskydare", "Cancel");
                            };
                            map.Pins.Add(pin);
                        }
                        catch (Exception e)
                        {
                            var pin1 = new Pin()
                            {
                                Position = new Position(Convert.ToDouble(loc.latitude.Replace('.', ',')),
                                                        Convert.ToDouble(loc.longitude.Replace('.', ','))),
                                Label = "Adda som beskyddare",
                                Type = PinType.SavedPin,
                                BindingContext = loc
                            };
                            pin1.Clicked += async (senderr, ee) =>
                            {
                                var button = senderr as Pin;
                                Location todo = button.BindingContext as Location;
                                FriendsList f = new FriendsList()
                                {
                                    FriendFBID = todo.fbid, UserName = todo.userName, ImgLink = null, UserFBID = App.FacebookId
                                };

                                await App.FriendsManager.SaveTaskAsync(f, true);

                                await DisplayAlert("Beskydare", "Du har Addat en ny Beskydare", "Cancel");
                            };
                            map.Pins.Add(pin1);
                        }

                    }
                }



            }
            catch (Exception) { }

        }




    }
}
