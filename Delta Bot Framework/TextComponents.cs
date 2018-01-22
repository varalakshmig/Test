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
using System.Globalization;
using SampleBotTemplate;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents;
using SampleBotTemplate.DataAccess;
namespace SampleBotTemplate
{
    public class TextComponents : BaseComponents
    {
        public override CustomerResponse SelectComponentAsync(CustomerResponse custResInfo, string UserId, string inputMessage)
        {
            try
            {
                switch (custResInfo.ComponentName.ToLower())
                {

                    case "sendtextnotification":
                        custResInfo = SendTextNotification(custResInfo, UserId);
                        break;
                    case "qnamaker":
                        custResInfo = QnAMaker(custResInfo, UserId);
                        break;
                    case "searchacronym":
                        custResInfo = SearchAcronym(custResInfo).Result;
                        break;
                    case "addnewkeyword":
                        custResInfo = AddNewKeyword(custResInfo).Result;
                        break;
                    case "getholidaylist":
                        custResInfo = GetHolidayList(custResInfo).Result;
                        break;


                }
            }

            catch (Exception e)
            {

                Trace.TraceError("Error:" + e + " Method: TextComponents ");
                custResInfo.respTxt = "Work in Progress, will be back up in sometime";
            }
            return custResInfo;
        }

        private async Task<CustomerResponse> SearchAcronym(CustomerResponse custResInfo)
        {
            string outStr = string.Empty;
            try
            {
                List<string> res = await CosmosDBConnectionManager.SearchAcronym(custResInfo.Entity == null ? custResInfo.InputMessage : custResInfo.Entity);
                if (res.Count > 1)
                {
                    outStr = "I have more than one Definition for the item you requested. Please find them below \n\n";
                    foreach (string str in res)
                    {
                        outStr += str + "\n\n";
                    }
                }
                else if (res.Count == 1)
                {
                    outStr = res[0].ToString();
                }
                else outStr = "Deltaspeaks is unfamilier with this item. If you discover its meaning, please contribute to our collection by responding in this format. ADD keyword#Definition#Employee Number. Also, Please use the below formats to get your answers.\n 1. For  Airport Code Type Airport Acronym/Abbreviation For Eg. ATL \n 2. For  Delta Key word, Type Delta Acronym/Abbreviation, For Eg, ACS \n 3. For  Delta Holiday List, Type Delta Holiday List to get full list \n 4. For specific holiday date, type holiday Name. For eg. Christmas";
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: TextComponents - SearchAcronym");
                custResInfo.respTxt = "Work in Progress, will be back up in sometime";
            }
            custResInfo.respTxt = outStr;
            return custResInfo;
        }

        private async Task<CustomerResponse> AddNewKeyword(CustomerResponse custResInfo)
        {
            if (custResInfo.InputMessage.ToLower().Contains("add") && custResInfo.InputMessage.ToLower().Contains("#"))
            {
                try
                {
                    string[] splitInputs = custResInfo.InputMessage.Split('#');
                    string keyword = splitInputs[0].Split(' ')[1];
                    string definition = splitInputs[1];
                    string empno = splitInputs[2];
                    try
                    {
                        bool isAdded = await CosmosDBConnectionManager.AddNewKeyword(keyword, definition, empno);
                        if (isAdded) custResInfo.respTxt = "Thanks for making Delta Speak better. We have noted your change and it will be reflected once its approved.";
                        else custResInfo.respTxt = "There is an Error in adding item to our collection. Please contact Admin.";
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Error:" + e + " Method: TextComponents - AddNewKeyword");
                        custResInfo.respTxt = "Work in Progress, will be back up in sometime";
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error:" + e + " Method: TextComponents - AddNewKeyword");
                    custResInfo.respTxt = "Invalid Format. Please use the below formats to get your answers.\n\n 1.For  Airport Code Type Airport Acronym / Abbreviation For Eg. ATL \n\n 2.For  Delta Key word, Type Delta Acronym / Abbreviation, For Eg, ACS \n\n 3.For  Delta Holiday List, Type Delta Holiday List to get full list \n\n 4.For specific holiday date, type holiday Name.For eg. Christmas";
                }
            }
            else
            {
                custResInfo.respTxt = "Invalid Format. Please use the below formats to get your answers.\n 1.For  Airport Code Type Airport Acronym / Abbreviation For Eg. ATL \n 2.For  Delta Key word, Type Delta Acronym / Abbreviation, For Eg, ACS \n 3.For  Delta Holiday List, Type Delta Holiday List to get full list \n 4.For specific holiday date, type holiday Name.For eg. Christmas";
            }
            return custResInfo;
        }

        private async Task<CustomerResponse> GetHolidayList(CustomerResponse custResInfo)
        {
            try
            {
                custResInfo.respTxt = await CosmosDBConnectionManager.GetHolidayList();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error:" + e + " Method: TextComponents - GetHolidayList");
                custResInfo.respTxt = "Work in Progress, will be back up in sometime";
            }
            return custResInfo;
        }

    }
}