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

namespace SampleBotTemplate
{
    public class SkypeComponents : BaseComponents
    {
        public override CustomerResponse SelectComponentAsync(CustomerResponse custResInfo, string UserId, string inputMessage)
        {
            try
            {


                switch (custResInfo.ComponentName.ToLower())
                {
                  
                }
            }

            catch (Exception e)
            {

                Trace.TraceError("Error:" + e + " Method: SkypeComponents ");
                custResInfo.respTxt = "There is an error in Mobile API. Please come back later.";
            }
            return custResInfo;
        }


    }
}