using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleBotTemplate
{
    public class FlightStatus
    {
        public string airlineCode { set; get; }
        public string airlineName { set; get; }
        public string arrivalAirportCode { set; get; }
        public string arrivalAirportName { set; get; }
        public string arrivalCityName { set; get; }
        public string arrivalLocalTimeEstimatedActual { set; get; }
        public string arrivalLocalTimeEstimatedActualLabel { set; get; }
        public string arrivalLocalTimeScheduled { set; get; }
        public string arrivalTerminal { set; get; }
        public string arrivalTsoagLatitudeDecimal { set; get; }
        public string arrivalTsoagLongitudeDecimal { set; get; }
        public string cabinCodeBusinessInd { set; get; }
        public string cabinCodeComfortInd { set; get; }
        public string changeOfAircraft { set; get; }
        public string connectionCarrierOrCodeShareOperator { set; get; }
        public string departureAirportCode { set; get; }
        public string departureAirportName { set; get; }
        public string departureCityName { set; get; }
        public string departureLocalTimeEstimatedActual { set; get; }
        public string departureLocalTimeEstimatedActualLabel { set; get; }
        public string departureLocalTimeScheduled { set; get; }
        public string departureTerminal { set; get; }
        public string departureTsoagLatitudeDecimal { set; get; }
        public string departureTsoagLongitudeDecimal { set; get; }
        public string equipmentCodeCass { set; get; }
        public string equipmentCodeDelta { set; get; }
        public string equipmentCodeIndustry { set; get; }
        public string equipmentType { set; get; }
        public string flightDistance { set; get; }
        public string flightNumber { set; get; }
        public string flightPerformanceCancellationsStatus { set; get; }
        public string flightPerformanceOnTimeStatus { set; get; }
        public string flightPerformanceStatTO { set; get; }
        public string flightStateColor { set; get; }
        public string flightStateDescription { set; get; }
        public string marketAirlineCode { set; get; }
        public string marketAirlineName { set; get; }
        public string mealCabinCodeFirstBusinessClass { set; get; }
        public string mealDescriptionFirstBusinessClass { set; get; }
        public string movieShown { set; get; }
        public string samePlaneAirlineCode { set; get; }
        public string samePlaneArrivalAirportCode { set; get; }
        public string samePlaneDepartureAirportCode { set; get; }
        public string samePlaneDepartureAirportName { set; get; }
        public string samePlaneDepartureCityName { set; get; }
        public string samePlaneDepartureDateTime { set; get; }
        public string samePlaneFlightNumber { set; get; }
        public string samePlaneFlightOriginDate { set; get; }
        public string samePlaneShowFlightStatusLink { set; get; }
        public string serviceDisruption { set; get; }
        public string shipNumber { set; get; }
        public string travelTimeHours { set; get; }
        public string travelTimeMinutes { set; get; }
        public string tripNumber { set; get; }
        public string arrivalGate { set; get; }
        public string departureGate { get; set; }
        public string aircraft_type { get; set; }
        public string selectedFlightNo { get; set; }
    }
}