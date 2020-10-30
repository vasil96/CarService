using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UserPlace
    {
        [Key]
        public int UserPlaceId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PlaceId { get; set; }
    }
}
