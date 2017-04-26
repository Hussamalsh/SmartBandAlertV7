﻿using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartBandAlertV7.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()

        {

            InitializeComponent();

            this.Icon = "hamburger.png";

            var masterPageItems = new List<MasterPageItem>();


           
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Hem",
                IconSource = "hem.png",
                TargetType = typeof(HemPage)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Profil",
                IconSource = "profil.png",
                TargetType = typeof(Profil)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Beskyddare",
                IconSource = "beskyddare.png",
                TargetType = typeof(Beskyddare)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "NearByUsers",
                IconSource = "om.png",
                TargetType = typeof(NearByUsers)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "TestAlarm",
                IconSource = "hjalp.png",
                TargetType = typeof(TestAlarmPage)

            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Hjälp",
                IconSource = "hjalp.png",
                TargetType = typeof(Hjalp)

            });
            
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Om",
                IconSource = "om.png",
                TargetType = typeof(Om)
            });
            
            masterPageItems.Add(new MasterPageItem
            {
                Title = "LogOut",
                IconSource = "om.png",
                TargetType = typeof(LogoutPage)
            });

            imgSRC.Source = App.ProfilePic;
            nameSet.Text = App.FacebookName;

            listView.ItemsSource = masterPageItems;

        }
    }
}
