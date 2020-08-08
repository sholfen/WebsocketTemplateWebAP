using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebsocketTemplateWebAP.Libs.WebSocket.Commands;
using WebsocketTemplateWebAP.Models;
using WebsocketTemplateWebAP.Models.WebSocketModels.Requests;
using WebsocketTemplateWebAP.Models.WebSocketModels.Responses;

namespace WebsocketTemplateWebAP.Libs.WebSocket
{
    public static class WebSocketManager
    {
        public static ConcurrentDictionary<string, Type> CommandsDict;
        public static ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> WebSocketsDict;
        public static ConcurrentDictionary<string, UserInfo> UserInfoDict;
        public static ConcurrentDictionary<string, UserInfo> UserNameMappingUserInfoDict;

        static WebSocketManager()
        {
            CommandsDict = new ConcurrentDictionary<string, Type>();
            CommandsDict.TryAdd("UserLogin", typeof(UserLoginCommand));
            CommandsDict.TryAdd("UserLogout", typeof(UserLogoutCommand));

            WebSocketsDict = new ConcurrentDictionary<string, System.Net.WebSockets.WebSocket>();

            UserInfoDict = new ConcurrentDictionary<string, UserInfo>();

            UserNameMappingUserInfoDict = new ConcurrentDictionary<string, UserInfo>();
        }

        public static void AddUserInfoAndWebSocket(string token, UserInfo userInfo, System.Net.WebSockets.WebSocket websock)
        {
            WebSocketsDict.TryAdd(token, websock);
            UserInfoDict.TryAdd(token, userInfo);
            UserNameMappingUserInfoDict.TryAdd(userInfo.UserName, userInfo);
        }

        public static void RemoveUserInfoAndWebSocket(string token)
        {
            System.Net.WebSockets.WebSocket webSocket = WebSocketsDict[token];
            UserInfo userInfo = UserInfoDict[token];
            UserNameMappingUserInfoDict.TryRemove(userInfo.UserName, out UserInfo info);
            WebSocketsDict.TryRemove(token, out System.Net.WebSockets.WebSocket socket);
            UserInfoDict.TryRemove(token, out UserInfo info2);
            try
            {
                webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "AutoLogout", CancellationToken.None);
            }
            catch (Exception ex)
            {

            }
        }

        public static void RemoveUserInfoAndWebSocketByUserName(string userName)
        {
            if (UserNameMappingUserInfoDict.ContainsKey(userName))
            {
                UserInfo userInfo = UserNameMappingUserInfoDict[userName];
                RemoveUserInfoAndWebSocket(userInfo.Token);
            }
        }

        public static async Task ExecuteAPI(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            var buffer = new byte[1024 * 20];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                string strRequest = Encoding.UTF8.GetString(buffer);
                string str = strRequest.Replace("\0", string.Empty);
                string jsonStr = string.Empty;

                try
                {
                    //Method
                    APIModel apiModel = JsonSerializer.Deserialize<APIModel>(str);
                    string apiName = apiModel.Method;
                    BaseCommand command = Activator.CreateInstance(CommandsDict[apiName]) as BaseCommand;
                    command.WebSocket = webSocket;
                    jsonStr = command.Execute(str);
                    buffer = Encoding.UTF8.GetBytes(jsonStr);
                    BaseRequestModel requestModel = JsonSerializer.Deserialize<BaseRequestModel>(str);
                    if (!string.IsNullOrEmpty(requestModel.Token))
                    {
                        if (command is UserLogoutCommand)
                        {
                            //do nothing
                        }
                        else
                        {
                            UserInfo userInfo = UserInfoDict[requestModel.Token];
                            userInfo.ActionTime = DateTime.Now;
                        }
                    }
                    else if (command is UserLoginCommand)
                    {
                        //do nothing
                    }
                }
                catch (Exception ex)
                {
                    ErrorResponseModel responseModel = new ErrorResponseModel();
                    responseModel.StatusCode = 0;
                    responseModel.ErrorCode = "500";
                    responseModel.Message = ex.Message;
                    jsonStr = JsonSerializer.Serialize(responseModel);
                    buffer = Encoding.UTF8.GetBytes(jsonStr);
                }

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, jsonStr.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                buffer = new byte[1024 * 20];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public static void SendMessageToAll(string message)
        {
            byte[] buffer = buffer = Encoding.UTF8.GetBytes(message);
            var websockets = WebSocketsDict.Values;
            Parallel.ForEach(websockets, item =>
            {
                item.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            });
        }

        public static void SendMessage(string userName, string message)
        {
            byte[] buffer = buffer = Encoding.UTF8.GetBytes(message);
            string token = UserNameMappingUserInfoDict[userName].Token;
            var websocket = WebSocketsDict[token];
            websocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
