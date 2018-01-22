using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace SampleBotTemplate
{
    public static class FetchResponseForIntent
    {

        public static string strRet = string.Empty;
        public static string strTemplate = string.Empty;
        public static string strWeb = string.Empty;
        public static string BotJson;
        public static string botName = ConfigurationManager.AppSettings["BotName"];

        public static CustomerResponse GetResponse(CustomerResponse custResp)
        {
            try
            {
                if (custResp.responseContext == null || custResp.responseContext == ""||custResp.responseContext=="None")
                {
                    if (botName == "botcreator") custResp.responseContext = "welcomenote";
                    else custResp.responseContext = "None";
                }

                //Get Response details from the DB
                custResp = ConnectionManager.GetResponseInfoForIntent(botName, custResp);
                if (custResp != null)
                {
                    custResp.IncomingContext = custResp.responseContext;
                    custResp.RespTxtCopy = custResp.respTxt;
                    string strResponseType = custResp.ResponseType.ToUpper();

                    if (strResponseType == "COMPONENT")
                    {
                        custResp.ComponentName = custResp.responseContext.ToLower() == "none" ? custResp.Comp_Name : custResp.responseContext;
                        custResp.RespTxtCopy = custResp.responseContext.ToLower() == "none" ? custResp.Comp_Name : custResp.responseContext;
                        custResp.componentInput = custResp.respTxt;
                    }
                   
                    custResp.next_response = "no";// r1.next_response;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: FetchResponseForIntent ");
                throw e;

            }
            return custResp;
        }
    }
}
