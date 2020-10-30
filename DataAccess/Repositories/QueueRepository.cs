using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Models;

namespace DataAccess.Repositories
{
    public class QueueRepository : RepositoryBase<Queue>, IQueueRepository
    {
        private IEnumerable<Queue> _Queue;
        private RepositoryContext _context;

        public QueueRepository(RepositoryContext context) : base(context)
        {
            _Queue = context.Queue;
            _context = context;
        }

        public int GetCount()
        {

            var queueCount = _Queue.Count(q => q.IsActive && !q.PlaceId.HasValue);
            return queueCount;
        }
        public int GetCountByPlace(int placeId)
        {
            var queues = _context.Queue.Where(q => q.IsActive && !q.PlaceId.HasValue).Include(q => q.OptionQueue);
            var placeOptions = _context.PlaceOption.Where(po => po.PlaceId == placeId).Select(p => p.OptionId);
            var count = queues.Where(p => p.OptionQueue.Any(v => placeOptions.Contains(v.OptionId))).Count();

            return count;
        }
        public int GetCountById(int id)
        {
            var queueCount = _Queue.Count(q => q.IsActive && q.QueueId < id && !q.PlaceId.HasValue);
            return queueCount;
        }
        public int GetNextNumberInLine()
        {
            var queue = _Queue.Where(q => q.IsActive).OrderBy(q => q.QueueId).FirstOrDefault();

            if (queue != null)
                return queue.QueueId;

            return 0;
        }

        public Queue GetNextInLine()
        {
            var queue = _Queue.Where(q => q.IsActive && !q.PlaceId.HasValue).OrderBy(q => q.QueueId).FirstOrDefault();
            return queue;
        }
        public Queue GetNextInLineByPlace(int placeId)
        {
            var queues = _context.Queue.Where(q => q.IsActive && !q.PlaceId.HasValue).Include(q => q.OptionQueue);
            var placeOptions = _context.PlaceOption.Where(po => po.PlaceId == placeId).Select(p => p.OptionId);
            var queue = queues.Where(p => p.OptionQueue.Any(v => placeOptions.Contains(v.OptionId))).OrderBy(q=>q.QueueId).FirstOrDefault();
            return queue;
        }

        public Queue GetIncomplete(int placeId)
        {
            var queue = _Queue.Where(q => q.IsActive && q.PlaceId.HasValue && q.PlaceId.Value == placeId).FirstOrDefault();

            return queue;
        }

        public void Clean()
        {
            RepositoryContext.Database.ExecuteSqlCommand("" +
                "TRUNCATE TABLE OptionQueue; " +
                "ALTER TABLE OptionQueue DROP CONSTRAINT FK_OptionQueue_Queue;" +
                "TRUNCATE TABLE Queue;" +
                "ALTER TABLE OptionQueue ADD CONSTRAINT FK_OptionQueue_Queue FOREIGN KEY(QueueId) REFERENCES Queue(QueueId)");           
        }
    }
}
