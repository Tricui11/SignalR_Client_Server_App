﻿using System.Collections.Generic;

namespace SignalRChat.Hubs
{
    public static class UserHandler
    {
        // ConnectionId, groupName
        public static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();
    }
}
