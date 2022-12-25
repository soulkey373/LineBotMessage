using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using LineBotMessage.Dtos.Messages;
using Npgsql;
using System;
using static LineBotMessage.Dtos.BaseMessageDto;
using NpgsqlTypes;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace LineBotMessage.Domain
{
    public class LineBotService
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "MRCN4reN9kDFdAEfp3DqyGp44Y0i2dWWazOHrcD3HqYtJWw5tlQ9iYEMvfKVZp7bIAtDAqjM0tZeYz226ubO0FotH6ajjfXmOaRkZPD4YF0/TA8sVqVy/jKAjXuBdzdsGt4Yz510nIXssnOaJK00cgdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "c9a0ee25cc2d021320c0765b0dbb2cc6";

        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";


        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public LineBotService() { }

        public async void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        {
            string strBody = requestBody.ToString();
            dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        if (eventObject.Message.Type == MessageTypeEnum.Text)
                            getPostgresDate();
                        await ReceiveMessageWebhookEvent(eventObject);
                        break;
                    case WebhookEventTypeEnum.Unsend:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Console.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Console.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.MemberJoined:
                        string joinedMemberIds = "";
                        foreach (var member in eventObject.Joined.Members)
                        {
                            joinedMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{joinedMemberIds}加入了群組！");
                        break;
                    case WebhookEventTypeEnum.MemberLeft:
                        string leftMemberIds = "";
                        foreach (var member in eventObject.Left.Members)
                        {
                            leftMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{leftMemberIds}離開了群組！");
                        break;
                    case WebhookEventTypeEnum.Postback:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件");
                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}");
                        break;
                    case MessageTypeEnum.Location:
                        messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<LocationMesssageDto>>(strBody);
                        break;
                }
            }
        }

        private async Task ReceiveMessageWebhookEvent(WebhookEventDto eventObject)
        {
            #region 一般天氣文字回復
            //ReplyMessageRequestDto<TextMessageDto> replyMessage = new ReplyMessageRequestDto<TextMessageDto>();
            //replyMessage.ReplyToken = eventObject.ReplyToken;
            //replyMessage.Messages = new List<TextMessageDto>();
            //TextMessageDto textMessage = new TextMessageDto();
            #endregion

            #region Carousel回復
            //ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>> replyMessage1 = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>();
            //replyMessage1.ReplyToken = eventObject.ReplyToken;
            //replyMessage1.Messages = new List<TemplateMessageDto<CarouselTemplateDto>>();
            //TemplateMessageDto<CarouselTemplateDto> templateMessage = new TemplateMessageDto<CarouselTemplateDto>();
            #endregion

            #region Quick Reply 快速回復
            dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();
            #endregion
            if (eventObject.Message.Text != "" && eventObject.Message.Text != null)
            {
                #region 當使用者鍵入"天氣時"
                //if (eventObject.Message.Text.Trim() == "天氣")
                //{
                //    string result = await GetWeather();
                //    textMessage.Text = result;
                //}
                #endregion

                #region 當使用者鍵入"天氣"Carousel 型態
                //if (eventObject.Message.Text.Contains("天氣 "))
                //{
                //    string Keyword = eventObject.Message.Text.Substring(3);//天氣空格=第三個字元開始
                //    List<CarouselColumnObjectDto>? result = new List<CarouselColumnObjectDto>();
                //    result = await GetWeatherCarousel(Keyword.Replace('台', '臺'));

                //    Task.WaitAll();
                //    templateMessage.AltText = "新竹市未來 36 小時天氣預測";
                //    templateMessage.Template = new CarouselTemplateDto();
                //    templateMessage.Template.Columns = new List<CarouselColumnObjectDto>
                //                                           {

                //        new CarouselColumnObjectDto
                //        {
                //            ThumbnailImageUrl = "https://obs.line-scdn.net/0huvyHSG7HKllEMTykfhpVDn1nKTZ3XTlaIAd7WgdfdG5oCT0NflE3b2cxdDlvA20HKgBjOGl0dTw8BmlbfwU/w644",
                //            Title = result[0].Title.ToString(),
                //            Text =  result[0].Text.ToString(),
                //            Actions = new List<ActionDto>
                //            {
                //                //按鈕 action
                //                new ActionDto
                //                {
                //                    Type = ActionTypeEnum.Location,
                //                    Label ="詳細天氣資訊",
                //                    Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-"
                //                }
                //            }
                //        },
                //        new CarouselColumnObjectDto
                //        {
                //            ThumbnailImageUrl = "https://truth.bahamut.com.tw/s01/202112/3e2f4ddb0738d88f8f08492f7a7e2c79.JPG",
                //            Title = result[1].Title.ToString(),
                //            Text =  result[1].Text.ToString(),
                //            Actions = new List<ActionDto>
                //            {
                //                //按鈕 action
                //                new ActionDto
                //                {
                //                    Type = ActionTypeEnum.Uri,
                //                    Label ="詳細天氣資訊",
                //                    Uri = "https://www.cwb.gov.tw/V8/C/W/County/index.html"
                //                }
                //            }
                //        },
                //        new CarouselColumnObjectDto
                //        {
                //            ThumbnailImageUrl = "https://pbs.twimg.com/media/FkRM6TOaEAE9CLr?format=jpg&name=large",
                //            Title = result[2].Title.ToString(),
                //            Text =  result[2].Text.ToString(),
                //            Actions = new List<ActionDto>
                //            {
                //                //按鈕 action
                //                new ActionDto
                //                {
                //                    Type = ActionTypeEnum.Uri,
                //                    Label ="詳細天氣資訊",
                //                    Uri = "https://www.cwb.gov.tw/V8/C/W/County/index.html"
                //                }
                //            }
                //        }
                //                                            };

                //}
                #endregion

                #region 當使用者鍵入"測試快速回復"

                if (eventObject.Message.Text.Contains("測試快速回復"))
                {
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                    {
                        ReplyToken = eventObject.ReplyToken,
                        Messages = new List<TextMessageDto>
                        {
                             new TextMessageDto
                             {
                                Text ="QuickReply 測試訊息",
                                QuickReply = new QuickReplyItemDto
                                {
                                     Items = new List<QuickReplyButtonDto>
                                     {
                                         // message action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "message 測試" ,
                                                    Text = "測試"
                                                }
                                            },
                                              // location action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Location,
                                                    Label = "開啟位置"
                                                }
                                            }
                                     }
                                }
                             }
                        }
                    };

                }
                #endregion
                //else
                //{
                //    textMessage.Text = eventObject.Message.Text;
                //}
                //replyMessage.Messages.Add(textMessage);
                //replyMessage1.Messages.Add(templateMessage);
                ReplyMessageHandler(replyMessage);
            }
        }


        /// <summary>
        /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void ReplyMessageHandler<T>(ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        /// <summary>
        /// 將回覆訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {

            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
                string? json = _jsonProvider.Serialize(request);
                HttpRequestMessage? requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(replyMessageUri),
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                HttpResponseMessage response = await client.SendAsync(requestMessage);
                Console.WriteLine($"response.IsSuccessStatusCode = {response.IsSuccessStatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("回復訊息失敗!\n" + ex.ToString());
            }

        }
        static async Task<string> GetWeather()
        {
            try
            {
                string result = "";
                string local = "新竹市";
                string Path = $"https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-C0032-001?format=JSON&locationName={local}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("CWB-99A47F28-FFB9-467F-B4E9-6972DDCF3CD6");
                using HttpResponseMessage response = await client.GetAsync(Path);
                Console.WriteLine(response.ToString());

                if (response.IsSuccessStatusCode == true)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WeatherReturn responseBodyJsonParse = JsonConvert.DeserializeObject<WeatherReturn>(responseBody);
                    var StartTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[1].startTime).ToString("yyyy  MM / dd dddd HH:mm");
                    var EndTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[1].endTime).ToString("yyyy  MM / dd dddd HH:mm");
                    result = "新竹市12小時天氣預報" +
                                                           Environment.NewLine + $"{StartTime}" +
                                                           Environment.NewLine + $"{EndTime}" +
                                                           Environment.NewLine + $"天氣狀態:{responseBodyJsonParse.records.location[0].weatherElement[0].time[0].parameter.parameterName}" +
                                                           Environment.NewLine + $"降雨機率:{responseBodyJsonParse.records.location[0].weatherElement[1].time[0].parameter.parameterName}" + "%" +
                                                           Environment.NewLine + $"最低溫度:{responseBodyJsonParse.records.location[0].weatherElement[2].time[0].parameter.parameterName}" + "°C" +
                                                           Environment.NewLine + $"最高溫度:{responseBodyJsonParse.records.location[0].weatherElement[4].time[0].parameter.parameterName}" + "°C" +
                                                           Environment.NewLine + $"天氣舒適度:{responseBodyJsonParse.records.location[0].weatherElement[3].time[0].parameter.parameterName}";
                    Console.WriteLine("抓取天氣API成功!");

                }
                else if (response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine("抓取天氣API失敗!");
                    return result;
                }
                return result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("抓取天氣失敗!");
                Console.WriteLine("Message :{0} ", e.Message.ToString());
                return "";
            }

        }
        static async Task<List<CarouselColumnObjectDto>> GetWeatherCarousel(string localname)
        {
            try
            {
                List<CarouselColumnObjectDto> carouselList = new List<CarouselColumnObjectDto>();

                string result = "";
                string Path = $"https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-C0032-001?format=JSON&locationName={localname}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("CWB-99A47F28-FFB9-467F-B4E9-6972DDCF3CD6");
                using HttpResponseMessage response = await client.GetAsync(Path);
                //Console.WriteLine(response.ToString());

                if (response.IsSuccessStatusCode == true)
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    WeatherReturn responseBodyJsonParse = JsonConvert.DeserializeObject<WeatherReturn>(responseBody);

                    for (int number = 0; number < 3; number++)
                    {
                        CarouselColumnObjectDto carouselColumnObject = new CarouselColumnObjectDto();
                        string? StartTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[number].startTime).ToString("MM/dd HH:mm");
                        string? EndTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[number].endTime).ToString("MM/dd HH:mm");
                        result = localname + ":" +
                                  Environment.NewLine + $"天氣狀態: {responseBodyJsonParse.records.location[0].weatherElement[0].time[number].parameter.parameterName}" +
                                  Environment.NewLine + $"降雨機率: {responseBodyJsonParse.records.location[0].weatherElement[1].time[number].parameter.parameterName}" + "%" +
                                  Environment.NewLine + $"最低溫度: {responseBodyJsonParse.records.location[0].weatherElement[2].time[number].parameter.parameterName}" + "°C" +
                                  Environment.NewLine + $"最高溫度: {responseBodyJsonParse.records.location[0].weatherElement[4].time[number].parameter.parameterName}" + "°C" +
                                  Environment.NewLine + $"舒適度: {responseBodyJsonParse.records.location[0].weatherElement[3].time[number].parameter.parameterName}";
                        //Console.WriteLine("抓取天氣API成功!");
                        carouselColumnObject.Title = StartTime + "~" + EndTime;
                        carouselColumnObject.Text = result;
                        carouselList.Add(carouselColumnObject);
                    }

                }
                else if (response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine("抓取天氣API失敗!");
                    return null;
                }
                return carouselList;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("抓取天氣失敗!");
                Console.WriteLine("Message :{0} ", e.Message.ToString());
                return null;
            }

        }

        public string getPostgresDate()
        {
            try
            {
                string Host = "soulkeydb.fly.dev";
                string User = "postgres";
                string DBname = "linebotapp";
                string Password = "NeB8A71EYRW4CgU";
                string Port = "5432";
                string connString =
                String.Format(
                   "Host={0};Username={1};Database={2};Port={3};Password={4};SSLMode=disable",
                   Host,
                   User,
                   DBname,
                   Port,
                   Password);

                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    Console.WriteLine("連線db成功!v6");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("連線db失敗!v6");
                Console.WriteLine(e.Message);
                throw new ArgumentException(e.ToString());
            }
            return "";

        }

    }
}


