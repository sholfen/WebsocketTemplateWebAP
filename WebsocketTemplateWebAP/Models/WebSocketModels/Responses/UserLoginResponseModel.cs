using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketTemplateWebAP.Models.WebSocketModels.Responses
{
    public class UserLoginResponseModel : BaseResponseModel
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
