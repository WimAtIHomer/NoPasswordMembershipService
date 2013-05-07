using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoPasswordMembershipService
{
    public interface INoPasswordUser
    {
        string Email { get; set; }
        string Password { get; set; }
        DateTime? ResetDate { get; set; }
        DateTime LastLoginDate { get; set; }
    }
}
