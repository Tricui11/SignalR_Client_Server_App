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
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup.Admins.ToString());
            UserHandler.ConnectedUsers[Context.ConnectionId] = UserGroup.Admins;
        }

        public async Task AddToGuestGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup.Guests.ToString());
            UserHandler.ConnectedUsers[Context.ConnectionId] = UserGroup.Guests;
            await Clients.Group("Admins").SendAsync("newGuestConnected", Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedUsers.Add(Context.ConnectionId, UserGroup.None);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (UserHandler.ConnectedUsers[Context.ConnectionId] == UserGroup.Guests)
            {
                Clients.Group("Admins").SendAsync("guestDisConnected", Context.ConnectionId);
            }
            UserHandler.ConnectedUsers.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
