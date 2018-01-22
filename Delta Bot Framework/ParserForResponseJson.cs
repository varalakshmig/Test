using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SampleBotTemplate
{
    public class ParserForResponseJson
    {
        public static string strRet = string.Empty;
            public static string strTemplate = string.Empty;
            public static string strWeb = string.Empty;
            public static string BotJson; 
            public static CustomerResponse GetResponse(CustomerResponse custResp)
            {
            try
            {
                if (custResp.responseContext == null || custResp.responseContext == "")
                {
                    custResp.responseContext = "None";
                }
                
                string json1 = "{" + @"Result" + ":" + BotJson + "}";
                responseLookup lookup = null;
                try
                {
                    lookup = JsonConvert.DeserializeObject<responseLookup>(BotJson);
                }

                catch (Exception e)
                {
                    throw (e);
                }



                if (null != lookup)
                {
                    int i = 0;
                    foreach (lResult r1 in lookup.Result)
                    {


                        if (r1.intent == custResp.intent && r1.incoming_context == custResp.responseContext)
                        {

                            strRet = r1.response;
                            custResp.respTxt = strRet;
                            custResp.IncomingContext = custResp.responseContext;
                            custResp.OutgoingContext = r1.outgoing_context;
                            custResp.ResponseType= r1.response_type.ToUpper();
                            string strResponseType = r1.response_type.ToUpper();

                            if (strResponseType.Contains("FACEBOOK"))
                            {
                                custResp.TemplateName = r1.response;
                            }
                            if (strResponseType == "COMPONENT")
                            {
                                custResp.respTxt = null;
                                custResp.ComponentName = r1.response;
                            }
                            if (strResponseType.Contains("TWILIO"))
                            {
                                custResp.TemplateName = r1.response;
                                //custResp.channel = "twilio";
                            }

                            if (strResponseType == "TEMPLATE")
                            {
                                custResp.TemplateName = r1.response;
                            }
                            else
                            {
                                custResp.TemplateName = null;
                            }
                            if (strResponseType == "SERVICE")
                            {
                                custResp.serviceName = r1.response;
                            }
                            else
                            {
                                custResp.serviceName = null;
                            }

                            if (r1.response.Contains("carousel"))
                            {
                                foreach (lcards c in r1.cards)
                                {
                                    custResp.CardList.Add(c);
                                }
                            }

                            else if (r1.buttons != null && (r1.card_title != null || r1.card_title != ""))
                            {
                                int j = 0;
                                foreach (lbuttons b in r1.buttons)
                                {

                                    custResp.buttonTitle[j] = b.Title;
                                    custResp.buttonType[j] = b.Type;
                                    custResp.buttonValue[j] = b.Value;
                                    j = j + 1;
                                    custResp.buttonCount = j;
                                }

                                custResp.HeroImg = r1.card_image;
                                custResp.HeroTitle = r1.card_title;
                                custResp.HeroSubtitle = r1.card_subtitle;
                                custResp.HeroText = r1.card_text;
                            }
                            custResp.next_response = "no";// r1.next_response;

                        }

                    }
                    
                }
            }
            catch (Exception e)
            {

            }
                return custResp;
            }

            public class lResult
            {
                public string response { get; set; }
                public string incoming_context { get; set; }
                public string request { get; set; }
                public string entity { get; set; }
                public string line { get; set; }
                public string intent { get; set; }
                public string response_type { get; set; }
                public string outgoing_context { get; set; }
                public string next_response { get; set; }
                public lbuttons[] buttons { get; set; }
                public string card_image { get; set; }
                public string card_title { get; set; }
                public string card_subtitle { get; set; }
                public string card_text { get; set; }
                public lcards[] cards { get; set; }
            }
            public class lcards
            {
                //public string card_name { get; set; }
                public string card_image { get; set; }
                public string card_title { get; set; }
                public string card_subtitle { get; set; }
                public string card_text { get; set; }
                public lbuttons[] buttons { get; set; }
            }

            public class recieptCard
            {
                public string title { get; set; }
                public lbuttons[] buttons { get; set; }
                public litems[] items { get; set; }
                public string total { get; set; }
                public string tax { get; set; }
            }

            public class signIn
            {
                public string text { get; set; }
                public lbuttons[] buttons { get; set; }
            }
            public class lbuttons
            {
                public string Value { get; set; }
                public string Type { get; set; }
                public string Title { get; set; }
            }

            public class litems
            {
                public string Title { get; set; }
                public string Subtitle { get; set; }
                public string Text { get; set; }
                public string Image { get; set; }
                public string Price { get; set; }
                public string Quantity { get; set; }
                public string Tap { get; set; }
            }

            public class responseLookup
            {
                public lResult[] Result { get; set; }

            }
        }
    }
