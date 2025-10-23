using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Serves as an arrival flight record within the Brisbane Airport system.
    /// </summary>
    internal class ArrivalFlight
    {
        public string FlightId { get; set; } = ""; // A unique identifier for the flight (e.g., JST150).
        public string Airline { get; set; } = ""; // The name of the airline operating the flight (e.g., Qantas, Jetstar).
        public string City { get; set; } = ""; // The city from which the flight is arriving.
        public string PlaneId { get; set; } = ""; // The identifier of the aircraft used for this flight (e.g., JST7A).
        public DateTime Time { get; set; } // The scheduled arrival date and time of the flight.
        public int Price { get; set; } // The base price of a ticket for this flight.
    }
}


