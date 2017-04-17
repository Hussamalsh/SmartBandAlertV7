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
    public partial class Beskyddare : ContentPage
    {
        public Beskyddare()
        {
            InitializeComponent();

            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.White };
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.FromHex("#ededed") };


            friendEXISTINGView.IsVisible = true;
            friendSEARCHView.IsVisible = false;
            searchFriends.IsVisible = false;
            friendEXISTINGView.ItemSelected += (sender, e) => friendEXISTINGView.SelectedItem = null;
            friendSEARCHView.ItemSelected += (sender, e) => friendSEARCHView.SelectedItem = null;

        }


        protected async override void OnAppearing()
        {

            base.OnAppearing();
            //var list = await App.UserManager.GetTasksAsync();
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;

        }


        void topButtonClicked(object sender, EventArgs e)
        {
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.FromHex("#ededed") };
            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.White };
            friendEXISTINGView.IsVisible = true;
            friendSEARCHView.IsVisible = false;
            searchFriends.IsVisible = false;
        }

        async void topRightButtonClicked(object sender, EventArgs e)
        {
            friendSEARCHView.ItemsSource = await App.UserManager.GetTasksAsync();
            topButton.BindingContext = new { text = "Existerande beskyddare", w0 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc1 = Color.White };
            topRightButton.BindingContext = new { text2 = "Sök beskyddare", w1 = App.ScreenWidth * 160 / (App.ScreenDPI * 2), bgc2 = Color.FromHex("#ededed") };
            friendEXISTINGView.IsVisible = false;
            friendSEARCHView.IsVisible = true;
            searchFriends.IsVisible = true;
        }

        async void checkTapped(object sender, EventArgs args)
        {

            var answer = await DisplayAlert("Beskyddare", "Vill du lägga till den här beskyddaren?", "Nej", "Ja");

            if (answer)
            {
                var button = sender as Image;
                User todo = button.BindingContext as User;

                FriendsList f = new FriendsList() { FriendFBID = todo.FBID, UserName = todo.UserName, ImgLink = todo.ImgLink, UserFBID = App.FacebookId };
                await CompleteAdd(f);
            }

        }

        async Task CompleteAdd(FriendsList item)
        {
            await App.FriendsManager.SaveTaskAsync(item, true);
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;
        }

        async void trashTapped(object sender, EventArgs args)
        {
            //((Image)sender).Opacity = 0.5;
            var answer = await DisplayAlert("Beskyddare", "Vill du ta bort den här beskyddaren?", "Ja", "Nej");

            //((Image)sender).Opacity = 1;
            if (answer)
            {
                var button = sender as Image;
                FriendsList item = button.BindingContext as FriendsList;
                await CompleteItem(item);
            }
        }

        async Task CompleteItem(FriendsList item)
        {

            //await manager.SaveTaskAsync(item);
            App.FriendsManager.DeleteTaskAsync(item);
            var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;
        }

        private async void MainSearchBar_OnSearchButtonPressed(object sender, EventArgs e)
        {
            string keyword = searchFriends.Text;
            var list = await App.UserManager.SearchUsersAsync(keyword);
            friendSEARCHView.ItemsSource = list;

        }

    }
}