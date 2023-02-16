using LineBotMessage.Enum;

namespace LineBotMessage.Dtos.Messages
{
	public class FlexMessageDto<T> : BaseMessageDto
	{
		public FlexMessageDto()
		{
			Type = MessageTypeEnum.Flex;
		}

		public string AltText { get; set; }
		public T contents { get; set; }

	}
	public class FlexBubbleContainerDto
	{
		public string type { get; set; } = FlexContainerTypeEnum.Bubble;
		public string? size { get; set; }
		public string? direction { get; set; }

		public FlexComponentDto? header { get; set; }
		public FlexComponentDto? hero { get; set; }
		public FlexComponentDto? body { get; set; }
		public FlexComponentDto? footer { get; set; }
		public FlexBubbleContainerStyle? styles { get; set; }
		public ActionDto? action { get; set; }
	}
	//public class FlexCarouselContainerDto
	//{
	//	public string Type { get; set; } = FlexContainerTypeEnum.Carousel;
	//	public List<FlexBubbleContainerDto> Contents { get; set; }

	//}
	public class FlexComponentDto
	{
		public string type { get; set; }

		// box component
		public string? layout { get; set; }
		public List<FlexComponentDto>? contents { get; set; }
		public string? backgroundColor { get; set; }
		public string? borderColor { get; set; }
		public string? borderWidth { get; set; }
		public string? CornerRadius { get; set; }
		public string? Width { get; set; }
		public string? MaxWidth { get; set; }
		public string? Height { get; set; }
		public string? MaxHeight { get; set; }
		public int? flex { get; set; }
		public string? spacing { get; set; }
		public string? Mergin { get; set; }
		public string? paddingAll { get; set; }
		public string? paddingTop { get; set; }
		public string? PaddingBottom { get; set; }
		public string? PaddingStart { get; set; }
		public string? PaddingEnd { get; set; }
		public string? position { get; set; }
		public string? OffsetTop { get; set; }
		public string? OffsetBottom { get; set; }
		public string? OffsetStart { get; set; }
		public string? OffsetEnd { get; set; }
		public ActionDto? Action { get; set; }
		public string? JustifyContent { get; set; }
		public string? AlignItems { get; set; }
		public FlexBackgroundDto? background { get; set; }

		// button coponent 
		public string? style { get; set; }
		public string? color { get; set; }
		public string? gravity { get; set; }
		public string? adjustMode { get; set; }

		//image component
		public string? url { get; set; }
		public string? align { get; set; }
		public string? size { get; set; }
		public string? AspectRatio { get; set; }
		public string? AspectMode { get; set; }
		public bool? Animated { get; set; }

		//video component
		public string? PreviewUrl { get; set; }
		public string? AltContent { get; set; }

		//text component
		public string? text { get; set; }
		public bool? Wrap { get; set; }
		public string? LineSpaceing { get; set; }
		public int? Maxlines { get; set; }
		public string? weight { get; set; }
		public string? Decoration { get; set; }

		public string? margin { get; set; }
		//icon, span, separator, filler 屬性已宣告過
	}
	public class FlexBackgroundDto
	{
		public string? type { get; set; }
		public string? angle { get; set; }
		public string? startColor { get; set; }
		public string? endColor { get; set; }
		public string? CenterColor { get; set; }
		public string? CenterPosition { get; set; }
	}

	// bubble container styles
	public class FlexBubbleContainerStyle
	{
		public FlexBlockStyle? Header { get; set; }
		public FlexBlockStyle? Hero { get; set; }
		public FlexBlockStyle? Body { get; set; }
		public FlexBlockStyle? Footer { get; set; }
	}

	public class FlexBlockStyle
	{
		public string? backgroundColor { get; set; }
		public bool? separator { get; set; }
		public string? separatorColor { get; set; }
	}

}
