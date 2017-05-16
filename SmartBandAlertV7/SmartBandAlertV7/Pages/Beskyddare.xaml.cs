using Acr.UserDialogs;
using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
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


            friendEXISTINGView.ItemsSource = new string[] { "Loading friends", "Loading friends", "Loading friends", "Loading friends" };

        }


        protected async override void OnAppearing()
        {

            base.OnAppearing();

            getAllFriends();

        }

        public async void getAllFriends()
        {
            try
            {
                //FriendsList[] list = null;
                ObservableCollection<FriendsList> list = new ObservableCollection<FriendsList>();
                using (var cancelSrc = new CancellationTokenSource())
                {
                    using (var dlg = UserDialogs.Instance.Progress("Hämtar data", cancelSrc.Cancel, "Avbryt"))
                    {
                        while (dlg.PercentComplete < 100)
                        {

                            await Task.Delay(500);
                            if (dlg.PercentComplete == 0)
                            {
                                dlg.PercentComplete += 20;

                                var fR = await App.FriendsManager.GetTasksAsync().ToTask(cancelSrc.Token);
                                foreach (FriendsList f in fR)
                                {
                                    if ((f.UserFBID.Equals(App.FacebookId) && f.Status == 1) ||
                                        (f.FriendFBID.Equals(App.FacebookId) && f.Status == 1)) //added friends
                                    {
                                        f.Sourceimg = "trash.png";
                                        list.Add(f);
                                    }
                                    else if (f.FriendFBID.Equals(App.FacebookId) && f.Status == 0) //friend request by others to be denied or accepted
                                    {
                                        f.Sourceimg = "check.png";
                                        //f.FriendReq = "Acceptera vänförfrågan";
                                        f.AddFriend = true;
                                        list.Add(f);
                                    }
                                }
                                friendEXISTINGView.ItemsSource = list.OrderBy(x => x.Status);
                                
                            }
                            dlg.PercentComplete += 20;
                        }
                    }
                }
            }
            catch (Exception) { }


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

            var answer = await DisplayAlert("Beskyddare", "Vill du lägga till den här beskyddaren?", "Ja", "Nej");

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
            //var list = await App.FriendsManager.GetTasksAsync();
            /* using (var cancelSrc = new CancellationTokenSource())
             {
                 using (UserDialogs.Instance.Loading("Hämtar data", cancelSrc.Cancel, "Cancel"))
                 {
                     friendEXISTINGView.ItemsSource = await App.FriendsManager.GetTasksAsync().ToTask(cancelSrc.Token);
                 }

             }*/
            getAllFriends();

        }

        async void trashTapped(object sender, EventArgs args)
        {
            //((Image)sender).Opacity = 0.5;

            //((Image)sender).Opacity = 1;

                var button = sender as Image;
                FriendsList item = button.BindingContext as FriendsList;
                await CompleteItem(item);
        }

        async Task CompleteItem(FriendsList item)
        {


            if (!item.AddFriend)
            {
                var answer = await DisplayAlert("Beskyddare", "Vill du ta bort den här beskyddaren?", "Ja", "Nej");

                //await manager.SaveTaskAsync(item);
                if (answer)
                {
                    App.FriendsManager.DeleteTaskAsync(item);
                }
                else
                    return;
            }
            else   //acceptera vänförfrågan
            {
                var answer = await DisplayAlert("Beskyddare", "Vill du Acceptera vänförfrågan?", "Ja", "Nej");
                if (answer)
                {
                    item.Status = 1;
                    App.FriendsManager.UpdateFriendRequest(item);
                }

            }

            /* using (var cancelSrc = new CancellationTokenSource())
             {
                 using (UserDialogs.Instance.Loading("Hämtar data", cancelSrc.Cancel, "Cancel"))
                 {
                     friendEXISTINGView.ItemsSource = await App.FriendsManager.GetTasksAsync().ToTask(cancelSrc.Token);
                 }
             }*/
            getAllFriends();

        }
        async void rejectFR(object sender, EventArgs args)
        {
            //((Image)sender).Opacity = 0.5;
            var answer = await DisplayAlert("Beskyddare", "Vill du Avvisa vänförfrågan?", "Ja", "Nej");

            //((Image)sender).Opacity = 1;
            if (answer)
            {
                var button = sender as Image;
                FriendsList item = button.BindingContext as FriendsList;
                await Rejectfriend(item);
            }
        }

        async Task Rejectfriend(FriendsList item)
        {

            //await manager.SaveTaskAsync(item);
            App.FriendsManager.DeleteTaskAsync(item);
            /*var list = await App.FriendsManager.GetTasksAsync();
            friendEXISTINGView.ItemsSource = list;*/
            /* using (var cancelSrc = new CancellationTokenSource())
             {
                 using (UserDialogs.Instance.Loading("Hämtar data", cancelSrc.Cancel, "Cancel"))
                 {
                     friendEXISTINGView.ItemsSource = await App.FriendsManager.GetTasksAsync().ToTask(cancelSrc.Token);
                 }
             }*/
            getAllFriends();

        }

        private async void MainSearchBar_OnSearchButtonPressed(object sender, EventArgs e)
        {
            string keyword = searchFriends.Text;
            var list = await App.UserManager.SearchUsersAsync(keyword);
            friendSEARCHView.ItemsSource = list;

        }

    }
}
