using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backendt1.Models
{
    public class AllUsersDM
    {
        public static AllUsersDM audmInstance = new AllUsersDM();
        public ArrayList userslist;

        public AllUsersDM()
        {
            userslist = new ArrayList();
        }

        public void addnewUser(DangerMode dm)
        {
            if (userslist.Count != 0)
            {
                bool hasItem = userslist.Cast<DangerMode>().Any(i => i.FBID == dm.FBID);
                if (hasItem)
                {
                    removeUser(dm.FBID);
                }
            }
            userslist.Add(dm);
        }

        public void removeUser(string id)
        {
            var query = from DangerMode dmobj in userslist
                        where dmobj.FBID == id
                        select dmobj;

            foreach (DangerMode dm in query)
            {

                dm.stopTimer();
                dm.isDangerModeOn = false;
                userslist.RemoveAt(userslist.IndexOf(dm));
            }
        }

        public void updateLiveValue(bool value, string id, string username)
        {
            var query = from DangerMode dmobj in userslist
                        where dmobj.FBID == id
                        select dmobj;

            foreach (DangerMode dm in query)
            {
                dm.islive = value;
                dm.UserName = username;
            }
        }



    }
}