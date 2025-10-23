
using Brisbane_Airport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Menu for a logged-in traveller. This class supports viewing details, changing password,
    /// booking arrival/departure flights, and viewing booked flight tickets.
    /// </summary>
    internal class TravellerMenuScreen : Screen
    {
        private readonly Traveller _traveller;

        public TravellerMenuScreen(Traveller traveller) => _traveller = traveller;

        public override Screen Show(App app)
        {
            Console.WriteLine("Traveller Menu.");
            Console.WriteLine("Please make a choice from the menu below:");
            Console.WriteLine("1. See my details.");
            Console.WriteLine("2. Change password.");
            Console.WriteLine("3. Book an arrival flight.");
            Console.WriteLine("4. Book a departure flight.");
            Console.WriteLine("5. See flight details.");
            Console.WriteLine("6. Logout.");
            Console.WriteLine("Please enter a choice between 1 and 6:");

            string choice = Console.ReadLine() ?? "";

            // 1) See details
            if (choice == "1")
            {
                Console.WriteLine("Your details.");
                Console.WriteLine($"Name: {_traveller.Name}");
                Console.WriteLine($"Age: {_traveller.Age}");
                Console.WriteLine($"Mobile phone number: {_traveller.Mobile}");
                Console.WriteLine($"Email: {_traveller.Email}");
                return this;
            }

            // 2) Change password
            if (choice == "2")
            {
                while (true)
                {
                    Console.WriteLine("Please enter your current password.");
                    string currentPwd = Console.ReadLine() ?? "";
                    if (currentPwd == _traveller.Password) break;

                    PrintPwdMismatch();
                }

                Console.WriteLine("Please enter your new password.");
                while (true)
                {
                    string newPwd = Console.ReadLine() ?? "";
                    if (IsValidPassword(newPwd))
                    {
                        _traveller.ChangePassword(newPwd);
                        break;
                    }

                    PrintInvalidPwd();
                }

                return this;
            }

            // 3) Book arrival
            if (choice == "3")
            {
                BookArrivalFlight(app, _traveller);
                return this;
            }

            // 4) Book departure
            if (choice == "4")
            {
                BookDepartureFlight(app, _traveller);
                return this;
            }

            // 5) See flight details
            if (choice == "5")
            {
                ShowMyFlightDetails(app, _traveller);
                return this;
            }

            // 6) Logout
            if (choice == "6") return new StartupScreen(false);

            return this;
        }

        // Helpers Functions
        private static bool IsValidPassword(string pwd) =>
            !string.IsNullOrEmpty(pwd) && pwd.Length >= 8 &&
            pwd.Any(char.IsUpper) && pwd.Any(char.IsLower) && pwd.Any(char.IsDigit);

        private static void PrintPwdMismatch()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Entered password does not match existing password.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static void PrintInvalidPwd()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Supplied password is invalid.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static void PrintOutOfRangeError()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Supplied value is out of range.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static void PrintSeatRowError()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Supplied seat row is invalid.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static void PrintSeatColError()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Supplied seat column is invalid.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static void PrintSeatOccupiedError()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Seat is already occupied.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static int ReadChoiceWithErrors(int max)
        {
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int v) && v >= 1 && v <= max) return v;

                PrintOutOfRangeError();
                Console.WriteLine($"Please enter a choice between 1 and {max}:");
            }
        }

        private static int ReadSeatRow()
        {
            Console.WriteLine("Please enter in your seat row between 1 and 10:");
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int r) && r >= 1 && r <= 10) return r;

                PrintSeatRowError();
                Console.WriteLine("Please enter in your seat row between 1 and 10:");
            }
        }

        private static char ReadSeatCol()
        {
            Console.WriteLine("Please enter in your seat column between A and D:");
            while (true)
            {
                string s = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                if (s.Length == 1 && s[0] >= 'A' && s[0] <= 'D') return s[0];

                PrintSeatColError();
                Console.WriteLine("Please enter in your seat column between A and D:");
            }
        }

        private static bool IsArrivalSeatTaken(App app, string flightId, int row, char col) =>
            app.TravellerArrivalBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col)
         || app.FrequentArrivalBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);

        private static bool IsDepartureSeatTaken(App app, string flightId, int row, char col) =>
            app.TravellerDepartureBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col)
         || app.FrequentDepartureBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);


        // Booking flows
        private void BookArrivalFlight(App app, Traveller t)
        {
            if (app.TravellerArrivalBookings.ContainsKey(t.Email))
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - You already have an arrival flight. You can not book another.");
                Console.WriteLine("#####");
                return;
            }

            var flights = app.ArrivalFlights.OrderBy(f => f.Time).ToList();

            bool hasDeparture = app.TravellerDepartureBookings.TryGetValue(t.Email, out var existingDeparture);
            DateTime? mustBeBefore = hasDeparture ? existingDeparture.flight.Time : (DateTime?)null;

            int idx;
            while (true)
            {
                Console.WriteLine("Please enter the arrival flight:");
                for (int i = 0; i < flights.Count; i++)
                    Console.WriteLine($"{i + 1}. Flight {flights[i].FlightId} operated by {flights[i].Airline} arriving at {flights[i].Time:HH:mm dd/MM/yyyy} from {flights[i].City} on plane {flights[i].PlaneId}.");
                Console.WriteLine($"Please enter a choice between 1 and {flights.Count}:");

                idx = ReadChoiceWithErrors(flights.Count);
                var candidate = flights[idx - 1];

                if (mustBeBefore.HasValue && candidate.Time >= mustBeBefore.Value)
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - The arrival time must be before the departure time.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                break;
            }

            var selectedArrival = flights[idx - 1];

            int rowA; char colA;
            while (true)
            {
                rowA = ReadSeatRow();
                colA = ReadSeatCol();

                if (IsArrivalSeatTaken(app, selectedArrival.FlightId, rowA, colA))
                {
                    PrintSeatOccupiedError();
                    continue;
                }
                break;
            }

            app.TravellerArrivalBookings[t.Email] = (selectedArrival, rowA, colA);
            Console.WriteLine($"Congratulations. You have booked flight {selectedArrival.FlightId} from {selectedArrival.City} arriving at {selectedArrival.Time:HH:mm dd/MM/yyyy} and are seated in {rowA}:{colA}.");
        }

        private void BookDepartureFlight(App app, Traveller t)
        {
            if (app.TravellerDepartureBookings.ContainsKey(t.Email))
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - You already have a departure flight. You can not book another.");
                Console.WriteLine("#####");
                return;
            }

            var flights = app.DepartureFlights.OrderBy(f => f.Time).ToList();

            bool hasArrival = app.TravellerArrivalBookings.TryGetValue(t.Email, out var existingArrival);
            DateTime? mustBeAfter = hasArrival ? existingArrival.flight.Time : (DateTime?)null;

            int idx;
            while (true)
            {
                Console.WriteLine("Please enter the departure flight:");
                for (int i = 0; i < flights.Count; i++)
                    Console.WriteLine($"{i + 1}. Flight {flights[i].FlightId} operated by {flights[i].Airline} departing at {flights[i].Time:HH:mm dd/MM/yyyy} to {flights[i].City} on plane {flights[i].PlaneId}.");
                Console.WriteLine($"Please enter a choice between 1 and {flights.Count}:");

                idx = ReadChoiceWithErrors(flights.Count);
                var candidate = flights[idx - 1];

                if (mustBeAfter.HasValue && candidate.Time <= mustBeAfter.Value)
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - The departure time must be after the arrival time.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                break;
            }

            var selectedDeparture = flights[idx - 1];

            int rowD; char colD;
            while (true)
            {
                rowD = ReadSeatRow();
                colD = ReadSeatCol();

                if (IsDepartureSeatTaken(app, selectedDeparture.FlightId, rowD, colD))
                {
                    PrintSeatOccupiedError();
                    continue;
                }
                break;
            }

            app.TravellerDepartureBookings[t.Email] = (selectedDeparture, rowD, colD);
            Console.WriteLine($"Congratulations. You have booked flight {selectedDeparture.FlightId} to {selectedDeparture.City} departing at {selectedDeparture.Time:HH:mm dd/MM/yyyy} and are seated in {rowD}:{colD}.");
        }

        // Details
        private void ShowMyFlightDetails(App app, Traveller t)
        {
            bool hasArrival = app.TravellerArrivalBookings.TryGetValue(t.Email, out var a);
            bool hasDeparture = app.TravellerDepartureBookings.TryGetValue(t.Email, out var d);

            if (!hasArrival && !hasDeparture)
            {
                Console.WriteLine("There are no flights to show.");
                return;
            }

            Console.WriteLine($"Showing flight details for {t.Name}:");
            if (hasArrival)
                Console.WriteLine($"Arrival Flight: Flight {a.flight.FlightId} from {a.flight.City} arriving at {a.flight.Time:HH:mm dd/MM/yyyy} in seat {a.row}:{a.col}.");
            if (hasDeparture)
                Console.WriteLine($"Departure Flight: Flight {d.flight.FlightId} to {d.flight.City} departing at {d.flight.Time:HH:mm dd/MM/yyyy} in seat {d.row}:{d.col}.");
        }
    }
}