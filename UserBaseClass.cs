using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// Common base type for all user roles. Stores identity (name/age/email/mobile/pwd)
    /// and credentials and exposes password update logic.
    /// </summary>
    internal abstract class UserBaseClass
    {
        public string Name { get; protected set; }
        public int Age { get; protected set; }
        public string Email { get; protected set; }
        public string Mobile { get; protected set; }

        public string Password { get; private set; }


        protected UserBaseClass(string name, int age, string email, string mobile, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = mobile;
            Password = password;
        }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }
    }
}