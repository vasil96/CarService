using System.Collections.Generic;
using System.Linq;
using Models;

namespace DataAccess.Repositories
{
    public class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        private IEnumerable<Role> _Role;
        public RoleRepository(RepositoryContext context) : base(context)
        {
            _Role = context.Role;
        }        
    }
}
