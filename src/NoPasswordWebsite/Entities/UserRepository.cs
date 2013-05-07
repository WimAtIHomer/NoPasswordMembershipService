using System.Linq;
using NoPasswordMembershipService;

namespace NoPasswordWebsite.Entities
{
    public class UserRepository : INoPasswordUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public User Find(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        INoPasswordUser INoPasswordUserRepository.Find(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}