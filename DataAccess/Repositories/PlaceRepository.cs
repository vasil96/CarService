using System.Collections.Generic;
using System.Linq;
using Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PlaceRepository : RepositoryBase<Place>, IPlaceRepository
    {
        private IEnumerable<Place> _place;
        public PlaceRepository(RepositoryContext context) : base(context)
        {
            _place = context.Place;
        }

        public int GetActiveCount()
        {

            return _place.Count(p => p.IsBlock == false);
        }
        public IEnumerable<Place> GetActive()
        {
            return _place.Where(p => p.IsBlock == false && !p.IsBusy);
        }

        public bool IsBlocked(int placeId)
        {
            var place = _place.Where(p => p.PlaceId == placeId).FirstOrDefault();
            return place.IsBlock || !place.IsBusy;
        }       
        public IEnumerable<Place> GetAllExtended()
        {
            return _place;
        }
    }
}
