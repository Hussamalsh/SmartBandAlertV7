using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7.Data
{
    public class FriendsManager
    {
        IRestService restService;
        public FriendsManager(IRestService service)
        {
            restService = service;
        }
        public IObservable <FriendsList[]> GetTasksAsync()
        {
            return restService.RefreshDataAsyncFriends();
        }
        public Task SaveTaskAsync(FriendsList item, bool isNewItem = false)
        {
            return restService.SaveTodoItemAsyncFriend(item, isNewItem);
        }
        public Task DeleteTaskAsync(FriendsList item)
        {
            return restService.DeleteTodoItemAsync(item.UserFBID, item.FriendFBID);
        }

        public void UpdateFriendRequest(FriendsList fr)
        {
            restService.acceptFriendReq(fr);
        }
    }
}
