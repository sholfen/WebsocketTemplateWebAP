using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketTemplateWebAP.Models.WebSocketModels.Requests
{
    public class UserLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
