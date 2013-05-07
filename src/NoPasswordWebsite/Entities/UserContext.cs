using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NoPasswordWebsite.Entities
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; } 
    }
}