using Models;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public interface IPlaceRepository : IRepositoryBase<Place>
    {
        int GetActiveCount();
        IEnumerable<Place> GetActive();
        bool IsBlocked(int placeId);
        IEnumerable<Place> GetAllExtended();
    }
}

