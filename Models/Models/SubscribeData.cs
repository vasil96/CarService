using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class SubscribeData
    {
        public string SpecificNumber { get; set; }
        public IEnumerable<Option> Option { get; set; }
    }
}
