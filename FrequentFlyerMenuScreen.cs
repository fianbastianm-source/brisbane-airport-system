using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Menu for a logged-in frequent flyer user. Adds points display and
    /// traveller-seat reallocation rules on top of traveller booking features.
    /// </summary>
    internal class FrequentFlyerMenuScreen : Screen
    {
        private readonly FrequentFlyer _ff;

        public FrequentFlyerMenuScreen(FrequentFlyer ff) => _ff = ff;

        public override Screen? Show(App app)
        {
            Console.WriteLine("Frequent Flyer Menu.");
            Console.WriteLine("Please make a choice from the menu below:");
            Console.WriteLine("1. See my details.");
            Console.WriteLine("2. Change password.");
            Console.WriteLine("3. Book an arrival flight.");
            Console.WriteLine("4. Book a departure flight.");
            Console.WriteLine("5. See flight details.");
            Console.WriteLine("6. See frequent flyer points.");
            Console.WriteLine("7. Logout.");
            Console.WriteLine("Please enter a choice between 1 and 7:");

            string choice = Console.ReadLine() ?? "";

            // 1) See details
            if (choice == "1")
            {
                Console.WriteLine("Your details.");
                Console.WriteLine($"Name: {_ff.Name}");
                Console.WriteLine($"Age: {_ff.Age}");
                Console.WriteLine($"Mobile phone number: {_ff.Mobile}");
                Console.WriteLine($"Email: {_ff.Email}");
                Console.WriteLine($"Frequent flyer number: {_ff.FlyerNumber}");
                Console.WriteLine($"Frequent flyer points: {_ff.FlyerPoints:N0}");
                return this;
            }

            // 2) Change password
            if (choice == "2")
            {
                Console.WriteLine("Please enter your current password.");
                string cur = Console.ReadLine() ?? "";
                if (cur != _ff.Password)
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Incorrect Password.");
                    Console.WriteLine("#####");
                    return this;
                }

                Console.WriteLine("Please enter your new password.");
                Console.WriteLine("Your password must:");
                Console.WriteLine("- be at least 8 characters long");
                Console.WriteLine("- contain a number");
                Console.WriteLine("- contain a lowercase letter");
                Console.WriteLine("- contain an uppercase letter");

                while (true)
                {
                    string np = Console.ReadLine() ?? "";
                    if (IsValidPassword(np))
                    {
                        _ff.ChangePassword(np);
                        Console.WriteLine("Your password has been successfully changed.");
                        break;
                    }

                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied password is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    Console.WriteLine("Please enter your new password.");
                }

                return this;
            }

            // 3) Book arrival
            if (choice == "3") { BookArrivalFlight(app, _ff); return this; }

            // 4) Book departure
            if (choice == "4") { BookDepartureFlight(app, _ff); return this; }

            // 5) See flight details
            if (choice == "5") { ShowMyFlightDetails(app, _ff); return this; }

            // 6) See points
            if (choice == "6")
            {
                Console.WriteLine($"Your current points are: {_ff.FlyerPoints.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture)}.");

                bool hasArrival = app.FrequentArrivalBookings.TryGetValue(_ff.Email, out var a);
                bool hasDeparture = app.FrequentDepartureBookings.TryGetValue(_ff.Email, out var d);

                int arrivalPts = 0, departurePts = 0;

                if (hasArrival)
                {
                    arrivalPts = CityPoints(a.flight.City);
                    Console.WriteLine($"Your points from your arrival flight will be : {arrivalPts:N0}.");
                }

                if (hasDeparture)
                {
                    departurePts = CityPoints(d.flight.City);
                    Console.WriteLine($"Your points from your departure flight will be: {departurePts:N0}.");
                }

                if (hasArrival || hasDeparture)
                {
                    int total = _ff.FlyerPoints + arrivalPts + departurePts;
                    string noun = (hasArrival && hasDeparture) ? "flights" : "flight";
                    Console.WriteLine($"After completing your {noun} your new points will be: {total.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture)}.");
                }

                return this;
            }

            // 7) Logout
            if (choice == "7") return new StartupScreen(false);

            return this;
        }

        // Helpers Functions
        private static bool IsValidPassword(string pwd) =>
            !string.IsNullOrEmpty(pwd) && pwd.Length >= 8 &&
            pwd.Any(char.IsUpper) && pwd.Any(char.IsLower) && pwd.Any(char.IsDigit);

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

        // seat state helper functions
        private static bool IsArrivalSeatTakenByFF(App app, string flightId, int row, char col) =>
            app.FrequentArrivalBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);

        private static bool IsDepartureSeatTakenByFF(App app, string flightId, int row, char col) =>
            app.FrequentDepartureBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);

        // Detect Traveller holding a seat
        private static bool IsArrivalSeatHeldByTraveller(App app, string flightId, int row, char col) =>
            app.TravellerArrivalBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);

        private static bool IsDepartureSeatHeldByTraveller(App app, string flightId, int row, char col) =>
            app.TravellerDepartureBookings.Values.Any(b =>
                b.flight.FlightId == flightId && b.row == row && b.col == col);

        // Checks if anyone else holds a seat
        private static bool IsArrivalSeatTakenByAnyone(App app, string flightId, int row, char col) =>
            IsArrivalSeatTakenByFF(app, flightId, row, col) || IsArrivalSeatHeldByTraveller(app, flightId, row, col);

        private static bool IsDepartureSeatTakenByAnyone(App app, string flightId, int row, char col) =>
            IsDepartureSeatTakenByFF(app, flightId, row, col) || IsDepartureSeatHeldByTraveller(app, flightId, row, col);

        // 3) Book arrival flight
        private void BookArrivalFlight(App app, FrequentFlyer f)
        {
            if (app.FrequentArrivalBookings.ContainsKey(f.Email))
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - You already have an arrival flight. You can not book another.");
                Console.WriteLine("#####");
                return;
            }

            var flights = app.ArrivalFlights.OrderBy(fl => fl.Time).ToList();

            bool hasDeparture = app.FrequentDepartureBookings.TryGetValue(f.Email, out var existingDeparture);
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

            // Seat loop (row then column)
            int rowA; char colA;
            while (true)
            {
                rowA = ReadSeatRow();
                colA = ReadSeatCol();

                if (IsArrivalSeatTakenByFF(app, selectedArrival.FlightId, rowA, colA))
                {
                    PrintSeatOccupiedError();
                    continue;
                }

                // If a Traveller holds it, this will reassign that traveller
                if (IsArrivalSeatHeldByTraveller(app, selectedArrival.FlightId, rowA, colA))
                {
                    var kv = app.TravellerArrivalBookings.First(b =>
                        b.Value.flight.FlightId == selectedArrival.FlightId &&
                        b.Value.row == rowA && b.Value.col == colA);

                    var (newRow, newCol) = FindNextArrivalSeat(app, selectedArrival, rowA, colA);
                    app.TravellerArrivalBookings[kv.Key] = (selectedArrival, newRow, newCol);
                }

                break;
            }

            app.FrequentArrivalBookings[f.Email] = (selectedArrival, rowA, colA);
            Console.WriteLine($"Congratulations. You have booked flight {selectedArrival.FlightId} from {selectedArrival.City} arriving at {selectedArrival.Time:HH:mm dd/MM/yyyy} and are seated in {rowA}:{colA}.");
        }

        // 4) Book departure flight
        private void BookDepartureFlight(App app, FrequentFlyer f)
        {
            if (app.FrequentDepartureBookings.ContainsKey(f.Email))
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - You already have a departure flight. You can not book another.");
                Console.WriteLine("#####");
                return;
            }

            var flights = app.DepartureFlights.OrderBy(fl => fl.Time).ToList();

            bool hasArrival = app.FrequentArrivalBookings.TryGetValue(f.Email, out var existingArrival);
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

                if (IsDepartureSeatTakenByFF(app, selectedDeparture.FlightId, rowD, colD))
                {
                    PrintSeatOccupiedError();
                    continue;
                }

                if (IsDepartureSeatHeldByTraveller(app, selectedDeparture.FlightId, rowD, colD))
                {
                    var kv = app.TravellerDepartureBookings.First(b =>
                        b.Value.flight.FlightId == selectedDeparture.FlightId &&
                        b.Value.row == rowD && b.Value.col == colD);

                    var (newRow, newCol) = FindNextDepartureSeat(app, selectedDeparture, rowD, colD);
                    app.TravellerDepartureBookings[kv.Key] = (selectedDeparture, newRow, newCol);
                }

                break;
            }

            app.FrequentDepartureBookings[f.Email] = (selectedDeparture, rowD, colD);
            Console.WriteLine($"Congratulations. You have booked flight {selectedDeparture.FlightId} to {selectedDeparture.City} departing at {selectedDeparture.Time:HH:mm dd/MM/yyyy} and are seated in {rowD}:{colD}.");
        }

        // 5) See flight details
        private void ShowMyFlightDetails(App app, FrequentFlyer f)
        {
            bool hasArrival = app.FrequentArrivalBookings.TryGetValue(f.Email, out var a);
            bool hasDeparture = app.FrequentDepartureBookings.TryGetValue(f.Email, out var d);

            if (!hasArrival && !hasDeparture)
            {
                Console.WriteLine("There are no flights to show.");
                return;
            }

            Console.WriteLine($"Showing flight details for {f.Name}:");
            if (hasArrival)
                Console.WriteLine($"Arrival Flight: Flight {a.flight.FlightId} from {a.flight.City} arriving at {a.flight.Time:HH:mm dd/MM/yyyy} in seat {a.row}:{a.col}.");
            if (hasDeparture)
                Console.WriteLine($"Departure Flight: Flight {d.flight.FlightId} to {d.flight.City} departing at {d.flight.Time:HH:mm dd/MM/yyyy} in seat {d.row}:{d.col}.");
        }

        // City points
        private static int CityPoints(string city) => city switch
        {
            "Sydney" => 1200,
            "Melbourne" => 1750,
            "Rockhampton" => 1400,
            "Adelaide" => 1950,
            "Perth" => 3375,
            _ => 0
        };

        private static (int row, char col) NextSeat(int row, char col)
        {
            int c = (col - 'A') + 1;         
            if (c == 4) { c = 0; row++; }    
            if (row == 11) row = 1;          
            return (row, (char)('A' + c));
        }

        private static (int row, char col) FindNextArrivalSeat(App app, ArrivalFlight flight, int startRow, char startCol)
        {
            int row = startRow; char col = startCol;
            for (int i = 0; i < 40; i++)                  
            {
                (row, col) = NextSeat(row, col);          
                if (!IsArrivalSeatTakenByAnyone(app, flight.FlightId, row, col))
                    return (row, col);
            }
            throw new InvalidOperationException("No seats available.");
        }

        private static (int row, char col) FindNextDepartureSeat(App app, DepartureFlight flight, int startRow, char startCol)
        {
            int row = startRow; char col = startCol;
            for (int i = 0; i < 40; i++)
            {
                (row, col) = NextSeat(row, col);
                if (!IsDepartureSeatTakenByAnyone(app, flight.FlightId, row, col))
                    return (row, col);
            }
            throw new InvalidOperationException("No seats available.");
        }

    }
}
