namespace LineBotMessage.Dtos.Webhook
{
    public class AiClass
    {
        public string? model { get; set; }
        public string? prompt { get; set; }
        public int? max_tokens { get; set; }
        public double? temperature { get; set; }
        public double? top_p { get; set; }
        public string? stop { get; set; }
        public double? frequency_penalty { get; set; }
        public double? presence_penalty { get; set; }

    }
}
