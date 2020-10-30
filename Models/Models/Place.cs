using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Place
    {
        [Key]
        public int PlaceId { get; set; }
        public bool IsBlock { get; set; }
        public bool IsBusy { get; set; }
        public int? ProtocolId { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }        
    }
}
