namespace Core.Models.DTO
{
    public class ChatDto
    {
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public ServerDto Server { get; set; }
    }
}