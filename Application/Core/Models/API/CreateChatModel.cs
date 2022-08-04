namespace Core.Models.API
{
    public class CreateChatModel
    {
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public int ServerId { get; set; }
    }
}