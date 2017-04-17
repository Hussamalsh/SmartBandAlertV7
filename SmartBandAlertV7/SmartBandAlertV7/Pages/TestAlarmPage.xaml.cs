﻿using Plugin.Geolocator;
using SmartBandAlertV7.Data;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV7.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestAlarmPage : ContentPage
    {
        public static Victim victim;

        public TestAlarmPage()
        {
            victim = new Victim();
            InitializeComponent();
        }

        async void Button_OnClicked(object sender, EventArgs e)
        {
            labelGPS.Text = "Getting gps";
            await getLocationAsync();
        }

        IProfileManager _profileManager;
        IProfileManager profileManager = new ProfileManager();
        Profile _profile;
        void LoadProfile()
        {
            _profile = _profileManager.LoadProfile();
        }

        async void ButtonAlarm_OnClicked(object sender, EventArgs e)
        {
            await App.VictimManager.SaveTaskAsync(victim, true);
        }


        public async Task getLocationAsync()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;


            var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);



            if (position == null)

            {

                labelGPS.Text = "null gps :(";

                return;

            }

            labelGPS.Text = string.Format("Time: {0} \nLat: {1} \nLong: {2} \n Altitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \n Heading: {6} \n Speed: {7}",

               position.Timestamp, position.Latitude, position.Longitude,

               position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

            Geocoder geoCoder = new Geocoder();
            var fortMasonPosition = new Position(position.Latitude, position.Longitude);
            var possibleAddresses = await geoCoder.GetAddressesForPositionAsync(fortMasonPosition);
            //foreach (var a in possibleAddresses)
            //{
            //  labelCity.Text += a + "\n";
            // }
            labelCity.Text = possibleAddresses.FirstOrDefault();




            _profileManager = profileManager;
            LoadProfile();

            victim.FBID = _profile.FBid; ;
            victim.UserName = _profile.FBusername;

            victim.StartDate = DateTime.Parse(position.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            victim.Latitude = "" + position.Latitude.ToString().Replace(",", ".");
            victim.Longitude = "" + position.Longitude.ToString().ToString().Replace(",", ".");
            victim.Adress = "" + possibleAddresses.FirstOrDefault();

        }

    }
}
