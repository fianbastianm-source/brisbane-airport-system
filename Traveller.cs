using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Domain model for a standard traveller (non–frequent flyer).
    /// </summary>
    internal class Traveller : UserBaseClass
    {
        public Traveller(string name, int age, string email, string mobile, string password)
            : base(name, age, email, mobile, password) { }
    }
}