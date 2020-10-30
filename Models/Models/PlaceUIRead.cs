using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PlaceUIRead
    {
        [Key]
        public int PlaceId { get; set; }
        public bool IsBlock { get; set; }
        public bool IsBusy { get; set; }
        public User User { get; set; }        
        public List<Option> Option { get; set; }
    }
}
