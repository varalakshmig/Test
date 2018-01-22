using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Reflection;
namespace SampleBotTemplate.Utility
{
    public static class Utility
    {
        public static string contextResetIntent = ConfigurationManager.AppSettings["contextResetIntent"];
        public static string errorString = string.Empty;
        public static string twilioNumberForSMS = ConfigurationManager.AppSettings["twilioNumberForSMS"];
        public static string SMSNotificationMessage = ConfigurationManager.AppSettings["SMSNotificationMessage"];
        public static string sendTextUrl = ConfigurationManager.AppSettings["sendTextUrl"];

        public static string ReplyForCommonIntents(string intent)
        {
            string resptxt = string.Empty;
            if (intent == "stop")
            {

            }
            else if (intent.ToUpper() == "HELP")
            {
                resptxt = "Type MENU to get the menu, STOP to stop receiving message, CONTACT for delta contact details.";
            }
            else if (intent.ToUpper() == "CONTACT")
            {
                resptxt = "Contact 18002211212 for domestic reservation, 18887503284 for delta.com support.";
            }
            else if (intent.ToUpper() == "THANKS")
            {
                resptxt = "You are welcome. See you onboard.";
            }
            return resptxt;
        }
        public static string UpdateComponentData(string inputMessage,string componentName,string userID,string outputMessage,Dictionary<string,string> entityDictionary)
        {
            if (componentName.ToLower() == "getfirstname")
            {
                ConnectionManager.UpdateFirstName(inputMessage, userID);
            }

            if (componentName.ToLower() == "getlastname")
            {
                ConnectionManager.UpdateLastName(inputMessage, userID);
            }

            if (componentName.ToLower() == "getpnr")
            {
                ConnectionManager.UpdatePNR(inputMessage, userID);
            }
            if (componentName.ToLower() == "getflightno")
            {
                ConnectionManager.UpdateFlightNo(inputMessage, userID);
            }
            if (componentName.ToLower() == "qnamaker")
            {
                if (entityDictionary.Count > 1)
                {
                    string keyword = entityDictionary["deltakeyword"];
                    string definition = entityDictionary["definition"];
                    if (definition.ToLower().Contains("null"))
                    {
                        definition = null;
                    }
                    Trace.TraceError("Entity. Definition-" + definition + " and Keyword-" + keyword + "");
                    ConnectionManager.InsOrUpdateDefinitions(keyword, definition);
                    outputMessage = "Thanks for making Delta Speak better. We have noted your change and it will be reflected once its approved.";
                }
                else if (inputMessage.ToLower().Contains("add") && inputMessage.ToLower().Contains("#"))
                {
                    string[] separator = { "add","Add","ADD","#" };
                    string[] stringarray = inputMessage.Split(separator, StringSplitOptions.None);
                    string keyword = stringarray[1].Trim();
                    string definition = stringarray[2].Trim();
                    if (definition.ToLower().Contains("null"))
                    {
                        definition = null;
                    }
                    Trace.TraceError("Split. Definition-" + definition + " and Keyword-" + keyword + "");
                    ConnectionManager.InsOrUpdateDefinitions(keyword, definition);
                    outputMessage = "Thanks for making Delta Speak better. We have noted your change and it will be reflected once its approved.";
                }
                //else
                //{
                //    outputMessage = "Sorry. we are not able to process this input format. Please provide the input in the proper format mentioned";
                //}
                //insert

            }
            if (componentName.ToLower() == "sendtextnotification")
            {

                Message obj = new Message();
                obj.fromNumber = twilioNumberForSMS;
                obj.messageToUser = ConnectionManager.GetResponseForRepeatComponent(userID, "sendtextnotification").Replace("Do you want me to send you the instructions through SMS?", "");
                if (inputMessage.ToLower().Contains("yes"))
                    obj.toNumber = userID;
                else
                {
                    if (inputMessage.StartsWith("+1"))
                        obj.toNumber = inputMessage;
                    else
                        obj.toNumber = "+1" + inputMessage;
                }
                SendSMSNotification(obj, sendTextUrl);
                //outputMessage = "We have sent a text notification to your number. Is there anything else i can help you with?";
            }
            return outputMessage;
        }
        public static void ResetContext(string intent,string userId)
        {
            if(intent.ToUpper() == contextResetIntent.ToUpper())
            ConnectionManager.DeleteResponseContextForBot(userId);
        }
        //ValidateUser - To check whether an user is valid or not 
        //This has to be tweaked based on the bot requirement
        public static string ValidateUser(string userId)
        {

            //int selind = Convert.ToInt32(customerData.Substring(2, 1));
            //Char validIndicator = ConnectionManager.fltdtchk(userId);
            //if (validIndicator == 'E')
            //{
            //    errorString = "Unfortunately, the deadline to indicate your meal preference prior to your upcoming Delta One flight has passed. However you can still select onboard.";
            //    return errorString;
            //}
            //else if(selind==1)
            //{
            //    errorString = "We have already noted your meal selection and look forward to seeing you onboard.";
            //    return errorString;
            //}
            return errorString;
        }
        public static CustomerResponse RecogniseUser(string userid,CustomerResponse custInfo)
        {
            return ConnectionManager.VerifyCustomer(userid, custInfo);
        }
        public static void SendSMSNotification(Message obj, string url)
        {
            string description = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json";
            string jsonOrder = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonOrder);
            request.ContentLength = data.Length;
            //post data
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            //get response
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.
            if (((HttpWebResponse)response).StatusDescription == "OK")
                Trace.TraceError("SMS sent successfully to the user: " + obj.messageToUser);
            //else
                //Trace.TraceError("SMS sending failed " + ((HttpWebResponse)response).);
        }
        public static string FromDictionaryToJson(this Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, kvp.Value));
            return string.Concat("{", string.Join(",", kvs), "}");
            
        }
        public class Message
        {
            public string fromNumber { get; set; }
            public string toNumber { get; set; }
            public string messageToUser { get; set; }
        }
        public static bool CheckIfBotNameExist(string botname)
        {
            return ConnectionManager.CheckBot(botname);
        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }
        public static string GetTemplateQuery(string type)
        {
            string file = System.Web.HttpContext.Current.Server.MapPath("~/Template.json");
            string Json = System.IO.File.ReadAllText(file);
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var personlist = ser.Deserialize<List<Template>>(Json);
            foreach (Template t in personlist)
            {
                if (t.type.ToString().ToLower() == type.ToLower())
                {
                    return t.title.ToString();
                }                
            }
            return "";
        }
        //public static List<FlightTrip> GetFlightSearchResults(string source, string destination, string date, string price)
        //{
        //    List<FlightTrip> flights = new List<FlightTrip>();
        //    QPXExpressService service = new QPXExpressService(new BaseClientService.Initializer()
        //    {
        //        ApiKey = "AIzaSyCH2nSAhL9HdzaqXmAUwx_px0pHqa42U9I",
        //        ApplicationName = "Reverse Query Bot",                
        //    });
        //    TripsSearchRequest x = new TripsSearchRequest();
        //    x.Request = new TripOptionsRequest();
        //    x.Request.Passengers = new PassengerCounts { AdultCount = 2 };
        //    x.Request.Slice = new System.Collections.Generic.List<SliceInput>();
        //    x.Request.Slice.Add(new SliceInput() { Origin = source, Destination = destination, Date = date }); //"2015-10-29"
        //    x.Request.Solutions = 10;
        //    x.Request.MaxPrice = "USD" + price;
        //    var result = service.Trips.Search(x).Execute();
        //    int i = 1;
        //    foreach (var trip in result.Trips.TripOption)
        //    {
        //        FlightTrip Ftrip = new FlightTrip();
        //        Ftrip.FlightNumber = trip.Slice.FirstOrDefault().Segment.FirstOrDefault().Flight.Number;
        //        Ftrip.Duration = trip.Slice.FirstOrDefault().Duration.ToString();
        //        Ftrip.Cabin = trip.Slice.FirstOrDefault().Segment.FirstOrDefault().Cabin;
        //        Ftrip.Price = trip.Pricing.FirstOrDefault().BaseFareTotal.ToString();
        //        flights.Add(Ftrip);
        //    }
        //    return flights;
        //}
        public static string IPRequestHelper(string url)
        {
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();

            StreamReader responseStream = new StreamReader(objResponse.GetResponseStream());
            string responseRead = responseStream.ReadToEnd();

            responseStream.Close();
            responseStream.Dispose();

            return responseRead;
        }
        public static IpProperties GetCountryByIP(string ipAddress)
        {
            string ipResponse = IPRequestHelper("http://ip-api.com/xml/" + ipAddress);
            using (TextReader sr = new StringReader(ipResponse))
            {
                using (System.Data.DataSet dataBase = new System.Data.DataSet())
                {
                    IpProperties ipProperties = new IpProperties();
                    dataBase.ReadXml(sr);
                    ipProperties.Status = dataBase.Tables[0].Rows[0][0].ToString();
                    ipProperties.Country = dataBase.Tables[0].Rows[0][1].ToString();
                    ipProperties.CountryCode = dataBase.Tables[0].Rows[0][2].ToString();
                    ipProperties.Region = dataBase.Tables[0].Rows[0][3].ToString();
                    ipProperties.RegionName = dataBase.Tables[0].Rows[0][4].ToString();
                    ipProperties.City = dataBase.Tables[0].Rows[0][5].ToString();
                    ipProperties.Zip = dataBase.Tables[0].Rows[0][6].ToString();
                    ipProperties.Lat = dataBase.Tables[0].Rows[0][7].ToString();
                    ipProperties.Lon = dataBase.Tables[0].Rows[0][8].ToString();
                    ipProperties.TimeZone = dataBase.Tables[0].Rows[0][9].ToString();
                    ipProperties.ISP = dataBase.Tables[0].Rows[0][10].ToString();
                    ipProperties.ORG = dataBase.Tables[0].Rows[0][11].ToString();
                    ipProperties.AS = dataBase.Tables[0].Rows[0][12].ToString();
                    ipProperties.Query = dataBase.Tables[0].Rows[0][13].ToString();

                    return ipProperties;
                }
            }
        }
        public static string IPify()
        {
            WebClient webClient = new WebClient();
            string publicIp = webClient.DownloadString("https://api.ipify.org");
            Console.WriteLine("Ip address", publicIp);
            var ipResponse = GetCountryByIP(publicIp);
            Console.WriteLine(ipResponse.City);
            return ipResponse.City;
        }
        public static List<Flight> SearchFlightsSOAService(string source, string destination, string date)
        {
            //TO BE IMPLEMENTED
            List<Flight> flightlist = new List<Flight>();
            return flightlist;
        }
    }
    public class Template
    {
        public string title { get; set; }
        public string type { get; set; }
    }
    public class IpProperties
    {
        public string Status { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string TimeZone { get; set; }
        public string ISP { get; set; }
        public string ORG { get; set; }
        public string AS { get; set; }
        public string Query { get; set; }
    }
}