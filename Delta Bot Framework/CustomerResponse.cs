using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class CustomerResponse
    {
        public CustomerResponse()
        {
            buttonTitle = new string[10];
            buttonType = new string[10];
            buttonValue = new string[10];
        }
        public string attribute { get; set; }
        public string respTxt { get; set; }
        public string serviceName { get; set; }
        public string botName { get; set; }
        public string TemplateName { get; set; }
        public string ComponentName { get; set; }
        public string ind { get; set; }
        public string IncomingContext { get; set; }
        public string OutgoingContext { get; set; }
        public string ResponseType { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string pnr { get; set; }
        public string ResponseAction { get; set; }
        public string RespTxtCopy { get; set; }
        public string componentInput { get; set; }
        public string responseContext { get; set; }
        public string intent { get; set; }
        public Attachment plAttachment { get; set; }
        public List<Attachment> plAttachmentList { get; set; }
        public JObject facebookTemplate { get; set; }
        public string next_response { get; set; }
        public int buttonCount { get; set; }
        public string[] buttonTitle { get; set; }
        public string[] buttonType { get; set; }
        public string[] buttonValue { get; set; }
        public string HeroImg { get; set; }
        public string HeroTitle { get; set; }
        public string HeroSubtitle { get; set; }
        public string HeroText { get; set; }
        public string selectedIndex { get; set; }
        public string InputMessage { get; set; }
        public string arrivalTime { get; set; }
        public string departureTime { get; set; }
        public string departureCity { get; set; }
        public string arrivalCity { get; set; }
        public string selectedFlightNo { get; set; }
        public string flightStatus { get; set; }
        public string departureStation { get; set; }
        public string arrivalStation { get; set; }
        public string flightNumber { get; set; }
        public List<String> destinationlist { get; set; }
        public List<String> flightList { get; set; }
        public List<String> depaturedateslist { get; set; }
        public string seatNumber { get; set; }
        public List<String> buttons { get; set; } = new List<String>();
        public string ChannelId { get; set; }
        public List<ParserForResponseJson.lcards> CardList { get; set; } = new List<ParserForResponseJson.lcards>();
        public string id { get; set; }
        public string bagTagNumber { get; set; }
        public string documentid { get; set; }
        public string Entity { get; set; }
        public string languageSelected { get; set; }
        public List<String> languageList { get; set; } = new List<string>();
        public string language { get; set; }
        public Dictionary<string, string> EntityList = new Dictionary<string, string>();
        public List<Flight> Flights = new List<Flight>();
        public List<Flight> FilteredFlights = new List<Flight>();
        public string Comp_Name { get; set; }
        public static implicit operator CustomerResponse(string v)
        {
            throw new NotImplementedException();
        }
    }
}
