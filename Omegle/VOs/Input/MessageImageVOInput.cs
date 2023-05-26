namespace Omegle.VOs.Input
{
    public class MessageImageVOInput
    {
        public string ChatId { get; set; } = default!;
        public string ConnectionID { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? ImageData { get; set; }
    }
}
