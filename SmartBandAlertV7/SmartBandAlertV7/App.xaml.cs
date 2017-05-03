using SmartBandAlertV7.Data;
using SmartBandAlertV7.Models;
using SmartBandAlertV7.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace SmartBandAlertV7
{
    public partial class App : Application
    {

        public static int ScreenWidth;
        public static int ScreenHeight;
        public static int ScreenDPI;

        #region Facebook Auth Settings
        public static string AppId = "386661445019743";
        public static string DisplayName = "SBA App";
        public static string ExtendedPermissions = "user_about_me,email,public_profile";
        public static string AuthorizeUrl = "https://www.facebook.com/dialog/oauth";
        public static string RedirectUrl = "https://www.facebook.com/connect/login_success.html";
        #endregion


        #region Google Auth Settings
        public static string ClientId = "1061097097490-h79pp9ru9a35p0659r2jcnvou0lp3nr8.apps.googleusercontent.com";
        public static string ClientSecret = "3WSsdUfTIfk_6GDdOhWLTbTF";
        public static string url1 = "https://www.googleapis.com/auth/userinfo.emai";
        public static string url2 = "https://accounts.google.com/o/oauth2/auth";
        public static string url3 = "https://www.youtube.com/channel/UCOjakhXt0i52uwYRjYBKjsg";
        public static string url4 = "https://accounts.google.com/o/oauth2/token";
        #endregion

        readonly IProfileManager _profileManager;
        public Profile _profile;

        public static UserManager UserManager { get; private set; }
        public static FriendsManager FriendsManager { get; private set; }
        public static VictimManager VictimManager { get; private set; }
        public static BLEAcrProfileManager BLEAcrProfileManager { get; private set; }

        public static IAuthenticate Authenticator { get; private set; } //initialize the interface with a platform-specific implementation
        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        public App()
        {
            InitializeComponent();


            UserManager = new UserManager(new RestService());
            FriendsManager = new FriendsManager(new RestService());
            VictimManager = new VictimManager(new RestService());

            BLEAcrProfileManager = new BLEAcrProfileManager();

            IProfileManager profileManager = new ProfileManager();
            _profileManager = profileManager;
            LoadProfile();


            PresentMainPage();


        }


        public void PresentMainPage()
        {
            if (IsLoggedIn)
            {
                sendUserLocationAsync();
            }
            if (NotificationOn)
            {
                MainPage = new MapNavigationPage(Latitude, Longitude);  //pass id, latitude and longitude.
                NotificationOn = false;
            }
            else
            {
                MainPage = !IsLoggedIn ? (Page)new LoginPage() : new MainPage();
            }

        }

        public void SaveProfile()
        {
            _profile.FBusername = FacebookName;
            _profile.FBimage = ProfilePic;
            _profile.FBid = FacebookId;
            _profile.FBemail = EmailAddress;
            _profile.HaveSmartBand = HaveSmartBand;
            _profile.BlegUID = BlegUID;
            _profileManager.SaveProfile(_profile);
        }
        void LoadProfile()
        {
            _profile = _profileManager.LoadProfile();
            FacebookId = _profile.FBid;
            FacebookName = _profile.FBusername;
            ProfilePic = _profile.FBimage;
            EmailAddress = _profile.FBemail;
            HaveSmartBand = _profile.HaveSmartBand;
            BlegUID = _profile.BlegUID;
        }

        public static bool IsLoggedIn
        {
            get;
            set;
        }

        public static bool NotificationOn
        {
            get;
            set;
        }


        public static bool HaveSmartBand
        {
            get;
            set;
        }

        public static String BlegUID
        {
            get;
            set;
        }
        public static String FacebookId
        {
            get;
            set;
        }

        public static string FacebookName
        {
            get;
            set;
        }

        public static string EmailAddress
        {
            get;
            set;
        }

        public static string ProfilePic
        {
            get;
            set;
        }

        public static bool isConnectedBLE
        {
            get;
            set;
        }

        public static bool dangerModeOn
        {
            get;
            set;
        }

        public static CancellationTokenSource ct
        {
            get;
            set;
        }

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
        public static String VictimId
        {
            get;
            set;
        }


        protected override void OnStart()
        {
            /* // Handle when your app starts
             Device.StartTimer(TimeSpan.FromSeconds(180), () =>
             {
                 // Do something
                 sendUserLocationAsync();
                 return false; // True = Repeat again, False = Stop the timer
             });*/
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public void sendUserLocationAsync()
        {
            Device.StartTimer(TimeSpan.FromSeconds(100), () =>
            {
                // Do something
                GPSLocation gpsloc = new GPSLocation();
                gpsloc.getLocationAsync(true);
                return false; // True = Repeat again, False = Stop the timer
            });

        }
    }
}
