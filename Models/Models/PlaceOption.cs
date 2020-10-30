using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class PlaceOption
    {
        [Key]
        public int PlaceOptionId { get; set; }
        public int PlaceId { get; set; }
        public int OptionId { get; set; }
    }
}
