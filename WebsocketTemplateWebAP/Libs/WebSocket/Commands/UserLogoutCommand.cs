using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebsocketTemplateWebAP.Libs.DBTools;
using WebsocketTemplateWebAP.Models;
using WebsocketTemplateWebAP.Models.WebSocketModels.Requests;
using WebsocketTemplateWebAP.Models.WebSocketModels.Responses;

namespace WebsocketTemplateWebAP.Libs.WebSocket.Commands
{
    public class UserLogoutCommand : BaseCommand
    {
        public override string Execute(string jsonStr)
        {
            string returnJsonStr = string.Empty;
            DBRepository dBRepository = new DBRepository();
            try
            {
                UserLogoutModel model = JsonSerializer.Deserialize<UserLogoutModel>(jsonStr);
                UserInfo userInfo = WebSocketManager.UserInfoDict[model.Token];
                dBRepository.UserLogoutSP(userInfo.UserName);
                BaseResponseModel responseModel = new BaseResponseModel();
                responseModel.StatusCode = 1;
                returnJsonStr = JsonSerializer.Serialize(responseModel);
                WebSocketManager.RemoveUserInfoAndWebSocket(model.Token);
            }
            catch (Exception ex)
            {
                ErrorResponseModel responseModel = new ErrorResponseModel();
                responseModel.StatusCode = 0;
                responseModel.ErrorCode = "500";
                responseModel.Message = ex.Message;
                returnJsonStr = JsonSerializer.Serialize(responseModel);
            }

            return returnJsonStr;
        }
    }
}
