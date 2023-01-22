namespace LineBotMessage.Dtos.Webhook
{
    public class AiClass
    {
        public string? model { get; set; }
        public string? prompt { get; set; }
        public int? max_tokens { get; set; }
        public double? temperature { get; set; }
    }
}
