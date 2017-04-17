using Xamarin.Forms;

namespace SmartBandAlertV7.Models
{
    public class FBLoginPage : ContentPage
    {
        public string ProviderName { get; set; }
        public FBLoginPage(string _providername)
        {
            ProviderName = _providername;
        }
    }
}
