
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AppStoreApiWithIdentityServer4.Handlers
{
    public class NotificationsHub: Hub
    {
        public async Task SendNotification( string message)
        {
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
            if (Clients != null)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
        }
    }
}


