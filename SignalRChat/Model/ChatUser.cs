namespace SignalRChat.Model
{
    public class ChatUser
    {
        public string ConnectionId { get; set; }
        public UserGroup UserGroup { get; set; }
        public string Name { get; set; }
    }
}
