using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsocketTemplateWebAP.Libs.WebSocket.Commands
{
    public abstract class BaseCommand
    {
        public abstract string Execute(string jsonStr);
        public System.Net.WebSockets.WebSocket WebSocket { get; set; }
    }
}
