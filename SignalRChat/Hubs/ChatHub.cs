﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Model;

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

        public async Task AddToAdminGroup(string adminName)
        {
            await AddToGroupAsync(adminName, UserGroup.Admins);
            await Clients.All.SendAsync("ReceiveMessage", $"{adminName} присоеденился к чату.", "Бот");
        }

        public async Task AddToGuestGroup(string guestName)
        {
            await AddToGroupAsync(guestName, UserGroup.Guests);
            await Clients.All.SendAsync("ReceiveMessage", $"{guestName} присоеденился к чату.", "Бот");
            await Clients.Group(UserGroup.Admins.ToString()).SendAsync("newGuestConnected", Context.ConnectionId, guestName);
        }

        private async Task AddToGroupAsync(string userName, UserGroup userGroup)
        {
            var chatUser = UserHandler.FindByConnectionId(Context.ConnectionId);
            chatUser.UserGroup = userGroup;
            chatUser.Name = userName;
            await Groups.AddToGroupAsync(chatUser.ConnectionId, chatUser.UserGroup.ToString());
        }

        public override Task OnConnectedAsync()
        {
            var newChatUser = new ChatUser() { ConnectionId = Context.ConnectionId , UserGroup = UserGroup.None};
            UserHandler.ConnectedUsers.Add(newChatUser);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var chatUser = UserHandler.FindByConnectionId(Context.ConnectionId);
            if (chatUser.UserGroup == UserGroup.Guests)
            {
                Clients.Group(UserGroup.Admins.ToString()).SendAsync("guestDisConnected", Context.ConnectionId);
            }
            Clients.All.SendAsync("ReceiveMessage", $"{chatUser.Name} покинул чат.", "Бот");
            UserHandler.ConnectedUsers.Remove(chatUser);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
