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
using System.Net.Http;
using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Security.Policy;
using System.Reflection.Emit;
using static System.Collections.Specialized.BitVector32;
using LineBotMessage.Dtos.Webhook;
using System.Text.RegularExpressions;
using System.Formats.Asn1;
using FlickrNet;
using Microsoft.VisualBasic;
using System.Reflection;
using System.Linq;

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
							if (eventObject.Message.Type == MessageTypeEnum.Location)
							{
								if (JudgeExsitLog())
								{
									UserRecordInformationDapper userRecord = new UserRecordInformationDapper();
									var result = userRecord.Load();
									UserRecord? xuserRecord = result[0];
									string lat = Convert.ToString(eventObject.Message.Latitude);
									string lon = Convert.ToString(eventObject.Message.Longitude);
									OrderFoodPhase3(xuserRecord.id, xuserRecord.mealtype, xuserRecord.foodtype, lat, lon, "30");
									Console.WriteLine("OrderFoodPhase3完成");
									ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>> replyMessage1 = new ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>>();
									replyMessage1.ReplyToken = eventObject.ReplyToken;
									replyMessage1.Messages = new List<TemplateMessageDto<ImageCarouselTemplateDto>>();
									TemplateMessageDto<ImageCarouselTemplateDto> templateMessage = new TemplateMessageDto<ImageCarouselTemplateDto>
									{
										AltText = "請選您要吃的價位~",
										Template = new ImageCarouselTemplateDto
										{
											Columns = new List<ImageCarouselColumnObjectDto>
											{
												 new ImageCarouselColumnObjectDto
												 {
													  ImageUrl="https://3.bp.blogspot.com/-EmZVRJnXsGA/V-yCZuwAUjI/AAAAAAAA-KE/VDeCip5CT3EzOIB7TDA8AIjuhrTmBbL8QCLcB/s800/binbou_man.png",
													  Action = new ActionDto
													  {
														   Type = ActionTypeEnum.Postback,
														   Label = "我現在是窮光蛋",
														   Data="low"
													  }
												 },
												 new ImageCarouselColumnObjectDto
												 {
													  ImageUrl="https://1.bp.blogspot.com/-XeLDe3ylSIY/XWS5pVa7TjI/AAAAAAABUVI/VpLH_IIPkA8PaiGVCRr7sYOnJmuIp-2qQCLcBGAs/s1600/kakedasu_suit1.png",
													  Action = new ActionDto
													  {
														   Type = ActionTypeEnum.Postback,
														   Label = "才月中，安全牛啦",
														   Data="mid"
													  }
												 },
												 new ImageCarouselColumnObjectDto
												 {
													  ImageUrl="https://1.bp.blogspot.com/-Ln1T9C2aorc/WJmUlZXld2I/AAAAAAABBhI/1jGLOzMNIOYsoBjf652x3HNfHBOLZz0pQCLcB/s800/kyuryou_bonus_man2.png",
													  Action = new ActionDto
													  {
														   Type = ActionTypeEnum.Postback,
														   Label = "我剛領薪水啦",
														   Data="high"
													  }
												 }
											}
										}
									};

									replyMessage1.Messages.Add(templateMessage);
									ReplyMessage(replyMessage1);
								}
								else
								{
									//ReplyMessageRequestDto<TextMessageDto> replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>();
									//replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>
									//{
									//    ReplyToken = eventObject.ReplyToken,
									//    Messages = new List<TextMessageDto>
									//    {
									//        new TextMessageDto
									//        {
									//            Text="距離上次呼叫已超過二分鐘，\n請重新鍵入-吃什麼-\n以便請用系統"
									//        }
									//     }
									//};
									//ReplyMessage(replyMessage1);
								}

							}
						}
						catch (Exception ex)
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
							if (postdata == "早餐" || postdata == "午餐" || postdata == "晚餐" || postdata == "消夜")
							{

								//Console.WriteLine($"進到postback裡輸入值為:{postdata}");
								if (JudgeExsitLog())
								{
									OrderFoodPhase1(userID, postdata, "10");
									//Console.WriteLine("OrderFoodPhase1完成!");
									ReplyMessageRequestDto<TextMessageDto> replyMessage = new ReplyMessageRequestDto<TextMessageDto>();
									replyMessage.ReplyToken = eventObject.ReplyToken;
									replyMessage.Messages = new List<TextMessageDto>();
									TextMessageDto textMessage = new TextMessageDto();
									textMessage.Text = "請輸入您想要的食物: 🤔\ne.g. 牛排🥩, 拉麵 🍜";
									replyMessage.Messages.Add(textMessage);
									ReplyMessage(replyMessage);
								}
							}
							else if (postdata == "low" || postdata == "mid" || postdata == "high")
							{
								UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
								UserRecord? result = informationDapper.Load().First();
								if (JudgeExsitLog())
								{

									OrderFoodPhase4(userID, result.mealtype, result.foodtype, result.lat, result.lon, "40", postdata);
									OrderFoodPhase5(eventObject.ReplyToken);
								}
								else if (result.step == "99")
								{
									//donothing;
									Console.WriteLine("step=99");
									break;
								}

							}


						}
						catch (Exception ex)
						{
							Console.WriteLine("儲存meal的過程發生錯誤{0}", ex.ToString());
						}
						break;

				}
			}
		}
		public async Task<bool> Recordlinemsg(string GroupID, string UserID, string msg)
		{
			string token = "";
			string Path = $"https://api.line.me/v2/bot/group/{GroupID}/member/{UserID}";
			HttpClient client = new HttpClient() { BaseAddress = new Uri(Path) };
			client.DefaultRequestHeaders.Add("authorization", "Bearer MRCN4reN9kDFdAEfp3DqyGp44Y0i2dWWazOHrcD3HqYtJWw5tlQ9iYEMvfKVZp7bIAtDAqjM0tZeYz226ubO0FotH6ajjfXmOaRkZPD4YF0/TA8sVqVy/jKAjXuBdzdsGt4Yz510nIXssnOaJK00cgdB04t89/1O/w1cDnyilFU=");
			HttpResponseMessage response = await client.GetAsync(Path);
			LineProfile? res = JsonConvert.DeserializeObject<LineProfile>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			try
			{
				//Console.WriteLine($"呼叫api_lineprofile成功，名子為:{res.displayName}");
				filetxtrecord(GroupID, res, msg);
				return true;

			}
			catch (Exception ex)
			{
				//Console.WriteLine($"呼叫api_lineprofile失敗，錯誤代碼:{ex.Message}");
				return false;
			}
		}
		public void filetxtrecord(string groupid, LineProfile lineProfile, string msg)
		{
			string path = $"/app/data/{groupid}";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			DateTime currentTime = DateTime.Now.AddHours(8);
			string formattedTime = currentTime.ToString("yyyyMMdd HH:mm");
			string txtNameTime = currentTime.ToString("yyyyMMdd");
			string content = "[" + formattedTime + "]" + $"{lineProfile.displayName}:" + msg;
			string filePath = $"/app/data/{groupid}/{txtNameTime}.txt";
			if (File.Exists(filePath))
			{
				content = "\n" + content;
				File.AppendAllText(filePath, content);
			}
			else
			{
				File.AppendAllText(filePath, content);
			}

		}
		private async Task ReceiveMessageWebhookEvent(WebhookEventDto eventObject)
		{
			//Console.WriteLine($"GroupID:{eventObject.Source.GroupId}");
			//Console.WriteLine($"UserID:{eventObject.Source.UserId}");

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

				#region 點餐系統
				if (eventObject.Message.Text.Trim().Contains("!停止") || (eventObject.Message.Text.Trim().Contains("！停止")))
				{
					stop();
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
							 {
								new TextMessageDto(){Text = "系統已停止"}
							 }
					};
					ReplyMessage(replyMessage1);
				}
				if (eventObject.Message.Text.Trim().Contains("!吃什麼") || eventObject.Message.Text.Trim().Contains("！吃什麼"))
				{
					//Console.WriteLine("進來吃什麼系統");

					if (File.Exists(filePath))
					{
						//讀取第一行
						string firstLine = File.ReadLines(filePath).First();
						//建檔時間
						DateTime time = DateTime.Parse(firstLine);
						//設定2分鐘區間
						TimeSpan interval = TimeSpan.FromMinutes(2);
						DateTime now = DateTime.Now;
						TimeSpan diff = now.Subtract(time);
						//如果當前時間跟文本時間相比，是超過設定的2分鐘，則回傳大於一的整數。
						if (diff.CompareTo(interval) > 0)
						{
							//Console.WriteLine("前一次啟用系統超過2分鐘，將刪除紀錄");
							File.Delete(filePath);
							UserRecordInformationDapper userRecord = new UserRecordInformationDapper();
							userRecord.Delete();
							DateTime now1 = DateTime.Now;
							File.WriteAllText(filePath, now1.ToString());
							//Console.WriteLine($"以建立點餐紀錄\n{firstLine}");
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
												DisplayText = "早餐"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "午餐",
												Label = "午餐",
												DisplayText = "午餐"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "晚餐",
												Label = "晚餐",
												DisplayText = "晚餐"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "消夜",
												Label = "消夜",
												DisplayText = "消夜"
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
							//Console.WriteLine("未超過2分鐘");
							ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
							{
								ReplyToken = eventObject.ReplyToken,
								Messages = new List<TextMessageDto>
							 {
								new TextMessageDto(){Text = "距離上一個點餐系統未超過二分鐘，請稍後再啟用系統"}
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
						//Console.WriteLine($"以建立點餐紀錄\n{firstLine}");

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
												DisplayText = "早餐"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "午餐",
												Label = "午餐",
												DisplayText = "午餐"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "晚餐",
												Label = "晚餐",
												DisplayText = "晚餐 ️"
											},
											new ActionDto
											{
												Type = ActionTypeEnum.Postback,
												Data = "消夜",
												Label = "消夜",
												DisplayText = "消夜"
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
						//Console.WriteLine("目前step:{0}", result[0].step);
						if (eventObject.Source.UserId == result[0].id)
						{
							string foodtype = eventObject.Message.Text.Trim();
							string mealtype = result[0].mealtype;
							if (JudgeExsitLog())
							{
								OrderFoodPhase2(userid, mealtype, foodtype, "20");
								Console.WriteLine("OrderFoodPhase2完成!");
								replyMessage = new ReplyMessageRequestDto<TextMessageDto>
								{
									ReplyToken = eventObject.ReplyToken,
									Messages = new List<TextMessageDto>
									{
										 new TextMessageDto
										 {
											Text ="請點擊下方的按鈕，打開地圖並輸入搜尋位置 📍",
											QuickReply = new QuickReplyItemDto
											{
												 Items = new List<QuickReplyButtonDto>
												 {
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

						}
						else
						{
							replyMessage = new ReplyMessageRequestDto<TextMessageDto>
							{
								ReplyToken = eventObject.ReplyToken,
								Messages = new List<TextMessageDto>
								{
									new TextMessageDto
									{
										Text="距離上次呼叫已超過二分鐘，\n請重新鍵入-吃什麼-\n以便請用系統"
									}
								}

							};
						}
						ReplyMessage(replyMessage);
					}
					catch (Exception ex)
					{
						throw ex;
					}


				}
				#endregion

				#region ChatGPT
				string? xuserid = eventObject.Source.UserId;
				Aimodel aimodelx = new Aimodel();
				aimodelx.userid = xuserid;
				AiRecordInformationDapper aiRecordInformation = new AiRecordInformationDapper();
				List<Aimodel>? loadresponse = aiRecordInformation.Load(aimodelx);
				string? userResponse = eventObject.Message.Text.Trim();
				if (userResponse == "@miko killsumall")
				{
					string filePathx = $"/app/data/{eventObject.Source.GroupId}";
					try
					{
						Directory.Delete(filePathx, true);
						//Console.WriteLine($"GroupId:{eventObject.Source.GroupId}刪除成功");
						ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
						{
							ReplyToken = eventObject.ReplyToken,
							Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = "已刪除今天的說話紀錄"}
									}
						};
						ReplyMessage(replyMessage1);
					}
					catch (Exception ex)
					{
						//Console.WriteLine($"GroupId:{eventObject.Source.GroupId}刪除失敗:今天還沒有人說話喔");
						ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
						{
							ReplyToken = eventObject.ReplyToken,
							Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = "刪除失敗:今天還沒有人說話喔"}
									}
						};
						ReplyMessage(replyMessage1);
					}
					return;
				}

				if (userResponse == "@miko sumall")
				{
					DateTime currentTime = DateTime.Now.AddHours(8);
					string formattedTime = currentTime.ToString("yyyyMMdd HH:mm");
					string txtNameTime = currentTime.ToString("yyyyMMdd");

					string filePathx = $"/app/data/{eventObject.Source.GroupId}/{txtNameTime}.txt";

					if (File.Exists(filePathx))
					{
						string text = File.ReadAllText(filePathx);
						Console.WriteLine("File found!: {0}", text);
						string result2 = await Chatgpt(text + " 請用繁體中文根據時間點總結一下上面的聊天內容。 ");
						Task.Delay(1000);

						ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
						{
							ReplyToken = eventObject.ReplyToken,
							Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = result2}
									}
						};
						ReplyMessage(replyMessage1);
					}
					else
					{
						Console.WriteLine("File not found!");
						ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
						{
							ReplyToken = eventObject.ReplyToken,
							Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = "今天還沒有人講話喔"}
									}
						};
						ReplyMessage(replyMessage1);
					}
					return;
				}

				#region 紀錄群組聊天

				Recordlinemsg(eventObject.Source.GroupId, eventObject.Source.UserId, eventObject.Message.Text);

				#endregion

				if (userResponse == "@miko h")
				{
					string result = "1.@miko+空格+敘述問題\r\n例:@miko 今天要幹嘛\r\n\n2.@miko+空格+繼續\n例:@miko 繼續\r\n這個功能是為了延續miko沒說完的話\r\n\n3.@miko+空格+總結\r\n例:@miko 總結\r\n這個功能是為了總結對話的內容";
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = result}
									}
					};
					ReplyMessage(replyMessage1);
					return;
				}
				if (userResponse.Contains("@miko 總結") && loadresponse.Count != 0 && loadresponse[0].isContinue == "Y")
				{
					//Console.WriteLine("進到總結了");
					string userInput = userResponse;
					string pattern = "@miko\\s(.*)";
					string result = "";
					//過濾問題:總結
					MatchCollection matches = Regex.Matches(userResponse, pattern);
					foreach (Match match in matches)
					{
						result += match.Groups[1].Value;
					}
					Aimodel ai_model = new Aimodel();
					ai_model.userid = eventObject.Source.UserId;
					AiRecordInformationDapper aiRecord = new AiRecordInformationDapper();
					List<Aimodel>? promt = aiRecord.Load(ai_model);
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = promt[0].prompt.Trim()}
									}
					};
					ReplyMessage(replyMessage1);
					aiRecord.Delete(ai_model);
					return;
				}
				if (userResponse.Contains("@miko ") && !userResponse.Contains("@miko 繼續") && loadresponse.Count > 0)//鍵入@miko和有值
				{
					aiRecordInformation.Delete(aimodelx);
					AiRecordInformationDapper aiRecord = new AiRecordInformationDapper();
					Aimodel aimodel = new Aimodel();
					string userInput = userResponse;
					string pattern = "@miko\\s(.*)";
					string result = "";
					MatchCollection matches = Regex.Matches(userResponse, pattern);

					foreach (Match match in matches)
					{
						result += match.Groups[1].Value;
					}

					string result2 = await Chatgpt(result + "Reply in 繁體中文");
					Task.Delay(1000);

					Aimodel ai_model = new Aimodel();
					ai_model.userid = eventObject.Source.UserId;
					ai_model.prompt = result2;
					ai_model.createtime = DateTime.Now;
					ai_model.isContinue = "Y";

					aiRecordInformation.Create(ai_model);
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = result2}
									}
					};
					ReplyMessage(replyMessage1);
				}
				if (userResponse.Contains("@miko ") && loadresponse.Count == 0)//鍵入@miko和空值
				{
					AiRecordInformationDapper aiRecord = new AiRecordInformationDapper();
					Aimodel aimodel = new Aimodel();
					string userInput = userResponse;
					string pattern = "@miko\\s(.*)";
					string result = "";
					MatchCollection matches = Regex.Matches(userResponse, pattern);

					foreach (Match match in matches)
					{
						result += match.Groups[1].Value;
					}

					string result2 = await Chatgpt(result + "Reply in 繁體中文");
					Task.Delay(1000);

					Aimodel ai_model = new Aimodel();
					ai_model.userid = eventObject.Source.UserId;
					ai_model.prompt = result2;
					ai_model.createtime = DateTime.Now;
					ai_model.isContinue = "Y";

					aiRecordInformation.Create(ai_model);
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = result2}
									}
					};
					ReplyMessage(replyMessage1);

				}
				else if (userResponse.Contains("@miko 繼續") && loadresponse.Count != 0 && loadresponse[0].isContinue == "Y")
				{
					string userInput = userResponse;
					string pattern = "@miko\\s(.*)";
					string result = "";
					MatchCollection matches = Regex.Matches(userResponse, pattern);

					foreach (Match match in matches)
					{
						result += match.Groups[1].Value;
					}
					Aimodel ai_model = new Aimodel();
					ai_model.userid = eventObject.Source.UserId;
					AiRecordInformationDapper aiRecord = new AiRecordInformationDapper();
					List<Aimodel>? promt = aiRecord.Load(ai_model);
					string result2 = await Chatgpt(promt[0].prompt + "\n" + result + "Reply in 繁體中文");
					Task.Delay(1000);

					ai_model.prompt = promt[0].prompt + "\n" + result2;
					ai_model.ongoingtime = DateTime.Now;
					ai_model.isContinue = "Y";

					aiRecord.Update(ai_model);
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = result2}
									}
					};
					ReplyMessage(replyMessage1);
				}

				else if (userResponse.Contains("@miko 停止"))
				{
					string userInput = userResponse;
					string pattern = "@miko\\s(.*)";
					string result = "";
					MatchCollection matches = Regex.Matches(userResponse, pattern);

					foreach (Match match in matches)
					{
						result += match.Groups[1].Value;
					}

					AiRecordInformationDapper aiRecord = new AiRecordInformationDapper();
					aiRecord.DeleteALL();
					ReplyMessageRequestDto<TextMessageDto>? replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>()
					{
						ReplyToken = eventObject.ReplyToken,
						Messages = new List<TextMessageDto>
									{
										new TextMessageDto(){Text = "ChaptGPT之紀錄已格式化"}
									}
					};
					ReplyMessage(replyMessage1);
				}

				#endregion


				#region 測試Flex message
				if (eventObject.Message.Text.Trim() == "!日幣" || eventObject.Message.Text.Trim() == "！日幣")
				{
					try
					{
						#region 爬蟲
						string headerTitle = "";
						var web = new HtmlWeb();
						var doc = web.Load("https://rate.bot.com.tw/xrt/quote/ltm/JPY");

						var rows = doc.DocumentNode.Descendants("tr")
							.Where(tr => tr.Descendants("td").Count() >= 3)
							.ToList()
							.Take(7);
						Dictionary<string, string>? dict = new Dictionary<string, string>();
						foreach (var row in rows)
						{
							var date = row.Descendants("td").ElementAt(0).InnerText.Trim();
							var rate = row.Descendants("td").ElementAt(5).InnerText.Trim();

							dict[date] = rate;
						}
						string todayValue = dict.Values.First();
						string minValue = dict.Values.Min();
						if(todayValue== minValue) { headerTitle = "七日內最低價!"; }
						else { headerTitle = "今天價格普通"; }

						todayValue = "NT$ " + todayValue;
						#endregion

						#region 用型別的方式 

						ReplyMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>? replyMessage1 = new ReplyMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>()
						{
							ReplyToken = eventObject.ReplyToken,
							Messages = new List<FlexMessageDto<FlexBubbleContainerDto>>()
							{
								new FlexMessageDto<FlexBubbleContainerDto>()
								{
									Type="flex",
									AltText ="本日-日幣價格",
									contents=new FlexBubbleContainerDto()
									{
										type="bubble",
										size="giga",
										direction="ltr",
										header=new FlexComponentDto()
										{
											type="box",
											layout="vertical",
											contents= new List<FlexComponentDto>()
											{
												new FlexComponentDto()
												{
													type="text",
													text=headerTitle,
													size="45px",
													color="#EA0000",
													style="normal",
													weight="bold",
													align="center"
												}
											}
										},
										body=new FlexComponentDto()
										{
											type="box",
											layout="vertical",
											contents=new List<FlexComponentDto>()
											{
											   new FlexComponentDto()
											   {
												   type="box",
												   layout="vertical",
												   contents=new List<FlexComponentDto>(),
												   position="absolute",
												   background= new FlexBackgroundDto()
												   {
													   type="linearGradient",
													   angle="0deg",
													   endColor="#00000000",
													   startColor="#00000099"
												   },
												   OffsetBottom="0px",
												   OffsetStart="0px",
												   OffsetEnd="0px"
											   },
											   new FlexComponentDto()
											   {
												   type="box",
												   layout="horizontal",
												   contents= new List<FlexComponentDto>()
												   {
													   new FlexComponentDto()
													   {
														   type="box",
														   layout="vertical",
														   contents= new List<FlexComponentDto>()
														   {
															   new FlexComponentDto()
															   {
																   type="box",
																   layout="horizontal",
																   contents= new List<FlexComponentDto>()
																   {
																	   new FlexComponentDto()
																	   {
																		   type="text",
																		   text="本日日幣台灣銀行賣出價格!",
																		   size="xl",
																		   color="#0066FF",
																		   weight="bold"
																	   }
																   }
															   },
															   new FlexComponentDto()
															   {
																   type="box",
																   layout="baseline",
																   contents=new List<FlexComponentDto>(),
																   spacing="xs"
															   },
															   new FlexComponentDto()
															   {
																   type="box",
																   layout="horizontal",
																   contents=new List<FlexComponentDto>()
																   {
																	   new FlexComponentDto()
																	   {
																		   type="box",
																		   layout="baseline",
																		   contents=new List<FlexComponentDto>()
																		   {
																			   new FlexComponentDto()
																			   {
																				   type="text",
																				   text=todayValue,
																				   color="#227700",
																				   size="xxl",
																				   flex=0,
																				   align="end",
																				   weight="bold"
																			   }
																		   },
																		   flex=0,
																		   spacing="lg"

																	   }
																   }
															   }
														   },
														   spacing="xs"
													   }
												   },
												   position="absolute",
												   OffsetBottom="0px",
												   OffsetEnd="0px",
												   paddingAll="20px",
												   JustifyContent="center",
												   OffsetStart="0px"
											   },
											   new FlexComponentDto()
											   {
												   type="image",
												   url="https://i.ibb.co/wJmdc6j/image.jpg",
												   size="full",
												   AspectRatio="1:1",
												   gravity="top",
												   position="relative",
												   margin="xs",
												   align="start"
											   }
											},
											paddingAll="20px",
											position="relative",
											borderColor="#FF1C1C",
											backgroundColor="#FFBB77",
											paddingTop="0px",
											background= new FlexBackgroundDto()
											{
												type="linearGradient",
												angle="0deg",
												startColor="#DDDddd",
												endColor="#FFFFDF"
											}
										},
										styles= new FlexBubbleContainerStyle()
										{
											Header = new FlexBlockStyle()
											{
												separator=false,
												backgroundColor="#FFFFB5",
												separatorColor="#FFFFB5"
											},
											Footer= new FlexBlockStyle()
											{
												backgroundColor="#dddddd",
												separator=true
											}
										}

									}
								}
							}
						};
						ReplyMessage(replyMessage1);
						#endregion

						Console.WriteLine("eventObject.ReplyToken{0}\r ", eventObject.ReplyToken);
					}
					catch(Exception ex)
					{
						Console.WriteLine("這是flex的錯誤:{0}",ex.ToString());
					}
				}
		
				#endregion
			}
		}

		#region chatgpt
		public async Task<string> Chatgpt(string promt)
		{
			string Path = "https://api.openai.com/v1/completions";
			AiClass aiClass = new AiClass()
			{
				model = "text-davinci-003",
				prompt = promt,
				max_tokens = 800,
				temperature = 0.5,
				top_p = 1,
				frequency_penalty = 0.0,
				presence_penalty = 0.6
			};
			string json = JsonConvert.SerializeObject(aiClass);
			HttpContent contentPost = new StringContent(json, Encoding.UTF8, "application/json");
			HttpClient client = new HttpClient() { BaseAddress = new Uri(Path) };
			client.DefaultRequestHeaders.Add("authorization", "Bearer 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");
			HttpResponseMessage response = await client.PostAsync(Path, contentPost);
			Airesponse? result = JsonConvert.DeserializeObject<Airesponse>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
			try
			{
				//Console.WriteLine($"回呼chatgpt成功:{result.choices[0].text.Trim()}");
				return result.choices[0].text.Trim();

			}
			catch (Exception ex)
			{
				return $"回呼chatgpt失敗錯誤代碼:{ex.Message}";
			}
		}
		#endregion

		public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
		{

			try
			{
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
				string? json = _jsonProvider.Serialize(request);
				Console.WriteLine("最後序列化的{0}",json);
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
		#region 點餐流程
		public void OrderFoodPhase1(string userID, string mealtype, string step)
		{
			//Console.WriteLine("進到OrderFoodPhase1");
			UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
			informationDapper.Delete();
			UserRecord record = new UserRecord();
			record.id = userID;
			record.mealtype = mealtype;
			record.step = step;
			informationDapper.Create(record);
		}
		public void OrderFoodPhase2(string userID, string mealtype, string foodtype, string step)
		{
			//Console.WriteLine("進到OrderFoodPhase2");
			UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
			UserRecord record = new UserRecord();
			record.id = userID;
			record.mealtype = mealtype;
			record.foodtype = foodtype;
			record.step = step;
			informationDapper.Update(record);
		}
		public void OrderFoodPhase3(string userID, string mealtype, string foodtype, string Lat, string Lon, string step)
		{
			//Console.WriteLine("進到OrderFoodPhase3");
			UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
			UserRecord record = new UserRecord();
			record.id = userID;
			record.mealtype = mealtype;
			record.foodtype = foodtype;
			record.lat = Lat;
			record.lon = Lon;
			record.step = step;
			informationDapper.Update(record);
			Console.WriteLine("OrderFoodPhase3完成");
		}
		public void OrderFoodPhase4(string userID, string mealtype, string foodtype, string Lat, string Lon, string step, string budget)
		{
			//Console.WriteLine("進到OrderFoodPhase4");
			UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
			UserRecord record = new UserRecord();
			record.id = userID;
			record.mealtype = mealtype;
			record.foodtype = foodtype;
			record.lat = Lat;
			record.lon = Lon;
			record.step = step;
			record.budget = budget;
			informationDapper.Update(record);
			Console.WriteLine("OrderFoodPhase4完成");
		}
		public async void OrderFoodPhase5(string eventObject_token)
		{
			//Console.WriteLine("進到OrderFoodPhase5");
			UserRecordInformationDapper informationDapper = new UserRecordInformationDapper();
			UserRecord? result = informationDapper.Load().First();
			if (result.step == "40")
			{
				// Replace YOUR_API_KEY with your actual API key
				string apiKey = "AIzaSyDPPsEaO_DDA8B4GQneWuztLgqFERD5aB0";

				// Set the location and radius for the search
				int budget = 0;
				string location = result.lat + "," + result.lon;
				string radius = "3000";
				string keyword = result.mealtype + "+" + result.foodtype;
				if (result.budget == "low") { budget = 1; }
				if (result.budget == "mid") { budget = 2; }
				if (result.budget == "high") { budget = 3; }

				string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location}&radius={radius}&keyword={keyword}&key={apiKey}&minprice={budget}&maxprice={budget}";
				//Console.WriteLine(url);
				// Send the request and get the response
				using (var client = new HttpClient())
				{
					HttpResponseMessage? response = await client.GetAsync(url);
					string? content = await response.Content.ReadAsStringAsync();
					PlacesApiResponse? results = JsonConvert.DeserializeObject<PlacesApiResponse>(content);
					Console.WriteLine($"總共有");
					List<Place>? restaurants = results.Results.ToList();
					int count = restaurants.Count();
					//Console.WriteLine($"Price_level:{budget}\n總共有:{count}筆");
					if (count == 0)
					{
						string filePath = "/app/data/status.txt";
						File.Delete(filePath);
						//Console.WriteLine("搜尋筆數為零，請重新操作");
						result.step = "99";
						informationDapper.Update(result);
						ReplyMessageRequestDto<TextMessageDto> replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>();
						replyMessage1 = new ReplyMessageRequestDto<TextMessageDto>
						{
							ReplyToken = eventObject_token,
							Messages = new List<TextMessageDto>
										{
											new TextMessageDto
											{
												Text="搜尋筆數為零，請重新操作"
											}
										 }
						};
						ReplyMessage(replyMessage1);
					}
					if (count > 0)
					{
						result.step = "50";
						informationDapper.Update(result);
						//Console.WriteLine($"總共搜尋到:{count}個");
						dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();
						List<TemplateMessageDto<CarouselTemplateDto>>? result1 = await bingCarousel(restaurants);
						if (result1[0].Template.Columns.Count > 10)
						{
							result1[0].Template.Columns = result1[0].Template.Columns.Take(10).ToList();
							//Console.WriteLine($"目List篩選過的的數量為:{result1[0].Template.Columns.Count}個");
						}
						else
						{
							//Console.WriteLine($"目List篩選過的的數量為:{result1[0].Template.Columns.Count}個");
						}
						Task.WaitAll();

						replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>()
						{
							ReplyToken = eventObject_token,
							Messages = result1
						};

						string filePath = "/app/data/status.txt";
						File.Delete(filePath);
						ReplyMessage(replyMessage);
					}

					Console.WriteLine("OrderFoodPhase5完成");
				}
			}
			else
			{
				return;
			}


		}

		public void stop()
		{
			string filePath = "/app/data/status.txt";
			File.Delete(filePath);
			UserRecordInformationDapper userRecord = new UserRecordInformationDapper();
			userRecord.Delete();
		}

		public bool JudgeExsitLog()
		{
			string filePath = "/app/data/status.txt";
			if (File.Exists(filePath))
			{
				//讀取第一行
				string firstLine = File.ReadLines(filePath).First();
				//建檔時間
				DateTime time = DateTime.Parse(firstLine);
				//設定2分鐘區間
				TimeSpan interval = TimeSpan.FromMinutes(2);
				DateTime now = DateTime.Now;
				TimeSpan diff = now.Subtract(time);
				//如果當前時間跟文本時間相比，是超過設定的2分鐘，則回傳大於一的整數。
				if (diff.CompareTo(interval) > 0)
				{
					//Console.WriteLine("前一次啟用系統超過2分鐘，將刪除紀錄");
					File.Delete(filePath);
					return false;
				}
				return true;

			}
			else
			{
				return false;
			}
		}
		#endregion

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
					//Console.WriteLine("抓取天氣API失敗!");
					return null;
				}
				return carouselList;
			}
			catch (HttpRequestException e)
			{
				//Console.WriteLine("抓取天氣失敗!");
				Console.WriteLine("Message :{0} ", e.Message.ToString());
				return null;
			}

		}
		#endregion

		#region 查詢bing圖片api
		public static async Task<List<TemplateMessageDto<CarouselTemplateDto>>> bingCarousel(List<Place> name)
		{
			try
			{
				List<TemplateMessageDto<CarouselTemplateDto>> dtos = new List<TemplateMessageDto<CarouselTemplateDto>>();
				TemplateMessageDto<CarouselTemplateDto> messageDto = new TemplateMessageDto<CarouselTemplateDto>();
				List<CarouselColumnObjectDto> carouselList = new List<CarouselColumnObjectDto>();

				foreach (Place xName in name)
				{

					string TitleContent = "";
					if (xName.Name.Length > 30) { TitleContent = xName.Name.Substring(0, 30); }
					else { TitleContent = xName.Name; }

					const string apiKey = "8049b67d654d48f19130d93f9b7c1017";
					HttpClient? client = new HttpClient { BaseAddress = new Uri("https://api.bing.microsoft.com/v7.0/images/search") };
					client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

					HttpResponseMessage? response = await client.GetAsync($"?q={TitleContent}&count=1");
					string? json = await response.Content.ReadAsStringAsync();
					dynamic data = JsonConvert.DeserializeObject(json);
					if (response.IsSuccessStatusCode == true)
					{
						foreach (var image in data.value)
						{
							string originalString = image.name;
							int maxLength = 55;
							string TextContent;
							if (originalString.Length > maxLength)
							{
								TextContent = originalString.Substring(0, maxLength) + "...";
							}
							else
							{
								TextContent = originalString;
							}
							response.EnsureSuccessStatusCode();

							string? name123 = HttpUtility.UrlEncode(xName.Name);
							CarouselColumnObjectDto? carouselColumnObject = new CarouselColumnObjectDto();
							//carouselColumnObject.ThumbnailImageUrl = data.value[0].thumbnailUrl;
							carouselColumnObject.ThumbnailImageUrl = image.thumbnailUrl;
							carouselColumnObject.Title = "店名: " + TitleContent;
							//carouselColumnObject.Text = data.value[0].name;
							carouselColumnObject.Text = TextContent;
							carouselColumnObject.Actions = new List<ActionDto>();
							ActionDto? action = new ActionDto();
							action.Type = ActionTypeEnum.Uri;
							action.Label = "立即導航";
							//action.Uri = "https://www.apple.com/tw/iphone-14-pro/?afid=p238%7Cs2W650oa9-dc_mtid_2092576n66464_pcrid_620529299490_pgrid_144614079327_&cid=wwa-tw-kwgo-iphone-slid---productid--Brand-iPhone14Pro-Announce-";
							action.Uri = $"https://www.google.com/maps?q={name123}";
							carouselColumnObject.Actions.Add(action);
							carouselList.Add(carouselColumnObject);

						}
						Console.WriteLine("Bing_Search_Api:成功");

					}
					else
					{
						Console.WriteLine("Bing_Search_Api:失敗");
					}

				}

				messageDto.AltText = "這是美食推播";
				messageDto.Template = new CarouselTemplateDto { Columns = carouselList };
				dtos.Add(messageDto);
				return dtos;
			}
			catch (Exception e)
			{
				Console.WriteLine("Bing_Search_Api失敗 :\n{0} ", e.Message.ToString());
				return null;
			}

		}
		#endregion

	}
}


