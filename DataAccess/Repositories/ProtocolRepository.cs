using System.Collections.Generic;
using System.Linq;
using Models;

namespace DataAccess.Repositories
{
    public class ProtocolRepository : RepositoryBase<Protocol>, IProtocolRepository
    {
        private IEnumerable<Protocol> _Protocol;

        public IEnumerable<Protocol> Protocol
        {
            get
            {
                return _Protocol;
            }
        }
        public ProtocolRepository(RepositoryContext context) : base(context)
        {
            _Protocol = context.Protocol;
        }
    }
}

