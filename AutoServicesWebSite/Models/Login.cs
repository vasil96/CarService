using Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoServicesWebSite.Models
{
    public class Login
    {
        //public Login() {
        //    Places= new HashSet<Place>();
        //}
        [Required(ErrorMessage ="Полето \"Потребител\" е задължително.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Полето \"Парола\" е задължително.")]
        public string Password { get; set; }
        public int PlaceId { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}

