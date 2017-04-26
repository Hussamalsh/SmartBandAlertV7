using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7.Data
{
    public class UserManager
    {
        IRestService restService;
        public UserManager(IRestService service)
        {
            restService = service;
        }

        public Task<List<User>> GetTasksAsync()
        {
            return restService.RefreshDataAsync();
        }

        public Task SaveTaskAsync(User item, bool isNewItem = false)
        {
            return restService.SaveTodoItemAsync(item, isNewItem);
        }
        public Task DeleteTaskAsync(User item)
        {
          // return restService.DeleteTodoItemAsync(item.FBID);
            return null;
        }
        //User
        public IObservable<Location[]> saveUserLocation()
        {
            return restService.SaveUserLocationAsync();
        }
        public void editUserLocation(Location userloc)
        {
            restService.editUserLocationAsync(userloc);
        }
        public IObservable<Location[]> testone()
        {
            return restService.test();
        }
        public Task<List<User>> SearchUsersAsync(string text)
        {
            return restService.SearchUsersAsync(text);
        }

    }
}
