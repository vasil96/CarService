using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PlaceUIWrite
    {
        [Key]
        public int PlaceId { get; set; }
        public bool IsBlock { get; set; }
        public bool IsBusy { get; set; }
        public User User { get; set; }        
        public int[] Option { get; set; }
    }
}
