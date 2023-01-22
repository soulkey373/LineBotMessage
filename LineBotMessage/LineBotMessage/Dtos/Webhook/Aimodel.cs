namespace LineBotMessage.Dtos.Webhook
{
    public class Aimodel
    {
        public string userid { get; set; }
        public string? prompt { get; set; }
        public string? isContinue { get; set; }
        public DateTime? createtime { get; set; }
        public DateTime? ongoingtime { get; set; }
    }
}
