using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBandAlertV7.Models
{
    public class FriendsList
    {
        public String FriendFBID { get; set; }
        public String UserFBID { get; set; }
        public String UserName { get; set; }
        public String ImgLink { get; set; }
        public int Status { get; set; }

        public String Sourceimg { get; set; }

        public String FriendReq { get; set; }
        public bool AddFriend { get; set; }

    }
}
