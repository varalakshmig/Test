using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class ReverseQuery
    {
        public string Stations { get; set; }
        public string Type { get; set; }
        public string Price { get; set; }
    }
    public class FlightTrip
    {
        public string FlightNumber { get; set; }
        public string Duration { get; set; }
        public string Cabin { get; set; }
        public string Price { get; set; }
    }
}