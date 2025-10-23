using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Serves as a registered frequent flyer user in the Brisbane Airport system.
    /// </summary>
    internal class FrequentFlyer : UserBaseClass
    {
        public int FlyerNumber { get; }
        public int FlyerPoints { get; set; }

        public FrequentFlyer(string name, int age, string mobile, string email, string password,
                             int flyerNumber, int flyerPoints)
            : base(name, age, mobile, email, password)
        {
            FlyerNumber = flyerNumber;
            FlyerPoints = flyerPoints;
        }
    }
}