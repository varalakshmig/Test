using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class Service : BaseService
    {

        public static string URL;
        public static string XML;
        public static string JsonResult;

        public static CustomerResponse selectService(CustomerResponse custResInfo, string UserId,string inputMessage)
        {
            try
            {
                if (custResInfo.serviceName == "FindMealSelection")
                {
                    string resptxt = string.Empty;
                    string menutext = VerifySelection(inputMessage);
                    Char conf = Convert.ToChar(menutext.Substring(0, 1));
                    String menu = menutext.Substring(1);
                    UpdateSelection(menu);
                    if (conf == 'Y')
                    {
                        custResInfo.responseContext = "SelectionText";
                        custResInfo.intent = "yesintent";
                        return custResInfo= ParserForResponseJson.GetResponse(custResInfo);
                    }
                    else if (conf == 'P')
                    {
                        custResInfo.respTxt = "Are you planning to set " + menu + " as your meal preference?";
                        custResInfo.OutgoingContext = "SelectionText";
                        return custResInfo;
                    }
                    else if (conf == 'N')
                    {
                        resptxt = "Unfortunately, you have made an invalid selection. Please indicate your meal preference by replying with 1,2,3 or 4.";
                        custResInfo.OutgoingContext = "SelectionService";
                        return custResInfo;

                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: Service ");
                throw (e);
            }
            return custResInfo;
        }
    }
}