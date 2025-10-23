using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Serves as an departure flight record within the Brisbane Airport system.
    /// </summary>
    internal class DepartureFlight
    {
        public string FlightId { get; set; } = "";
        public string Airline { get; set; } = "";
        public string City { get; set; } = "";
        public string PlaneId { get; set; } = "";
        public DateTime Time { get; set; }
        public int Price { get; set; }   // ← add this
    }
}
