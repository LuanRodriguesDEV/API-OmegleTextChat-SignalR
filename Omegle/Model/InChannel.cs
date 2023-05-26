namespace Omegle.Model
{
    public class InChannel
    {
        public string ChatID { get; set; } = default!;
        public List<User> Users { get; set; } = default!;
        public List<Message> Messages { get; set; } = default!;
    }
}
