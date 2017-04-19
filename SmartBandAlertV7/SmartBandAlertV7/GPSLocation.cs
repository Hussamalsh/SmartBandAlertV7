using Plugin.Geolocator;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace SmartBandAlertV7
{
    public class GPSLocation
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeStamploc { get; set; }

        /* public GPSLocation()
         {
         }*/
        public Victim victim { set; get; } = new Victim();
        public async void getLocationAsync()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

            if (position == null)
            {
                return;
            }

            Latitude  = position.Latitude;
            Longitude = position.Longitude;
            TimeStamploc = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public async Task getvictimLocationAsync()
        {
            Geocoder geoCoder = new Geocoder();
            var fortMasonPosition = new Position(Latitude, Longitude);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(fortMasonPosition);

            victim.FBID = App.FacebookId;
            victim.UserName = App.FacebookName;

            victim.StartDate = TimeStamploc;
            victim.Latitude = "" + Latitude.ToString().Replace(",", ".");
            victim.Longitude = "" + Longitude.ToString().ToString().Replace(",", ".");
            victim.Adress = "" + possibleAddresses.FirstOrDefault();
        }



    }
}
