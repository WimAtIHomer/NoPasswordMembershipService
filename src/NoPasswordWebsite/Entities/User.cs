using System;
using System.Security.Principal;
using NoPasswordMembershipService;

namespace NoPasswordWebsite.Entities
{
    public class User : INoPasswordUser, IIdentity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? ResetDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsAdmin { get; set; }

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        string IIdentity.Name
        {
            get { return Email; }
        }
    }
}