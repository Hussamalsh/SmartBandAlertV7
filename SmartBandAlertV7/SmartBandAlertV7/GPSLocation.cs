using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7
{
    public class GPSLocation
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }

       /* public GPSLocation()
        {
        }*/

        public async Task<Plugin.Geolocator.Abstractions.Position> getLocationAsync()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

            if (position == null)

            {
                return null;
            }

            /*this.Latitude = position.Latitude;
            this.Longitude = position.Longitude;*/

            return position;

        }



    }
}
