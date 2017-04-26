using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7.Data
{
    public interface IRestService
    {
        Task<List<User>> RefreshDataAsync();
        //User
        Task<List<User>> SearchUsersAsync(string text);
        IObservable<Location[]> SaveUserLocationAsync();//getnearbyusers
        IObservable<Location[]> test();
        void editUserLocationAsync(Location userloc);
        //Friend
        IObservable<FriendsList[]> RefreshDataAsyncFriends();
        Task SaveTodoItemAsyncFriend(FriendsList item, bool isNewItem);
        Task SaveTodoItemAsync(User item, bool isNewItem);
        //Victim
        Task SaveVictimAsync(Victim item, bool isNewItem);
        Task<Victim> SearchVictimAsync(string text);
         void ActivateDangerMode(Victim item, bool isNewItem);

        void setAlive();

        //
        Task DeleteTodoItemAsync(String userid, String friendid);

    }
}
