using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV7.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapNavigationPage : ContentPage
    {
        //public Victim v = new Victim();

        public static double Latitude
        {
            get;
            set;
        }

        public static double Longitude
        {
            get;
            set;
        }

        public MapNavigationPage(double lat, double longi)
        {
            InitializeComponent();

            Latitude = lat;
            Longitude = longi;
            getVictim();
        }

        void OnNavigateButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }
        public async void getVictim()
        {
            //var v1 = App.VictimManager.SearchVictimAsync(victimId);
           // v = v1.Result;

            inputEntry.Text = Latitude.ToString().Replace(",", ".") + "," + Longitude.ToString().Replace(",", ".");

            if (!string.IsNullOrWhiteSpace(inputEntry.Text))
            {
                var address = inputEntry.Text;
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                            new Uri(string.Format("http://maps.apple.com/?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                            new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(address))));
                        break;
                    case TargetPlatform.Windows:
                    case TargetPlatform.WinPhone:
                        Device.OpenUri(
                            new Uri(string.Format("bingmaps:?where={0}", Uri.EscapeDataString(address))));
                        break;
                }
            }
        }
    }
}
