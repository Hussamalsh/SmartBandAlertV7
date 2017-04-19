using SmartBandAlertV7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7.Data
{
    public class VictimManager
    {
        public IRestService restService;
        public VictimManager(IRestService service)
        {
            restService = service;
        }
        public Task SaveTaskAsync(Victim item, bool isNewItem = false)
        {
            return restService.SaveVictimAsync(item, isNewItem);
        }
        public Task<Victim> SearchVictimAsync(string text)
        {
            return restService.SearchVictimAsync(text);
        }
        public void setDM(Victim item, bool isNewItem = false)
        {
             restService.ActivateDangerMode(item, isNewItem);
        }
        public void setliveMode()
        {
             restService.setAlive();
        }
    }
}
