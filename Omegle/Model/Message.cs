namespace Omegle.Model
{
    public class Message
    {
        public string ChatID { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsImage { get; set; } = false;
        public string? ImageData { get; set; } = default!;
    }
}
