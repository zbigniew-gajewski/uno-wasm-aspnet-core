using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace UnoWeb.Test
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}