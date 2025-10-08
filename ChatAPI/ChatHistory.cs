namespace ChatAPI
{
    public class ChatHistory
    {
        public List<ChatMessage> Messages { get; set; }

        public ChatHistory()
        {
            Messages = new List<ChatMessage>();
        }
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }
        public List<ChatMessage> GetMessagesAfter(DateTime timestamp)
        {
            return Messages.Where(m => m.Timestamp > timestamp).ToList();
        }
        public List<ChatMessage> GetLast(int count)
        {
            return Messages.TakeLast<ChatMessage>(count).ToList();
        }
    }
}
