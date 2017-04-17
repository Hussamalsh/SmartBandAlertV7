using System.Threading.Tasks;

namespace SmartBandAlertV7.Data
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync();
        bool LogoutAsync();
    }
}
