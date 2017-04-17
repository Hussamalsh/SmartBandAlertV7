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
    public partial class Profil : ContentPage
    {
        public Profil()
        {
            InitializeComponent();

            profilbild.Source = App.ProfilePic;
            Visningsnamn.Detail = App.FacebookName;
            Mailadress.Detail = App.EmailAddress;
            FacegoogID.Detail = App.FacebookId;
            location.Detail = "Halmstad";
        }
    }
}
