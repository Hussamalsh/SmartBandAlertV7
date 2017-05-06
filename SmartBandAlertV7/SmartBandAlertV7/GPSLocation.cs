using Plugin.Geolocator;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace SmartBandAlertV7
{
    public class GPSLocation
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeStamploc { get; set; }
        public string adress { set; get; }

        /* public GPSLocation()
         {
         }*/
        public Victim victim { set; get; } = new Victim();
        public async void getLocationAsync(bool postdata)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                locator.AllowsBackgroundUpdates = true;

                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                if (position == null)
                {
                    return;
                }



                App.Latitude = Latitude = position.Latitude;
                App.Longitude = Longitude = position.Longitude;
                TimeStamploc = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));

                Geocoder geoCoder = new Geocoder();
                var fortMasonPosition = new Position(Latitude, Longitude);
                var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(fortMasonPosition);

                adress = possibleAddresses.FirstOrDefault();

                if (postdata)
                {
                    App.UserManager.editUserLocation(new Models.Location
                    {
                        fbid = App.FacebookId,
                        userName = App.FacebookName,
                        latitude = Latitude.ToString().Replace(",", "."),
                        longitude = Longitude.ToString().Replace(",", "."),
                        distance = ""
                    });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            



        }

        public  IObservable<object> getUserLocation()
        {
            return Observable.Create<object>(async ob =>
            {
                var cancelSrc = new CancellationTokenSource();



                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                if (position == null)
                {
                    return null;
                }
                ob.OnNext(Latitude);
                
                App.Latitude = Latitude = position.Latitude;
                App.Longitude = Longitude = position.Longitude;
                TimeStamploc = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));



                ob.OnCompleted();
                

                return () =>
                {
                    //connected?.Dispose();
                    cancelSrc.Dispose();
                };
            });
        }


        public void getvictimLocationAsync()
        {
            victim.FBID = App.FacebookId;
            victim.UserName = App.FacebookName;

            victim.StartDate = TimeStamploc;
            victim.Latitude = "" + Latitude.ToString().Replace(",", ".");
            victim.Longitude = "" + Longitude.ToString().ToString().Replace(",", ".");
            victim.Adress = "" + adress;
        }



    }
}
