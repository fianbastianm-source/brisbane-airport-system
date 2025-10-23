using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Registration flow for new users. This serves as collection details and creates either
    /// a Traveller, Frequent Flyer or Flight Manager in the repository.
    /// </summary>
    internal class RegisterScreen : Screen
    {
        public override Screen? Show(App app)
        {
            Console.WriteLine("Which user type would you like to register?");
            Console.WriteLine("1. A standard traveller.");
            Console.WriteLine("2. A frequent flyer.");
            Console.WriteLine("3. A flight manager.");
            Console.WriteLine("Please enter a choice between 1 and 3:");
            string? typeChoice = Console.ReadLine();


            if (typeChoice == "1")
            {
                Console.WriteLine("Registering as a traveller.");

                string name = PromptValidName();

                int age = PromptValidAge();

                string mobile = PromptValidMobileNumber();

                string email = PromptValidEmail(app);

                string password = PromptValidPassword();

                var traveller = new Traveller(name, age, email, mobile, password);
                app.Users.Add(traveller);

                Console.WriteLine($"Congratulations {name}. You have registered as a traveller.");
            }

            else if (typeChoice == "2")
            {
                Console.WriteLine("Registering as a frequent flyer.");

                string name = PromptValidName();

                int age = PromptValidAge();

                string mobile = PromptValidMobileNumber();

                string email = PromptValidEmail(app);

                string password = PromptValidPassword();

                // Frequent Flyer Number
                int flyerNumber;
                while (true)
                {
                    Console.WriteLine("Please enter in your frequent flyer number between 100000 and 999999:");
                    string input = Console.ReadLine() ?? "";
                    if (int.TryParse(input, out flyerNumber) && flyerNumber >= 100000 && flyerNumber <= 999999)
                        break;

                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied frequent flyer number is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                }

                // Frequent Flyer Points
                int flyerPoints;
                while (true)
                {
                    Console.WriteLine("Please enter in your current frequent flyer points between 0 and 1000000:");
                    string input = Console.ReadLine() ?? "";
                    if (int.TryParse(input, out flyerPoints) && flyerPoints >= 0 && flyerPoints <= 1000000)
                        break;

                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied current frequent flyer points is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                }

                // Save
                var flyer = new FrequentFlyer(name, age, email, mobile, password, flyerNumber, flyerPoints);
                app.Users.Add(flyer);

                Console.WriteLine($"Congratulations {name}. You have registered as a frequent flyer.");
            }

            else if (typeChoice == "3")
            {
                Console.WriteLine("Registering as a flight manager.");

                string name = PromptValidName();
                int age = PromptValidAge();
                string mobile = PromptValidMobileNumber();
                string email = PromptValidEmail(app);  
                string pwd = PromptValidPassword();

                int staffId;
                while (true)
                {
                    Console.WriteLine("Please enter in your staff id between 1000 and 9000:");
                    var s = (Console.ReadLine() ?? "").Trim();
                    if (int.TryParse(s, out staffId) && staffId >= 1000 && staffId <= 9000) break;

                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied staff id is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                }

                var manager = new FlightManager(name, age, email, mobile, pwd, staffId);
                app.Users.Add(manager);

                Console.WriteLine($"Congratulations {name}. You have registered as a flight manager.");
            }

            return new StartupScreen(false);

        }

        private static string PromptValidName()
        {
            while (true)
            {
                Console.WriteLine("Please enter in your name:");
                string name = Console.ReadLine() ?? "";

                if (IsValidName(name))
                    return name;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied name is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            bool hasLetter = name.Any(char.IsLetter);
            bool allAllowed = name.All(c => char.IsLetter(c) || c == '\'' || c == '-' || c == ' ');
            return hasLetter && allAllowed;
        }

        private static int PromptValidAge()
        {
            while (true)
            {
                Console.WriteLine("Please enter in your age between 0 and 99:");
                string input = Console.ReadLine() ?? "";

                if (!int.TryParse(input, out int age))
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied value is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                if (age < 0 || age > 99)
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied age is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                return age;
            }
        }


        private static string PromptValidMobileNumber()
        {
            while (true)
            {
                Console.WriteLine("Please enter in your mobile number:");
                string mobile = Console.ReadLine() ?? "";

                if (IsValidMobile(mobile))
                    return mobile;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied mobile number is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
        }

        private static bool IsValidMobile(string mobile)
        {
            return mobile.Length == 10 &&
                   mobile.StartsWith("0") &&
                   mobile.All(char.IsDigit);
        }


        private static string PromptValidEmail(App app)
        {
            while (true)
            {
                Console.WriteLine("Please enter in your email:");
                string email = Console.ReadLine() ?? "";

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Supplied email is invalid.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                if (app.Users.EmailExists(email))
                {
                    Console.WriteLine("#####");
                    Console.WriteLine("# Error - Email already registered.");
                    Console.WriteLine("# Please try again.");
                    Console.WriteLine("#####");
                    continue;
                }

                return email;
            }
        }

        private static bool IsValidEmail(string email)
        {
            int at = email.IndexOf('@');
            return at > 0 && at == email.LastIndexOf('@') && at < email.Length - 1;
        }

        private static string PromptValidPassword()
        {
            while (true)
            {
                Console.Write("Please enter in your password:");
                Console.WriteLine();

                Console.WriteLine("Your password must:");
                Console.WriteLine("-be at least 8 characters long");
                Console.WriteLine("-contain a number");
                Console.WriteLine("-contain a lowercase letter");
                Console.WriteLine("-contain an uppercase letter");

                string pwd = Console.ReadLine() ?? "";

                if (IsValidPassword(pwd))
                    return pwd;

                Console.WriteLine("#####");
                Console.WriteLine("# Error - Supplied password is invalid.");
                Console.WriteLine("# Please try again.");
                Console.WriteLine("#####");
            }
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