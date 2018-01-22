using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using SampleBotTemplate.Utility;
namespace SampleBotTemplate
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        public static string userId;
        private static string intent;
        private static string inputMessage;
        private static string intentScore;
        private static string ResponseContext;
        private static string resptxt;
        private static string ChannelId;
        private static string ADOConnectionstring = "Server=tcp:dlbotdbs1.database.windows.net,1433;Initial Catalog=DLBotDB;Persist Security Info=False;User ID=DlBotdbs1;Password=delta@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static string userbotname;
        public static string price;
       
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

         
            Dictionary<string, string> entityDictionary = new Dictionary<string, string>();
            CustomerResponse custResInfo = new CustomerResponse();
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                
                try
                {

                    //change the link to luis master for bot creator !
                    string LuisUrl = ConfigurationManager.AppSettings["qnaluislink"];
                    string text = string.Empty;
                    text = activity.Text;
                    Luis.RootObject stLuis = await Luis.LUISClient.ParseUserInput(text, LuisUrl);
                    userId = activity.Id.Substring(0, 10);
                    ChannelId = activity.ChannelId;

                    if (ChannelId.ToLower() == "facebook")
                    {
                        userId = activity.From.Id.Trim();
                    }
                    else if (ChannelId.ToLower() == "skype")
                    {
                        userId = "abc123";// activity.Conversation.Id;
                    }
                    else if (ChannelId.ToLower() == "directline")
                    {
                        userId = activity.From.Id.Trim().Replace("user_", "");
                    }
                    if (ChannelId.ToLower() == "emulator")
                    {
                        userId = "+14049806196";
                    }

                    intent = stLuis.topScoringIntent.intent;
                    intentScore = stLuis.topScoringIntent.score.ToString();

                    //Working with entities
                    for (int i = 0; i < stLuis.entities.Count(); i++)
                    {
                        if (i == 0) custResInfo.Entity = stLuis.entities[i].entity.ToLower();
                        if(!entityDictionary.ContainsKey(stLuis.entities[i].type.ToLower())) entityDictionary.Add(stLuis.entities[i].type.ToLower(), stLuis.entities[i].entity.ToLower());
                        if(!custResInfo.EntityList.ContainsKey(stLuis.entities[i].type.ToLower())) custResInfo.EntityList.Add(stLuis.entities[i].type.ToLower(), stLuis.entities[i].entity.ToLower());

                        string js = string.Empty;
                        Newtonsoft.Json.Linq.JObject parseddata = new Newtonsoft.Json.Linq.JObject();
                        
                        if (!stLuis.entities[i].type.ToLower().Equals("airport"))
                        {
                            if (stLuis.entities[i].resolution != null && stLuis.entities[i].resolution.values != null)
                            js = JsonConvert.SerializeObject(stLuis.entities[i].resolution.values[0]);
                        }
                        if (!stLuis.entities[i].type.ToLower().Equals("airports") && !stLuis.entities[i].type.ToLower().Equals("airport") &&
                            !stLuis.entities[i].type.ToLower().Equals("keywords")&& !stLuis.entities[i].type.ToLower().Equals("stations"))
                        {
                            if(!String.IsNullOrEmpty(js))
                            parseddata = Newtonsoft.Json.Linq.JObject.Parse(js);
                        }
                        //for date and time
                        if (stLuis.entities[i].type == "builtin.datetimeV2.date")
                        {
                            entityDictionary.Add(parseddata["type"].ToString().ToLower(), parseddata["timex"].ToString().Replace("XXXX", "2017"));
                            custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), parseddata["timex"].ToString().Replace("XXXX", "2017"));
                        }

                        if (stLuis.entities[i].type == "builtin.datetimeV2.timerange")
                        {
                           

                                if (parseddata.Property("end") != null && parseddata.Property("start") !=null)
                            {
                                entityDictionary.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + parseddata["end"].ToString());
                                custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + parseddata["end"].ToString());
                            }

                         
                            else if (parseddata.Property("end") == null)
                            {
                                entityDictionary.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + "23:59:00");
                                custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + "23:59:00");
                            }
                            else if (parseddata.Property("start") == null)
                            {
                                entityDictionary.Add(parseddata["type"].ToString().ToLower(), "00:01:00" + "-" + parseddata["end"].ToString());
                                custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), "00:01:00" + "-" + parseddata["end"].ToString());
                            }

                              

                        }
                        if (stLuis.entities[i].type == "builtin.datetimeV2.time")
                        {
                            entityDictionary.Add(parseddata["type"].ToString().ToLower(), parseddata["value"].ToString());
                            custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), parseddata["value"].ToString());

                        }
                        if (stLuis.entities[i].type == "builtin.datetimeV2.datetimerange")
                        {
                            entityDictionary.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + parseddata["end"].ToString());
                            custResInfo.EntityList.Add(parseddata["type"].ToString().ToLower(), parseddata["start"].ToString() + "-" + parseddata["end"].ToString());

                        }
                    }

                    inputMessage = text;

                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error:" + ex + " Method: LUIS ");

                }
               

                try
                {
                    string errorText = Utility.Utility.ValidateUser(userId);
                    if (String.IsNullOrEmpty(errorText))
                    {
                        //Reset ResponseContext
                        Utility.Utility.ResetContext(intent, userId);
                        //Generate Response For common intents
                        //custResInfo.respTxt = Utility.Utility.ReplyForCommonIntents(intent);
                        custResInfo.ChannelId = ChannelId;
                        custResInfo.InputMessage = inputMessage;
                        custResInfo.botName = ConfigurationManager.AppSettings["BotName"];
                        if (String.IsNullOrEmpty(custResInfo.respTxt) || userId == "")
                            //other intents
                            custResInfo = await ResponseProvider.GetResponse(userId, inputMessage, intent, custResInfo, ChannelId, entityDictionary, intentScore);
                    }
                    else
                    {
                        custResInfo.respTxt = errorText;
                    }
                    ResponseContext = custResInfo.responseContext;
                }
                catch (Exception ex)
                {
                    if (String.IsNullOrEmpty(userId) == false && userId != "")
                        ConnectionManager.DeleteResponseContextForBot(userId);
                    Trace.TraceError("Error:" + ex + " Method: LUIS ");
                    custResInfo.respTxt = "Sorry. There is a problem in the system. Please come back later. ";
                }


                Activity reply = activity.CreateReply($"");
                Activity replyToConversation = activity.CreateReply("");
                replyToConversation.Recipient = activity.From;
                replyToConversation.Type = "message";
                replyToConversation.Attachments = new List<Attachment>();

                if (custResInfo.facebookTemplate != null)
                {
                    reply.ChannelData = custResInfo.facebookTemplate;
                    if (null != reply.ChannelData)
                    {
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }

                if (custResInfo.respTxt == null || custResInfo.respTxt == "")
                {
                }
                else
                {
                    reply = activity.CreateReply(custResInfo.respTxt);

                    await connector.Conversations.ReplyToActivityAsync(reply);
                } 

                if (custResInfo.plAttachment != null)
                {
                    replyToConversation.Attachments.Add(custResInfo.plAttachment);
                    var reply1 = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                if (custResInfo.plAttachmentList != null)
                {
                    replyToConversation.Attachments = custResInfo.plAttachmentList;
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    var reply1 = await connector.Conversations.SendToConversationAsync(replyToConversation);
                }
                
            }
            else
            {
                HandleSystemMessage(activity);
            }


            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

    }
}