using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Serves as a flight manager user in the Brisbane Airport system.
    /// </summary>
    internal class FlightManager : UserBaseClass
    {
        public int StaffId { get; }

        public FlightManager(string name, int age, string email, string mobile, string password, int staffId)
            : base(name, age, email, mobile, password)
        {
            StaffId = staffId;
        }
    }
}