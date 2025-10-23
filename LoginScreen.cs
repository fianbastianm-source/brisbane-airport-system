using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// First step of the auth flow is prompts for email and password and,
    /// if it's valid, returns the appropriate menu screen based on the user type.
    /// </summary>
    internal class LoginScreen : Screen
    {
        public override Screen? Show(App app)
        {
            Console.WriteLine("Login Menu.");

            if (!app.Users.HasAnyUsers())
            {
                Console.WriteLine("#####");
                Console.WriteLine("# Error - There are no people registered.");
                Console.WriteLine("#####");
                return new StartupScreen(false);
            }

            // Email loop
            string email;
            while (true)
            {
                Console.WriteLine("Please enter in your email:");
                email = (Console.ReadLine() ?? "").Trim(); 

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied email is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                if (!app.Users.EmailExists(email))            
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Email is not registered.");
                    Console.WriteLine("#####");
                    continue;
                }
                break;
            }

            var user = app.Users.FindByEmail(email);         
            if (user is null) return new StartupScreen(false);

            // Password or pwd loop
            while (true)
            {
                Console.WriteLine("Please enter in your password:");
                string pwd = (Console.ReadLine() ?? "").Trim();

                if (!IsValidPassword(pwd))
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied password is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                if (user.Password != pwd)
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Incorrect Password.");
                    Console.WriteLine("#####");
                    continue;
                }

                Console.WriteLine($"Welcome back {user.Name}.");

                return user switch
                {
                    FlightManager m => new FlightManagerMenu(m),
                    FrequentFlyer f => new FrequentFlyerMenuScreen(f),
                    Traveller t => new TravellerMenuScreen(t),
                    _ => new StartupScreen(false),
                };
            }
        }

        private static bool IsValidEmail(string email)
        {
            int at = email.IndexOf('@');
            return at > 0 && at == email.LastIndexOf('@') && at < email.Length - 1;
        }

        private static bool IsValidPassword(string pwd)
        {
            if (pwd.Length < 8) return false;
            bool hasUpper = pwd.Any(char.IsUpper);
            bool hasLower = pwd.Any(char.IsLower);
            bool hasDigit = pwd.Any(char.IsDigit);
            return hasUpper && hasLower && hasDigit;
        }
    }
}