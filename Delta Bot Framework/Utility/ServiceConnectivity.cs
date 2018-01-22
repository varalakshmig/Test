using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


namespace SampleBotTemplate
{
    class ServiceConnectivity
    {
        private bool stop_ind = false;
        private string response = string.Empty;

        public bool Stop_Indicator
        {
            get
            {
                return stop_ind;
            }
            set
            {
                this.stop_ind = value;
            }
        }
        public string Response_Text
        {
            get
            {
                return response;
            }
            set
            {
                this.response = value;
            }
        }
        public async Task<LUIS> GetLuisClientConnection(string input,string subscriptionkey)
        {
            string strRet = string.Empty;
            string strEscaped = Uri.EscapeDataString(input);

            using (var client = new HttpClient())
            {
                string uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7c076fca-b6fa-4d20-90ff-9a396f051e27?subscription-key=" + subscriptionkey + "&timezoneOffset=0.0&verbose=true&q=" + strEscaped;
                HttpResponseMessage msg = await client.GetAsync(uri);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonResponse = await msg.Content.ReadAsStringAsync();
                    var _Data = JsonConvert.DeserializeObject<LUIS>(jsonResponse);
                    if (_Data.topScoringIntent.intent == "stop")
                    {
                        Stop_Indicator = true;
                    }
                    else if (_Data.topScoringIntent.intent == "help")
                    {
                        Response_Text = "Type MENU to get the menu, STOP to stop receiving message, CONTACT for delta contact details.";
                    }
                    else if (_Data.topScoringIntent.intent == "contact")
                    {
                        Response_Text = "Contact 18002211212 for domestic reservation, 18887503284 for delta.com support.";
                    }
                    return _Data;
                }
            }
            return null;

        }
    }
    public class LUIS
    {
        public string query { get; set; }
        public lTopScoringIntent topScoringIntent { get; set; }
        public lTopScoringIntent[] intents { get; set; }
        public lEntity[] entities { get; set; }
        public dialog dialog { get; set; }
    }

    public class lTopScoringIntent
    {
        public string intent { get; set; }
        public float score { get; set; }
        public lactions[] actions { get; set; }
    }

    public class lactions
    {
        public Boolean triggered;
        public string name;
        public lparameters[] parameters;
    }

    public class lparameters
    {
        public string name;
        public string type;
        public Boolean required;
        public value[] value;
    }

    public class lEntity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
        public resolution resolution { get; set; }
    }

    public class resolution
    {
        public string comment { get; set; }
        public string time { get; set; }
    }

    public class value
    {
        public string entity { get; set; }
        public string type { get; set; }
        public resolution resolution { get; set; }
    }

    public class dialog
    {
        public string prompt { get; set; }
        public string parameterName { get; set; }
        public string parameterType { get; set; }
        public string contextId { get; set; }
        public string status { get; set; }
    }

}
