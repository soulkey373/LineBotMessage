using System.Net.Http.Headers;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            foreach (WebhookEventDto eventObject in requestBody.Events)
            {
                if (!String.IsNullOrEmpty(eventObject.Message.Text))
                {
                    if (eventObject.Message.Text == "天氣")
                    {
                        String Weather_Conditions = await GetWeather("新竹市");
                        Task.WaitAll();
                        var replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto(){Text = Weather_Conditions}
                            }
                        };
                        ReplyMessageHandler("text", replyMessage);

                    }
                }

            }
        }


        /// <summary>
        /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
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
            }
            catch(Exception ex)
            {
                Console.WriteLine("回復訊息失敗!\n"+ ex.ToString());
            }           

        }
        static async Task<string> GetWeather(string location)
        {
            try
            {
                string WeatherCode = "CWB-99A47F28-FFB9-467F-B4E9-6972DDCF3CD6";
                string result="";
                string Path = $"https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-C0032-001?Authorization={WeatherCode}" + "&" + "locationName=" + $"{location}";
                using HttpResponseMessage response = await client.GetAsync(Path);
                Task.WaitAll();
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
                    Console.WriteLine("抓取天氣成功!");
                   
                }
                else if(response.IsSuccessStatusCode == false)
                {
                    Console.WriteLine("response.IsSuccessStatusCode==False");
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

    }
}

