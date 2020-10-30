using Models;

namespace DataAccess.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        User GetAuthenticatedUser(string username, string password);
    }
}