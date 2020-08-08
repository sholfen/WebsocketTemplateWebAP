using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketTemplateWebAP.Models.WebSocketModels.Responses
{
    public class ErrorResponseModel : BaseResponseModel
    {
        public string ErrorCode { get; set; }
        public string Message
        {
            get; set;
        }
    }
}
