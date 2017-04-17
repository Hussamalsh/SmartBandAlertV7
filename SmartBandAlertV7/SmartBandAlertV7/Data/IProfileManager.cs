using SmartBandAlertV7.Models;


namespace SmartBandAlertV7
{
    public interface IProfileManager
    {
        void SaveProfile(Profile profile);
        Profile LoadProfile();
    }
}
