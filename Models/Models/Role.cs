using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}
