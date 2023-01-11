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
using System.Drawing;
using System.Data.Common;
using System.Data;
using LineBotMessage.DbConn;
using Microsoft.Build.Tasks;
using System.Collections.Generic;

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
            foreach (WebhookEventDto eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        try
                        {
                            if (eventObject.Message.Type == MessageTypeEnum.Text)
                            {
                                await ReceiveMessageWebhookEvent(eventObject);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("WebhookEventTypeEnum.Message過程發生錯誤\n{0}", ex.ToString());
                        }
                        break;
                    case WebhookEventTypeEnum.Postback:

                        try
                        {
                            string? userID = eventObject.Source.UserId;
                            string postdata = eventObject.Postback.Data.Trim();
                            string filePath = "/app/data/status.txt";
                            if (File.Exists(filePath))
                            {
                                OrderFoodPhase1(userID, postdata, "10");
                            }
                            Console.WriteLine("OrderFoodPhase1完成!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("儲存meal的過程發生錯誤{0}", ex.ToString());
                        }
                        break;
                }
            }
        }

        private async Task ReceiveMessageWebhookEvent(WebhookEventDto eventObject)
        {
            ReplyMessageRequestDto<TextMessageDto> replyMessage = new ReplyMessageRequestDto<TextMessageDto>();
            replyMessage.ReplyToken = eventObject.ReplyToken;
            replyMessage.Messages = new List<TextMessageDto>();

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
            //dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();
            #endregion

            if (eventObject.Message.Text != "" && eventObject.Message.Text != null)
            {
                string filePath = "/app/data/status.txt";
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

                //if (eventObject.Message.Text.Contains("測試快速回復"))
                //{
                //    replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                //    {
                //        ReplyToken = eventObject.ReplyToken,
                //        Messages = new List<TextMessageDto>
                //        {
                //             new TextMessageDto
                //             {
                //                Text ="QuickReply 測試訊息",
                //                QuickReply = new QuickReplyItemDto
                //                {
                //                     Items = new List<QuickReplyButtonDto>
                //                     {
                //                         // message action
                //                            new QuickReplyButtonDto {
                //                                Action = new ActionDto {
                //                                    Type = ActionTypeEnum.Message,
                //                                    Label = "message 測試" ,
                //                                    Text = "測試"
                //                                }
                //                            },
                //                              // location action
                //                            new QuickReplyButtonDto {
                //                                Action = new ActionDto {
                //                                    Type = ActionTypeEnum.Location,
                //                                    Label = "開啟位置"
                //                                }
                //                            }
                //                     }
                //                }
                //             }
                //        }
                //    };
                //}
                //else
                //{
                //    textMessage.Text = eventObject.Message.Text;
                //}
                //replyMessage.Messages.Add(textMessage);
                //replyMessage1.Messages.Add(templateMessage);
                #endregion

                #region 點餐系統

                if (eventObject.Message.Text.Trim() == "吃什麼")
                {
                    Console.WriteLine("進來吃什麼系統");

                    if (File.Exists(filePath))
                    {
                        //讀取第一行
                        string firstLine = File.ReadLines(filePath).First();
                        //建檔時間
                        DateTime time = DateTime.Parse(firstLine);
                        //設定2分鐘區間
                        TimeSpan interval = TimeSpan.FromMinutes(1);
                        DateTime now = DateTime.Now;
                        TimeSpan diff = now.Subtract(time);
                        //如果當前時間跟文本時間相比，是超過設定的2分鐘，則回傳大於一的整數。
                        if (diff.CompareTo(interval) > 0)
                        {
                            Console.WriteLine("前一次啟用系統超過1分鐘，將刪除紀錄");
                            File.Delete(filePath);
                            UserRecordInformationDapper userRecord = new UserRecordInformationDapper();
                            userRecord.Delete();
                            DateTime now1 = DateTime.Now;
                            File.WriteAllText(filePath, now1.ToString());
                            Console.WriteLine($"以建立點餐紀錄\n{firstLine}");
                            ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>? replyMessage1 = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
                            {
                                ReplyToken = eventObject.ReplyToken,
                                Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                                {
                                    new TemplateMessageDto<ButtonsTemplateDto>
                                    {
                                        AltText = "這是點餐系統👋",
                                        Template = new ButtonsTemplateDto
                                        {
                                        ThumbnailImageUrl = "https://pbs.twimg.com/media/E3RzkQkUcAETIMA?format=jpg&name=large",
                                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
                                        ImageSize = TemplateImageSizeEnum.Cover,
                                        Title = "歡迎使用本點餐系統!",
                                         Text = "請選擇您想要的餐點種類:",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "早餐",
                                                Label = "早餐🍳",
                                                DisplayText = "早餐"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "午餐",
                                                Label = "午餐🍱",
                                                DisplayText = "午餐"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "晚餐",
                                                Label = "晚餐 🍽️",
                                                DisplayText = "晚餐"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "夜消",
                                                Label = "夜消🍪",
                                                DisplayText = "夜消"
                                            }
                                        }
                                        }
                                    }
                                }
                            };
                            ReplyMessage(replyMessage1);
                        }
                        else
                        {
                            Console.WriteLine("未超過1分鐘");
                            ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
                            {
                                ReplyToken = eventObject.ReplyToken,
                                Messages = new List<TextMessageDto>
                             {
                                new TextMessageDto(){Text = "距離上一個點餐系統未超過一分鐘，請稍後再啟用系統"}
                             }
                            };
                            ReplyMessage(replyMessage1);
                        }
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        File.WriteAllText(filePath, now.ToString());
                        string firstLine = File.ReadLines(filePath).First();
                        Console.WriteLine($"以建立點餐紀錄\n{firstLine}");

                        ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>? replyMessage1 = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                                {
                                    new TemplateMessageDto<ButtonsTemplateDto>
                                    {
                                        AltText = "這是點餐系統👋",
                                        Template = new ButtonsTemplateDto
                                        {
                                        ThumbnailImageUrl = "https://pbs.twimg.com/media/E3RzkQkUcAETIMA?format=jpg&name=large",
                                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
                                        ImageSize = TemplateImageSizeEnum.Cover,
                                        Title = "歡迎使用本點餐系統!",
                                         Text = "請選擇您想要的餐點種類:",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "早餐",
                                                Label = "早餐",
                                                DisplayText = "早餐🍳"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "午餐",
                                                Label = "午餐🍱",
                                                DisplayText = "午餐"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "晚餐",
                                                Label = "晚餐 ️🍽",
                                                DisplayText = "晚餐 ️"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "消夜",
                                                Label = "夜消🍪",
                                                DisplayText = "夜消"
                                            }
                                        }
                                        }
                                    }
                                }
                        };
                        ReplyMessage(replyMessage1);
                    }
                    return;
                }

                if (File.Exists(filePath))
                {
                    try
                    {
                        string? userid = eventObject.Source.UserId;
                        UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
                        IList<UserRecord>? result = informationDapper.Load();
                        Console.WriteLine("目前step:{0}",result[0].step);
                        if (eventObject.Source.UserId == result[0].id)
                        {
                            string foodtype = eventObject.Message.Text.Trim();
                            string mealtype = result[0].mealtype;
                            OrderFoodPhase2(userid, mealtype,foodtype, "20");
                            Console.WriteLine("OrderFoodPhase2完成!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                }
                #endregion

            }
        }

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

        public void OrderFoodPhase1(string userID, string mealtype, string step)
        {
            Console.WriteLine("進到OrderFoodPhase1");
            UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
            UserRecord record = new UserRecord();
            record.id = userID;
            record.mealtype = mealtype;
            record.step = step;
            informationDapper.Create(record);
        }
        public void OrderFoodPhase2(string userID,string mealtype, string foodtype, string step)
        {
            UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
            UserRecord record = new UserRecord();
            record.id = userID;
            record.mealtype = mealtype;
            record.foodtype = foodtype;
            record.step = step;
            informationDapper.Update(record);
        }

        #region 文字天氣
        //static async Task<string> GetWeather()
        //{
        //    try
        //    {
        //        string result = "";
        //        string local = "新竹市";
        //        string Path = $"https://opendata.cwb.gov.tw/api/v1/rest/datastore/F-C0032-001?format=JSON&locationName={local}";
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("CWB-99A47F28-FFB9-467F-B4E9-6972DDCF3CD6");
        //        using HttpResponseMessage response = await client.GetAsync(Path);
        //        Console.WriteLine(response.ToString());

        //        if (response.IsSuccessStatusCode == true)
        //        {
        //            response.EnsureSuccessStatusCode();
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            WeatherReturn responseBodyJsonParse = JsonConvert.DeserializeObject<WeatherReturn>(responseBody);
        //            var StartTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[1].startTime).ToString("yyyy  MM / dd dddd HH:mm");
        //            var EndTime = Convert.ToDateTime(responseBodyJsonParse.records.location[0].weatherElement[0].time[1].endTime).ToString("yyyy  MM / dd dddd HH:mm");
        //            result = "新竹市12小時天氣預報" +
        //                                                   Environment.NewLine + $"{StartTime}" +
        //                                                   Environment.NewLine + $"{EndTime}" +
        //                                                   Environment.NewLine + $"天氣狀態:{responseBodyJsonParse.records.location[0].weatherElement[0].time[0].parameter.parameterName}" +
        //                                                   Environment.NewLine + $"降雨機率:{responseBodyJsonParse.records.location[0].weatherElement[1].time[0].parameter.parameterName}" + "%" +
        //                                                   Environment.NewLine + $"最低溫度:{responseBodyJsonParse.records.location[0].weatherElement[2].time[0].parameter.parameterName}" + "°C" +
        //                                                   Environment.NewLine + $"最高溫度:{responseBodyJsonParse.records.location[0].weatherElement[4].time[0].parameter.parameterName}" + "°C" +
        //                                                   Environment.NewLine + $"天氣舒適度:{responseBodyJsonParse.records.location[0].weatherElement[3].time[0].parameter.parameterName}";
        //            Console.WriteLine("抓取天氣API成功!");

        //        }
        //        else if (response.IsSuccessStatusCode == false)
        //        {
        //            Console.WriteLine("抓取天氣API失敗!");
        //            return result;
        //        }
        //        return result;
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine("抓取天氣失敗!");
        //        Console.WriteLine("Message :{0} ", e.Message.ToString());
        //        return "";
        //    }

        //}
        #endregion

        #region Carousel天氣
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
        #endregion

        #region Dapper範例參考_orderfood
        //public void OrderFood(string text)
        //{
        //    string connString = "Host=soulkeydb.internal;Port=5432;Username=postgres;Password=xwOCnnjArOaAnBZ;Database=runoobdb";
        //    //UserRecordInformation userRecord = new UserRecordInformation();
        //    UserRecordInformationDapper userRecord1 = new UserRecordInformationDapper();
        //    int day = (int)Convert.ToUInt32(text);
        //    switch (day)
        //    {
        //        case 1:
        //            DateTime time = DateTime.Now;
        //            UserRecord userRecord = new UserRecord();
        //            userRecord.id = 930030;
        //            userRecord.mealtype = "早餐";
        //            userRecord.foodtype = "滿福堡";
        //            userRecord.lon = "122";
        //            userRecord.lat = "76";
        //            userRecord.time = time;
        //            userRecord.step = "Q1";
        //            var reusult = userRecord1.Create(connString, userRecord);
        //            Console.WriteLine("---------Dapper測試:Create模式---------");
        //            Console.WriteLine("Create:{0}", reusult ? "成功" : "失敗");
        //            break;
        //        case 2:
        //            int bocci = 930030;
        //            var reusult2 = userRecord1.Load(connString, bocci);
        //            Console.WriteLine("---------Dapper測試:Load模式---------");
        //            Console.WriteLine($"Load \n{reusult2[0].id}\n{reusult2[0].mealtype}\n{reusult2[0].foodtype}\n{reusult2[0].lat}\n{reusult2[0].lon}\n{reusult2[0].step}\n{reusult2[0].time}");

        //            break;
        //        case 3:
        //            DateTime time2 = DateTime.Now;
        //            UserRecord userRecord4 = new UserRecord();
        //            userRecord4.id = 930030;
        //            userRecord4.mealtype = "午餐";
        //            userRecord4.foodtype = "雞腿便當";
        //            userRecord4.lon = "122";
        //            userRecord4.lat = "76";
        //            userRecord4.time = time2;
        //            userRecord4.step = "Q1";
        //            var reusult3 = userRecord1.Update(connString, userRecord4);
        //            var arg2 = reusult3 ? "成功" : "失敗";
        //            Console.WriteLine("---------Dapper測試:Load模式---------");
        //            Console.WriteLine($"更新:{arg2}");
        //            break;
        //        case 4:
        //            int bocc2 = 930030;
        //            var reusult5 = userRecord1.Delete(connString, bocc2);
        //            var arg = reusult5 ? "成功" : "失敗";
        //            Console.WriteLine("---------Dapper測試:Load模式---------");
        //            Console.WriteLine($"刪除:{arg}");
        //            break;
        //        case 5:
        //            string time1 = DateTime.Now.ToString();
        //            string filePath = "/app/data/status.txt";
        //            if (File.Exists(filePath))
        //            {
        //                //讀取第一行
        //                string firstLine = File.ReadLines(filePath).First();
        //                //建檔時間
        //                DateTime time3 = DateTime.Parse(firstLine);
        //                //設定2分鐘區間
        //                TimeSpan interval = TimeSpan.FromMinutes(2);
        //                DateTime now = DateTime.Now;
        //                TimeSpan diff = now.Subtract(time3);
        //                //如果當前時間跟文本時間相比，是超過設定的2分鐘，則回傳大於一的整數。
        //                if (diff.CompareTo(interval) > 0)
        //                {
        //                    Console.WriteLine("超過2分鐘，將會刪除");
        //                    File.Delete(filePath);
        //                }
        //                else
        //                {
        //                    Console.WriteLine("未超過2分鐘");
        //                }
        //            }
        //            else
        //            {
        //                File.WriteAllText(filePath, time1);
        //                string firstLine = File.ReadLines(filePath).First();
        //                Console.WriteLine($"以建立紀錄\n{firstLine}");
        //            }

        //            break;
        //    }
        //}
        #endregion

    }
}


