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
    public class UserLoginCommand : BaseCommand
    {
        public override string Execute(string jsonStr)
        {
            string returnJsonStr = string.Empty;
            try
            {
                DBRepository dBRepository = new DBRepository();
                UserLoginModel model = JsonSerializer.Deserialize<UserLoginModel>(jsonStr);

                //reconnection logic
                if (!string.IsNullOrEmpty(model.Token))
                {
                    if (WebSocketManager.WebSocketsDict.ContainsKey(model.Token))
                    {
                        WebSocketManager.WebSocketsDict[model.Token] = base.WebSocket;
                        BaseResponseModel responseModel = new BaseResponseModel();
                        responseModel.StatusCode = 1;
                        return JsonSerializer.Serialize(responseModel);
                    }
                }

                string token = Guid.NewGuid().ToString();
                UserInfo result = dBRepository.UserLoginLogic(model.UserName, model.Password);

                if (result!=null)
                {
                    //remove duplite user first
                    WebSocketManager.RemoveUserInfoAndWebSocketByUserName(model.UserName);

                    UserLoginResponseModel responseModel = new UserLoginResponseModel();
                    responseModel.StatusCode = 1;
                    responseModel.UserName = model.UserName;
                    responseModel.Token = token;
                    UserInfo userInfo = new UserInfo
                    {
                        Token = responseModel.Token,
                    };
                    //check table number
                    WebSocketManager.AddUserInfoAndWebSocket(userInfo.Token, userInfo, WebSocket);
                    returnJsonStr = JsonSerializer.Serialize(responseModel);
                }
                else
                {
                    ErrorResponseModel responseModel = new ErrorResponseModel();
                    responseModel.StatusCode = 0;
                    responseModel.ErrorCode = "500";
                    responseModel.Message = "error";
                    returnJsonStr = JsonSerializer.Serialize(responseModel);
                }
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
