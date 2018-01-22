using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class BookingStatus
    {
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
        public string seatClass { get; set; }
    }
}