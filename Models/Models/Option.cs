using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }
        public string Name { get; set; }
    }
}
