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
		//public T Contents { get; set; }
		public T Contents { get; set; }
	}
	public class FlexBubbleContainerDto
	{
		public string Type { get; set; } = FlexContainerTypeEnum.Bubble;
		public string? Size { get; set; }
		public string? Direction { get; set; }

		public FlexComponentDto? Header { get; set; }
		public FlexComponentDto? Hero { get; set; }
		public FlexComponentDto? Body { get; set; }
		public FlexComponentDto? Footer { get; set; }
		public FlexBubbleContainerStyle? Styles { get; set; }
		public ActionDto? Action { get; set; }
	}
	public class FlexCarouselContainerDto
	{
		public string Type { get; set; } = FlexContainerTypeEnum.Carousel;
		public List<FlexBubbleContainerDto> Contents { get; set; }

	}
	public class FlexComponentDto
	{
		public string Type { get; set; }

		// box component
		public string? Layout { get; set; }
		public List<FlexComponentDto>? Contents { get; set; }
		public string? BackgroundColor { get; set; }
		public string? BorderColor { get; set; }
		public string? BorderWidth { get; set; }
		public string? CornerRadius { get; set; }
		public string? Width { get; set; }
		public string? MaxWidth { get; set; }
		public string? Height { get; set; }
		public string? MaxHeight { get; set; }
		public int? Flex { get; set; }
		public string? Spacing { get; set; }
		public string? Mergin { get; set; }
		public string? PaddingAll { get; set; }
		public string? PaddingTop { get; set; }
		public string? PaddingBottom { get; set; }
		public string? PaddingStart { get; set; }
		public string? PaddingEnd { get; set; }
		public string? Position { get; set; }
		public string? OffsetTop { get; set; }
		public string? OffsetBottom { get; set; }
		public string? OffsetStart { get; set; }
		public string? OffsetEnd { get; set; }
		public ActionDto? Action { get; set; }
		public string? JustifyContent { get; set; }
		public string? AlignItems { get; set; }
		public FlexBackgroundDto? Background { get; set; }

		// button coponent 
		public string? Style { get; set; }
		public string? Color { get; set; }
		public string? Gravity { get; set; }
		public string? AdjustMode { get; set; }

		//image component
		public string? Url { get; set; }
		public string? Align { get; set; }
		public string? Size { get; set; }
		public string? AspectRatio { get; set; }
		public string? AspectMode { get; set; }
		public bool? Animated { get; set; }

		//video component
		public string? PreviewUrl { get; set; }
		public string? AltContent { get; set; }

		//text component
		public string? Text { get; set; }
		public bool? Wrap { get; set; }
		public string? LineSpaceing { get; set; }
		public int? Maxlines { get; set; }
		public string? Weight { get; set; }
		public string? Decoration { get; set; }

		//icon, span, separator, filler 屬性已宣告過
	}
	public class FlexBackgroundDto
	{

		public string? Type { get; set; }
		public string? Angle { get; set; }
		public string? StartColor { get; set; }
		public string? EndColor { get; set; }
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
		public string? BackgroundColor { get; set; }
		public bool? Separator { get; set; }
		public string? SeparatorColor { get; set; }
	}

}
