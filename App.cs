using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// The central application state.
    /// Holds:
    /// - The user repository and the currently logged-in user.
    /// - The master lists of Arrival and Departure flights.
    /// - Four booking maps (Traveller vs FrequentFlyer × Arrival vs Departure).
    /// </summary>
    internal class App
    {
        public UserRepository Users { get; } = new(); // All registered users (travellers, frequent flyers, and flight managers).
        public UserBaseClass? CurrentUser { get; set; } // The user who is currently authenticated, or null when logged out.
        public List<ArrivalFlight> ArrivalFlights { get; } = new(); 
        public List<DepartureFlight> DepartureFlights { get; } = new();

        public Dictionary<string, (ArrivalFlight flight, int row, char col)> TravellerArrivalBookings
            = new Dictionary<string, (ArrivalFlight flight, int row, char col)>();

        public Dictionary<string, (ArrivalFlight flight, int row, char col)> FrequentArrivalBookings
            = new Dictionary<string, (ArrivalFlight flight, int row, char col)>();

        public Dictionary<string, (DepartureFlight flight, int row, char col)> TravellerDepartureBookings
            = new Dictionary<string, (DepartureFlight flight, int row, char col)>();

        public Dictionary<string, (DepartureFlight flight, int row, char col)> FrequentDepartureBookings
            = new Dictionary<string, (DepartureFlight flight, int row, char col)>();

        /// <summary>
        /// Application loop: start on the Startup screen and keep transitioning
        /// until a screen returns <c>null</c>. Each screen receives the shared
        /// <see cref="App"/> instance so it can read/update state.
        /// </summary>

        public void Run()
        {
            Screen? s = new StartupScreen(true);
            while (s != null)
                s = s.Show(this);
        }
    }
}