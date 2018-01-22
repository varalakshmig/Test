using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SampleBotTemplate.Models;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Http;

namespace SampleBotTemplate
{
    public abstract class BaseService
    {
        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        private Dictionary<string, string> cookies;
        public static List<KeyValuePair<string, string>> cookieList = new List<KeyValuePair<string, string>>();
        public static string VerifySelection(string replyText)
        {
            try
            {

                string menutxt = ConnectionManager.FindMenu(replyText);
                return menutxt;
            }
            catch (Exception ex)
            {
                return "E";
            }
        }
        public static bool UpdateSelection(string menu)
        {
            try
            {
                //update the menu
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string GETAPICall(string destinationUrl)
        {
            string result = string.Empty;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                HttpResponseMessage response = client.GetAsync(destinationUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:" + ex + " Method: GETAPICall ");
                throw (ex);
            }
            return result;
       }
        public static string postXMLData(string destinationUrl, string requestXml)
        {
            string responseStr = string.Empty;
            try
            {
                // To avoid SSL Socket error
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);

                request.ContentType = "application/xml";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ReadAllCookies(response.Headers);
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: serviceToFindAvailableFlights ");
                throw (e);
            }
            return responseStr;
        }
        public static string PostCalltoComponentAPI(string destinationUrl,Request apiRequest)
        {
            string responseStr = string.Empty;
            try
            {
                // To avoid SSL Socket error
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                var parameters = Newtonsoft.Json.JsonConvert.SerializeObject(apiRequest);
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
                request.ContentLength = bytes.Length;
                using (var os = request.GetRequestStream())
                {
                    os.Write(bytes, 0, bytes.Length);

                    os.Close();
                }
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();

                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: PostCalltoComponentAPI ");
                throw (e);
            }
            return responseStr;
        }
        public static string postXMLDatawithCookies(string destinationUrl, string requestXml)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
            byte[] bytes;
            bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);


            for (int i = 0; i < cookieList.Count; i++)
            {
                try
                {

                    Cookie cookie = new Cookie()
                    {
                        Domain = ".delta.com",
                        Name = cookieList[i].Key,
                        Value = cookieList[i].Value
                    };
                    if (request.CookieContainer == null)
                    {
                        request.CookieContainer = new CookieContainer();
                    }

                    request.CookieContainer.Add(cookie);
                    // request.CookieContainer.Add(cookie);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error:" + e + " Method: serviceSelectAlternateFlight ");
                    throw (e);
                }
            }
            request.ContentType = "application/xml";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                //ReadAllCookies(response.Headers);
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                return responseStr;
            }
            return null;
        }    
        public static string GetNameFromFacebook(string Input, string URL)
        {
               string rt;
                try
                {
                    //string URL = "https://graph.facebook.com/v2.6/" + userId + "?fields=first_name,last_name,profile_pic,locale,timezone,gender&access_token=EAAOMDKZA0oRUBACd5ZA2KKgZBSHBHQdmOdZB1upMWIpUan3J3xxZB8i6smojRlmBQZAo6LNHCh8Vd06nyDTspdcZCOgc2mmQK2JWNqpcnZCHsBiYo9F7kjpKGBDsBFsgamtd5bqacMXy9gEZCEthLUlEtsTSqsVAUos3sa2m5aQcnlwZDZD";
                    WebRequest Request = WebRequest.Create(URL);
                    WebResponse Resp = Request.GetResponse();
                    Stream dataStream = Resp.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    rt = reader.ReadToEnd();
                    reader.Close();
                    Resp.Close();
                }

                catch (Exception e)
                {
                    Trace.TraceError("Error:" + e + " Method: CreateXMLforPNR");
                    throw (e);
                }
                return rt;
            

        }
   
        public static void ReadAllCookies(WebHeaderCollection headers)
        {
            try
            {
                var list = new List<KeyValuePair<string, string>>();
                var cookies = headers.GetValues("Set-Cookie");
                if (cookies == null)
                { }
                else
                {
                    foreach (var v in cookies)
                    {
                        string[] kvPairs = v.Split(';');
                        foreach (string s in kvPairs)
                        {
                            string[] kv = s.Split('=');
                            string key = kv[0];
                            string value = kv[1];
                            list.Add(new KeyValuePair<string, string>(key, value));
                            break;
                        }
                        //cookieListKey.Add(v);
                        Console.WriteLine(v);
                    }
                    cookieList = list;
                    var ii = cookieList.Count;
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("Error:" + e + " Method: serviceToFindAvailableFlights ");
                //throw (e);
            }

        }
        public static string CallQandAMaker(string question, string knowledgebaseid, string subkey)
        {
            string responseString = string.Empty;
            var query = question; //User Query 
            var knowledgebaseId = knowledgebaseid; // Use knowledge base id created. 
            var qnamakerSubscriptionKey = subkey; //Use subscription key assigned to you. 
                                                  //Build the URI 
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");
            //Add the question as part of the body 
            var postBody = $"{{\"question\": \"{query}\"}}";
            //Send the POST request 
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8 
                client.Encoding = System.Text.Encoding.UTF8;
                //Add the subscription key header 
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }
            return GetDeserializedResponse(responseString);
        }
        private static string GetDeserializedResponse(string jsonString)
        {
            QnAMakerResult response;
            try
            {
                response = JsonConvert.DeserializeObject<QnAMakerResult>(jsonString);
            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }
            return response.Answer;
        }
    }
    public class QnAMakerResult
    {
        /// <summary> 
        /// The top answer found in the QnA Service. 
        /// </summary> 
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
        /// <summary> 
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service. 
        /// </summary> 
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}