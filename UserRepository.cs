using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brisbane_Airport
{
    /// <summary>
    /// this serves as in-memory store for all users. Provides lookups by email and helpers to add
    /// new users while gives uniqueness constraints.
    /// </summary>
    internal class UserRepository
    {
        private readonly List<UserBaseClass> _users = new();

        public void Add(UserBaseClass user) => _users.Add(user);

        public bool HasAnyUsers() => _users.Count > 0;

        public bool EmailExists(string email) =>
            _users.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

        public UserBaseClass? FindByEmail(string email) =>
            _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }
}