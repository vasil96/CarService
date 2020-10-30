using System.Collections.Generic;
using System.Linq;
using Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PlaceOptionRepository : RepositoryBase<PlaceOption>, IPlaceOptionRepository
    {
        private IEnumerable<PlaceOption> _PlaceOption;
        public PlaceOptionRepository(RepositoryContext context) : base(context)
        {
            _PlaceOption = context.PlaceOption;
        }     

    }
}
