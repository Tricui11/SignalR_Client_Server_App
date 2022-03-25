using SignalRChat.Model;
using System.Collections.Generic;
using System.Linq;

namespace SignalRChat.Hubs
{
    public static class UserHandler
    {
        public static ICollection<ChatUser> ConnectedUsers = new List<ChatUser>();
        public static ChatUser FindByConnectionId(string connectionId) => UserHandler.ConnectedUsers.First(x => x.ConnectionId == connectionId);
        public static bool CanAddUserName(string userName)
        {
            IEnumerable<string> connectedUsersNames = UserHandler.ConnectedUsers.Select(x => x.Name);
            if (connectedUsersNames.Contains(userName))
            {
                return false;
            }
            return true;
        }
    }
}
