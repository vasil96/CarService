using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class OptionQueue
    {
        [Key]
        public int OptionQueueId { get; set; }
        public int QueueId { get; set; }
        public int OptionId { get; set; }
    }
}
