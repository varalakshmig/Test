using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class Flight
    {
        //public string FltNo { get; set; }
        //public string From { get; set; }
        //public string To { get; set; }
        //public string DepartDate { get; set; }
        //public string DepartTime { get; set; }
        //public string ArrivalDate { get; set; }
        //public string ArrivalTime { get; set; }
        //public string Carrier { get; set; }
        //public string AC {get;set;}
        //public string DeltaOne { get; set; }
        //public string SeatsAvailableInFirstClass { get; set; }
        //public string SeatsAvailableInPremiumSelect { get; set; }
        //public string SeatsAvailableInComfort { get; set; }
        //public string SeatsAvailableInMainCabin { get; set; }
        //public string Destination { get; set; }

        public string flightNumber { get; set; }
        public string departureDateTime { get; set; }
        public string arrivalDateTime { get; set; }
        public string arrivalCityName { get; set; }
    }
}