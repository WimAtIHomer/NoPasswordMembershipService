using IHomer.GenericRepository.Interfaces;

namespace NoPasswordMembershipService
{
    public interface INoPasswordUserRepository : ISaveChanges
    {
        INoPasswordUser Find(string email);
    }
}
