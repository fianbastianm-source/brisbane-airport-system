using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Landing menu shown on app start or after logout. Lets the user choose
    /// to log in, register, or exit the program.
    /// </summary>
    internal class StartupScreen : Screen
    {
        private readonly bool _showBanner;

        public StartupScreen(bool showBanner = true)
        {
            _showBanner = showBanner;
        }

        public override Screen? Show(App app)
        {
            if (_showBanner)
            {
                Console.WriteLine("==========================================");
                Console.WriteLine("=  Welcome to Brisbane Domestic Airport  =");
                Console.WriteLine("==========================================");
                Console.WriteLine();
            }

            Console.WriteLine("Please make a choice from the menu below:");
            Console.WriteLine("1. Login as a registered user.");
            Console.WriteLine("2. Register as a new user.");
            Console.WriteLine("3. Exit.");
            Console.WriteLine("Please enter a choice between 1 and 3:");

            string? input = Console.ReadLine();

            if (input == "3")
            {
                Console.WriteLine("Thank you. Safe travels.");
                return null;
            }
            if (input == "2") return new RegisterScreen();   
            if (input == "1") return new LoginScreen();      

            return new StartupScreen(false);
        }
    }
}