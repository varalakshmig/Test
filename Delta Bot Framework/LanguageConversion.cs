using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SampleBotTemplate
{
    struct Doc
    {
        public string id;
        public string text;
    }
    public class LanguageConversion
    {
        static readonly string APIKEY = "a7e9abcbb666400c9cf70354e418f627";
        const string subscriptionKey = "9eab7523404c4be7b08641e812bca7fb";
        public static string ReplyText;
        const string uriBase = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/languages";
        public static string convert(string text1,string lang)
        {
            Task.Run(async () =>
            {
                var accessToken = await GetAuthenticationToken(APIKEY);
                var output = await Translate(text1, lang, accessToken);
                ReplyText = output;
            }).Wait();
            return ReplyText;
        }
        static async Task<string> Translate(string textToTranslate, string language, string accessToken)
        {
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            string query = $"?text={System.Net.WebUtility.UrlEncode(textToTranslate)}&to={language}&contentType=text/plain";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return "ERROR: " + result;
                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }
        static async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }

        /// <summary>
        /// Queries the language for a set of document and outputs the information to the console.
        /// </summary>
        static async Task<string> GetLanguage(List<Doc> documents)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            HttpResponseMessage response;

            // Compose request.
            string body = "";
            foreach (Doc doc in documents)
            {
                if (!string.IsNullOrEmpty(body))
                {
                    body = body + ",";
                }

                body = body + "{ \"id\":\"" + doc.id + "\",  \"text\": \"" + doc.text + "\"   }";
            }

            body = "{  \"documents\": [" + body + "] }";

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(body);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uriBase, content);
            }

            // Get the JSON response
            string result = await response.Content.ReadAsStringAsync();

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("\nResponse:\n");
            JObject res = JObject.Parse(result);
            return res["documents"][0]["detectedLanguages"][0]["iso6391Name"].ToString();
        }
    }
}