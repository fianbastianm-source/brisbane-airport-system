using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Serves as the interactive menu interface for Flight Managers.
    /// This class provides all functionality available to flight managers,
    /// including creating new flights, delaying flights, viewing flight details,
    /// and managing account information.
    /// </summary>
    internal class FlightManagerMenu : Screen
    {
        private readonly FlightManager _manager;

        public FlightManagerMenu(FlightManager manager)
        {
            _manager = manager;
        }

        public override Screen Show(App app)
        {
            Console.WriteLine("Flight Manager Menu.");
            Console.WriteLine("Please make a choice from the menu below:");
            Console.WriteLine("1. See my details.");
            Console.WriteLine("2. Change password.");
            Console.WriteLine("3. Create an arrival flight.");
            Console.WriteLine("4. Create a departure flight.");
            Console.WriteLine("5. Delay an arrival flight.");
            Console.WriteLine("6. Delay a departure flight.");
            Console.WriteLine("7. See the details of all flights.");
            Console.WriteLine("8. Logout.");
            Console.WriteLine("Please enter a choice between 1 and 8:");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1":
                    PrintManagerDetails();
                    break;
                case "2":
                    ChangePassword();
                    break;
                case "3":
                    CreateArrivalFlight(app);
                    break;
                case "4":
                    CreateDepartureFlight(app);
                    break;
                case "5":
                    DelayArrivalFlight(app);
                    break;
                case "6":
                    DelayDepartureFlight(app);
                    break;
                case "7":
                    ShowFlightDetails(app);
                    break;
                case "8":
                    return new StartupScreen(false);
                default:
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Invalid choice.");
                    Console.WriteLine("#####");
                    break;
            }

            return this;
        }

        // Option 1
        private void PrintManagerDetails()
        {
            Console.WriteLine("Your details.");
            Console.WriteLine($"Name: {_manager.Name}");
            Console.WriteLine($"Age: {_manager.Age}");
            Console.WriteLine($"Mobile phone number: {_manager.Mobile}");
            Console.WriteLine($"Email: {_manager.Email}");
            Console.WriteLine($"Staff ID: {_manager.StaffId}");
        }

        // Option 2
        private void ChangePassword()
        {
            Console.WriteLine("Please enter your current password:");
            string current = Console.ReadLine() ?? "";
            if (current != _manager.Password)
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - Incorrect Password.");
                Console.WriteLine("#####");
                return;
            }

            Console.WriteLine("Please enter your new password:");
            Console.WriteLine("Your password must:");
            Console.WriteLine("- be at least 8 characters long");
            Console.WriteLine("- contain a number");
            Console.WriteLine("- contain a lowercase letter");
            Console.WriteLine("- contain an uppercase letter");

            while (true)
            {
                string newPwd = Console.ReadLine() ?? "";
                if (IsValidPassword(newPwd))
                {
                    _manager.ChangePassword(newPwd);
                    Console.WriteLine("Your password has been successfully changed.");
                    break;
                }

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied password is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private static int InferPriceFromFlightNumber(int flightNum)
        {
            if (flightNum < 200) return 1200; 
            if (flightNum < 300) return 1250;  
            if (flightNum < 400) return 1400;  
            if (flightNum < 500) return 1750;  
            return 2300;                       
        }


        // Option 3
        private void CreateArrivalFlight(App app)
        {
            var airline = ChooseAirline();
            string origin = ChooseCity("departing city");

            int flightNum = PromptFlightNumber100to900();
            int planeDigit = PromptPlaneDigit0to9();

            Console.WriteLine("Please enter in the arrival date and time in the format HH:mm dd/MM/yyyy:");
            DateTime arrWhen = ReadDateTime();

            string flightId = $"{airline.Code}{flightNum:D3}";
            string planeId = $"{airline.Code}{planeDigit}A";   // A = arrival

            int price = InferPriceFromFlightNumber(flightNum);

            if (app.ArrivalFlights.Any(f => f.PlaneId == planeId))
            {
                PlaneAlreadyAssignedError(planeId, isArrival: true);
                return;
            }

            var flight = new ArrivalFlight
            {
                FlightId = flightId,
                Airline = airline.Name,
                City = origin,
                PlaneId = planeId,
                Time = arrWhen,
                Price = price
            };

            app.ArrivalFlights.Add(flight);
            Console.WriteLine($"Flight {flightId} on plane {planeId} has been added to the system.");
        }

        // Option 4
        private void CreateDepartureFlight(App app)
        {
            var airline = ChooseAirline();
            string dest = ChooseCity("arrival city");

            int flightNum = PromptFlightNumber100to900();
            int planeDigit = PromptPlaneDigit0to9();

            int price = InferPriceFromFlightNumber(flightNum);

            Console.WriteLine("Please enter in the departure date and time in the format HH:mm dd/MM/yyyy:");
            DateTime depWhen = ReadDateTime();

            string flightId = $"{airline.Code}{flightNum:D3}";
            string planeId = $"{airline.Code}{planeDigit}D";   // D = departure

            if (app.DepartureFlights.Any(f => f.PlaneId == planeId))
            {
                PlaneAlreadyAssignedError(planeId, isArrival: false);
                return;
            }

            var flight = new DepartureFlight
            {
                FlightId = flightId,
                Airline = airline.Name,
                City = dest,
                PlaneId = planeId,
                Time = depWhen,
                Price = price,
            };

            app.DepartureFlights.Add(flight);
            Console.WriteLine($"Flight {flightId} on plane {planeId} has been added to the system.");
        }


        // Option 5
        private void DelayArrivalFlight(App app)
        {
            if (app.ArrivalFlights.Count == 0)
            {
                Console.WriteLine("The airport does not have any arrival flights.");
                return;
            }

            Console.WriteLine("Please enter the arrival flight:");
            for (int i = 0; i < app.ArrivalFlights.Count; i++)
            {
                var f = app.ArrivalFlights[i];
                Console.WriteLine($"{i + 1}. Flight {f.FlightId} operated by {f.Airline} arriving at {f.Time:HH:mm dd/MM/yyyy} from {f.City} on plane {f.PlaneId}.");
            }

            int idx = ReadChoiceWithErrors(app.ArrivalFlights.Count) - 1;
            int minutes = ReadDelayMinutes();

            var arrival = app.ArrivalFlights[idx];
            arrival.Time = arrival.Time.AddMinutes(minutes);

            var planeId = arrival.PlaneId;                   
            if (planeId.Length >= 1 && planeId[^1] == 'A')
            {
                var depPlaneId = planeId[..^1] + "D";        
                var linkedDeparture = app.DepartureFlights.FirstOrDefault(d => d.PlaneId == depPlaneId);
                if (linkedDeparture != null)
                {
                    linkedDeparture.Time = linkedDeparture.Time.AddMinutes(minutes);
                }
            }
        }


        // Option 6
        private void DelayDepartureFlight(App app)
        {
            if (app.DepartureFlights.Count == 0)
            {
                Console.WriteLine("The airport does not have any departure flights.");
                return;
            }

            Console.WriteLine("Please enter the departure flight:");
            for (int i = 0; i < app.DepartureFlights.Count; i++)
            {
                var f = app.DepartureFlights[i];
                Console.WriteLine($"{i + 1}. Flight {f.FlightId} operated by {f.Airline} departing at {f.Time:HH:mm dd/MM/yyyy} to {f.City} on plane {f.PlaneId}.");
            }

            int idx = ReadChoiceWithErrors(app.DepartureFlights.Count) - 1;
            int minutes = ReadDelayMinutes();

            var flight = app.DepartureFlights[idx];
            flight.Time = flight.Time.AddMinutes(minutes);
        }


        // Option 7
        private void ShowFlightDetails(App app)
        {
            Console.WriteLine("Arrival Flights:");
            if (app.ArrivalFlights.Count == 0)
            {
                Console.WriteLine("There are no arrival flights.");
            }
            else
            {
                foreach (var f in app.ArrivalFlights.OrderBy(f => f.Time))
                    Console.WriteLine($"Flight {f.FlightId} operated by {f.Airline} arriving at {f.Time:HH:mm dd/MM/yyyy} from {f.City} on plane {f.PlaneId}.");
            }

            Console.WriteLine("Departure Flights:");
            if (app.DepartureFlights.Count == 0)
            {
                Console.WriteLine("There are no departure flights.");
            }
            else
            {
                foreach (var f in app.DepartureFlights.OrderBy(f => f.Time))
                    Console.WriteLine($"Flight {f.FlightId} operated by {f.Airline} departing at {f.Time:HH:mm dd/MM/yyyy} to {f.City} on plane {f.PlaneId}.");
            }
        }



        // Helper Functions
        private readonly (string Name, string Code)[] _airlines =
        {
            ("Jetstar", "JST"),
            ("Qantas", "QFA"),
            ("Regional Express", "RXA"),
            ("Virgin", "VOZ"),
            ("Fly Pelican", "FRE"),
        };

        private readonly string[] _cities =
        {
            "Sydney",
            "Melbourne",
            "Rockhampton",
            "Adelaide",
            "Perth"
        };

        private string ChooseCity(string prompt)
        {
            Console.WriteLine($"Please enter the {prompt}:");
            for (int i = 0; i < _cities.Length; i++)
                Console.WriteLine($"{i + 1}. {_cities[i]}");
            Console.WriteLine("Please enter a choice between 1 and 5:");

            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int n) && n >= 1 && n <= 5)
                    return _cities[n - 1];

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied value is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private (string Name, string Code) ChooseAirline()
        {
            Console.WriteLine("Please enter the airline:");
            for (int i = 0; i < _airlines.Length; i++)
                Console.WriteLine($"{i + 1}. {_airlines[i].Name}");
            Console.WriteLine("Please enter a choice between 1 and 5:");

            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int n) && n >= 1 && n <= 5)
                    return _airlines[n - 1];

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied value is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private int PromptFlightNumber100to900()
        {
            Console.WriteLine("Please enter in your flight id between 100 and 900:");
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int n) && n >= 100 && n <= 900)
                    return n;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied flight id is invalid.");
                Console.WriteLine("# It must be between 100 and 900.");
                Console.WriteLine("#####");
            }
        }

        private int PromptPlaneDigit0to9()
        {
            Console.WriteLine("Please enter in your plane id between 0 and 9:");
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int n) && n >= 0 && n <= 9)
                    return n;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied plane id digit is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private DateTime ReadDateTime()
        {
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (DateTime.TryParseExact(s, "HH:mm dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    return dt;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied date/time is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private static void PlaneAlreadyAssignedError(string planeId, bool isArrival)
        {
            Console.WriteLine("#####");
            Console.WriteLine(isArrival
                ? $"# Error - Plane {planeId} has already been assigned to an arrival flight."
                : $"# Error - Plane {planeId} has already been assigned to a departure flight.");
            Console.WriteLine("#####");
        }

        private static void PrintOutOfRangeError()
        {
            Console.WriteLine("#####");
            Console.WriteLine("# Error - Supplied value is out of range.");
            Console.WriteLine("# Please try again.");
            Console.WriteLine("#####");
        }

        private static int ReadChoiceWithErrors(int max)
        {
            Console.WriteLine($"Please enter a choice between 1 and {max}:");
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int v) && v >= 1 && v <= max) return v;
                PrintOutOfRangeError();
                Console.WriteLine($"Please enter a choice between 1 and {max}:");
            }
        }

        private static int ReadDelayMinutes()
        {
            Console.WriteLine("Please enter in your minutes delayed:");
            while (true)
            {
                string s = Console.ReadLine() ?? "";
                if (int.TryParse(s, out int m) && m >= 0) return m;
                PrintOutOfRangeError();
                Console.WriteLine("Please enter in your minutes delayed:");
            }
        }


        private static bool IsValidPassword(string pwd)
        {
            if (string.IsNullOrEmpty(pwd) || pwd.Length < 8) return false;
            bool hasUpper = pwd.Any(char.IsUpper);
            bool hasLower = pwd.Any(char.IsLower);
            bool hasDigit = pwd.Any(char.IsDigit);
            return hasUpper && hasLower && hasDigit;
        }
    }
}