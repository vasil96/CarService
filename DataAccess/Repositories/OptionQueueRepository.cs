using System.Collections.Generic;
using System.Linq;
using Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class OptionQueueRepository : RepositoryBase<OptionQueue>, IOptionQueueRepository
    {
        private IEnumerable<OptionQueue> _OptionQueue;        
        public OptionQueueRepository(RepositoryContext context) : base(context)
        {
            _OptionQueue = context.OptionQueue;
        }       

    }
}
