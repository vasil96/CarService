using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Queue
    {
        [Key]
        public int QueueId { get; set; }
        public bool IsActive { get; set; }
        public string SpecificNumber { get; set; }
        public int? PlaceId { get; set; }
        public ICollection<OptionQueue> OptionQueue { get; set; }
          
    }
}
