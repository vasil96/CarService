using System.Collections.Generic;
using System.Linq;
using Models;

namespace DataAccess.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private IEnumerable<User> _User;
        public UserRepository(RepositoryContext context) : base(context)
        {
            _User = context.User;
        }
        public User GetAuthenticatedUser(string username, string password)
        {
            var user = _User.Where(u => u.UserName == username && u.Password == password).FirstOrDefault(); ;
            return user;
        }
    }
}
