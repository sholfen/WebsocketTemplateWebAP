using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketTemplateWebAP.Models
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public DateTime ActionTime { get; set; }
        public string Token { get; set; }
    }
}
