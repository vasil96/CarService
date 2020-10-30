using System.Collections.Generic;
using System.Linq;
using Models;

namespace DataAccess.Repositories
{
    public class OptionRepository : RepositoryBase<Option>, IOptionRepository
    {
        private IEnumerable<Option> _Option;

        public IEnumerable<Option> Option
        {
            get
            {
                return _Option;
            }
        }
        public OptionRepository(RepositoryContext context) : base(context)
        {
            _Option = context.Option;
        }
    }
}

