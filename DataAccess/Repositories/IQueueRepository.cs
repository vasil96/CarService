using Models;
using System.Collections.Generic;

namespace DataAccess.Repositories
{
    public interface IQueueRepository : IRepositoryBase<Queue>
    {        
        int GetCount();
        int GetCountByPlace(int placeId);
        int GetCountById(int id);
        int GetNextNumberInLine();
        Queue GetNextInLine();
        Queue GetNextInLineByPlace(int placeId);
        Queue GetIncomplete(int placeId);
        void Clean();
    }
}
