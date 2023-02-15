namespace LineBotMessage.Dtos.Messages
{
	// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
	public class Body
	{
		public string backgroundColor { get; set; }
		public string type { get; set; }
		public string layout { get; set; }
		public List<Content> contents { get; set; }
	}

	public class Content
	{
		public string type { get; set; }
		public string text { get; set; }
	}

	public class Footer
	{
		public string backgroundColor { get; set; }
		public string type { get; set; }
		public string layout { get; set; }
		public List<Content> contents { get; set; }
	}

	public class Header
	{
		public string backgroundColor { get; set; }
		public string type { get; set; }
		public string layout { get; set; }
		public List<Content> contents { get; set; }
	}

	public class Hero
	{
		public string type { get; set; }
		public string url { get; set; }
		public string size { get; set; }
		public string aspectRatio { get; set; }
	}

	public class RootFlexMessageDto
	{
		public string type { get; set; }
		public Styles styles { get; set; }
		public Header header { get; set; }
		public Hero hero { get; set; }
		public Body body { get; set; }
		public Footer footer { get; set; }
	}

	public class Styles
	{
		public Header header { get; set; }
		public Body body { get; set; }
		public Footer footer { get; set; }
	}
}
