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
    public class FBComponents : BaseComponents
    {
        public override CustomerResponse SelectComponentAsync(CustomerResponse custResInfo, string UserId, string inputMessage)
        {

            try
            {

                switch (custResInfo.ComponentName.ToLower())
                {
                    case "bookingyesno":
                        custResInfo = getNameYesNo(custResInfo, UserId);
                        break;
                }
            }

            catch (Exception e)
            {

                Trace.TraceError("Error:" + e + " Method: SkypeComponents ");
                custResInfo.respTxt = "There is an error in Mobile API. Please come back later.";
            }
            return custResInfo;
        }

        private CustomerResponse getNameYesNo(CustomerResponse custResInfo, string UserId)
        {
            string fbAccessToken = ConfigurationManager.AppSettings["fbAccessToken"];
            Customer custInfo = new Customer();
            FlightStatus fltStatus = new FlightStatus();
            BookingStatus bookingStatus = new BookingStatus();
            Cards card = new Cards();
            try
            {
                string URL = "https://graph.facebook.com/v2.6/" + UserId + "?fields=first_name,last_name,profile_pic,locale,timezone,gender&access_token=" + fbAccessToken + "";
                string Result = BaseService.GetNameFromFacebook("", URL);
                custResInfo = ParserForFacebook.getvalues(Result, custResInfo);
                card.HeroTitle = custResInfo.firstName + " " + custResInfo.lastName;
                card.HeroSubtitle = "Is this the name on your booking? Please Confirm with Yes or No";
                card.buttonCount = 2;
                card.buttonTitle[0] = "Yes";
                card.buttonType[0] = "postBack";
                card.buttonValue[0] = "Yes";
                card.buttonTitle[1] = "No";
                card.buttonType[1] = "postBack";
                card.buttonValue[1] = "No";
                custResInfo = TemplateResponseTest.GetHeroCard(custResInfo, card, UserId);
                custResInfo.respTxt = null;
                ConnectionManager.UpdateFirstName(custResInfo.firstName, UserId);
                ConnectionManager.UpdateLastName(custResInfo.lastName, UserId);
            }
            catch (Exception e)
            {

                Trace.TraceError("Error:" + e + " Method: SkypeComponents ");

            }
            return custResInfo;
        }
    }
}