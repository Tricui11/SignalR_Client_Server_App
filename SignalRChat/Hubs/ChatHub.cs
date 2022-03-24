using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task DisconnectUser(string userID)
        {
            await Clients.Client(userID).SendAsync("Disconnect", userID);
        }

        public async Task SendMessage(string message, string userName)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, userName);
        }

        public async Task AddToAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            UserHandler.ConnectedUsers[Context.ConnectionId] = "Admins";
        }

        public async Task AddToGuestGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Guests");
            UserHandler.ConnectedUsers[Context.ConnectionId] = "Guests";
            await Clients.Group("Admins").SendAsync("newGuestConnected", Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedUsers.Add(Context.ConnectionId, null);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (UserHandler.ConnectedUsers[Context.ConnectionId] == "Guests")
            {
                Clients.Group("Admins").SendAsync("guestDisConnected", Context.ConnectionId);
            }
            UserHandler.ConnectedUsers.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
