namespace LineBotMessage.Dtos
{
    public class ReplyMessageRequestDto<T>
    {
        public string ReplyToken { get; set; }
		//public List<T> Messages { get; set; }
		public List<T> Messages { get; set; }
		public bool? NotificationDisabled { get; set; }
    }

	public class ReplyMessageRequestDto2<T>
	{
		public string ReplyToken { get; set; }
		//public List<T> Messages { get; set; }
		public string Messages { get; set; }
		public bool? NotificationDisabled { get; set; }
	}
}

