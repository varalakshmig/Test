using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using SampleBotTemplate.Utility;
using SampleBotTemplate.Models;
using Newtonsoft.Json.Linq;
using TweetSharp;
using System.Web.Script.Serialization;
using Microsoft.Azure.Documents;
namespace SampleBotTemplate
{
    public abstract class BaseComponents:BaseService
    {
        private static string knowledgeBaseID=ConfigurationManager.AppSettings["QandAKnowledgeBaseID"].ToString();
        private static string knowledgeBaseName = ConfigurationManager.AppSettings["knowledgeBaseName"].ToString();
        private static string QnASubscriptionKey = ConfigurationManager.AppSettings["QandASubscriptionKey"].ToString();
        public abstract CustomerResponse SelectComponentAsync(CustomerResponse custResInfo, string UserId, string inputMessage);
        public CustomerResponse SendTextNotification(CustomerResponse custResInfo, string userID)
        {
            string phoneno = string.Empty;
            phoneno = userID.Substring(2, 3) + "-" + userID.Substring(5, 3) + "-" + userID.Substring(8, 4);
            custResInfo.respTxt = "Please reply 'Yes', if you want to receive the SMS notification to the number  " + phoneno + " . If you want to get the notification in any other number, please tell us the number.";
            return custResInfo;
        }
        public CustomerResponse QnAMaker(CustomerResponse custResInfo, string userID)
        {
            string reply;
            if (custResInfo.InputMessage.ToLower().Contains("add") && custResInfo.InputMessage.ToLower().Contains("#"))
            {
                string[] separator = { "add", "Add", "ADD", "#" };
                string[] stringarray = custResInfo.InputMessage.Split(separator, StringSplitOptions.None);
                string keyword = stringarray[1].Trim();
                string definition = stringarray[2].Trim();
                if (definition.ToLower().Contains("null"))
                {
                    definition = null;
                }
                Trace.TraceError("Split. Definition-" + definition + " and Keyword-" + keyword + "");
                ConnectionManager.InsOrUpdateDefinitions(keyword, definition);
                reply = "Thanks for making Delta Speak better. We have noted your change and it will be reflected once its approved.";
            }
            else
            {
                knowledgeBaseID = ConnectionManager.GetKnowledgebaseID(knowledgeBaseName);
                reply = CallQandAMaker(custResInfo.InputMessage, knowledgeBaseID, QnASubscriptionKey).Replace("&amp;", "&").Replace("\\n", "\n");
                if (reply.Contains("No good match found in the KB"))
                {
                    reply = "I am unfamiliar with this item. If you discover its meaning, please contribute to our collection by responding in this format. ADD keyword#Definition#Employee number";
                }
            }
           
            custResInfo.respTxt = reply;
            return custResInfo;
        }
       
    }
}