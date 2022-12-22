using LineBotMessage.Enum;
namespace LineBotMessage.Dtos.Messages
{
    public class LocationMesssageDto:BaseMessageDto
    {
        public LocationMesssageDto()
        {
            Type = MessageTypeEnum.Location;
        }

        public string Title { get; set; }
        public string Address { get; set; }

        public double Latitude { get; set; } // 緯度
        public double Longitude { get; set; } // 經度
    }
}
