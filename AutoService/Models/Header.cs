using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoServiceWebSite.Models
{
    public class Header
    {
        public bool IsAdministrator { get; set; }
        public bool IsTechnician { get; set; }
        public bool IsOffice { get; set; }
        public bool IsMechanic { get; set; }
        public int? IncomleteQueueId { get; set; }

    }
}
