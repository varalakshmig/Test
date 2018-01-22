using SampleBotTemplate.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
namespace SampleBotTemplate
{
    public class ResponseProvider
    {
        public static async Task<CustomerResponse> GetResponse(string userId, string InputMessage, string Intent, CustomerResponse custResInfo, string channelId,Dictionary<string,string> entityDictionary,string intentScore)
        {
            bool isSave = true;
			string jsonstring = Utility.Utility.FromDictionaryToJson(entityDictionary);
            try
            {
                string responseContext = string.Empty;
                string outputMessage= string.Empty;
                custResInfo.intent = Intent;                
                do
                {
                    // get response and assign o CustomerResponse.ResponseContext
                    ConnectionManager.InsOrUpdateIntentForBot(Intent, userId);
                    custResInfo.responseContext = ConnectionManager.getResponseContextForBot(userId);
                    custResInfo = ConnectionManager.GetLastConversationDetails(userId, custResInfo);
                   
                    //UpdateComponentData
                    if (custResInfo.ResponseType.ToUpper() == "COMPONENT")
                        outputMessage=Utility.Utility.UpdateComponentData(InputMessage, custResInfo.respTxt, userId, outputMessage,entityDictionary);
                   
                    //SaveResponse
                    bool proceedWithNextContext = true;
                    string propToValidate = "";
                    bool saveOperation = false;
                    if (custResInfo.ResponseAction.ToUpper() == "SAVE" && isSave)
                    {
                        saveOperation = true;
                        //ConnectionManager.SaveResponse(InputMessage, userId, custResInfo.IncomingContext, "", custResInfo.botName);
                        ConnectionManager.SaveResponse(InputMessage, userId, custResInfo.attribute, "", custResInfo.botName);
                    }
                    custResInfo.intent = Intent;

                    if (proceedWithNextContext)
                    {
                        if (String.IsNullOrEmpty(outputMessage))
                            custResInfo = FetchResponseForIntent.GetResponse(custResInfo);
                        else
                        {
                            custResInfo.respTxt = outputMessage;
                            custResInfo.RespTxtCopy = outputMessage;
                        }

                        ConnectionManager.InsertTransaction(custResInfo.ChannelId, custResInfo.InputMessage, custResInfo.responseContext, custResInfo.botName, userId, Intent, jsonstring, intentScore);

                        if (custResInfo.ComponentName != null)
                        {
                            BaseComponents objBaseComponents = ComponentSelector.SelectChannelComponents(custResInfo.ChannelId, custResInfo.ComponentName);
                            objBaseComponents.SelectComponentAsync(custResInfo, userId, InputMessage);
                        }

                        if (custResInfo.OutgoingContext == "" || custResInfo.OutgoingContext == null)
                        {
                            custResInfo.OutgoingContext = "None";
                        }
                        custResInfo.responseContext = custResInfo.OutgoingContext;
                        //ConnectionManager.UpdateResponseContextForBot(custResInfo, userId);
                        ConnectionManager.UpdateResponseContextForBotCopy(custResInfo, userId);
                    }
                    else if(!proceedWithNextContext && saveOperation)
                    {
                        custResInfo.respTxt = string.Format(Utility.Utility.GetTemplateQuery("GetInput"),propToValidate);
                    }

                }
                while (custResInfo.next_response == "yes");
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: decideResponse");
                throw (e);
            }
            if (custResInfo.respTxt == null || custResInfo.respTxt == "")
            {
                string component = "Component : " + custResInfo.ComponentName;
                ConnectionManager.InsertTransaction(custResInfo.ChannelId, component, custResInfo.responseContext, custResInfo.botName, userId,Intent, jsonstring, intentScore);
            }
            else
            {
                ConnectionManager.InsertTransaction(custResInfo.ChannelId, custResInfo.respTxt, custResInfo.responseContext, custResInfo.botName,userId, Intent, jsonstring, intentScore);
            }
            //ConnectionManager.InsOrUpdateDfd_Orders(custResInfo.firstName, custResInfo.lastName, channelId, custResInfo.pnr, userId);
            
            return custResInfo;
        }
    }
}