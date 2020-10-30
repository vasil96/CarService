using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Полето \"Псевдоним\" е задължително.")]
        [StringLength(12, MinimumLength = 3, ErrorMessage = "Полето \"Псевдоним\" не може да е по-малко от 3 и по-голямо от 12 символа")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Полето \"Име\" е задължително.")]
        [RegularExpression(@"^[а-яА-Я]+$", ErrorMessage = "Полето \"Име\" може да съдържа само букви на кирилица.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Полето \"Име\" не може да е по-малко от 3 и по-голямо от 15 символа")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Полето \"Презиме\" е задължително.")]
        [RegularExpression(@"^[а-яА-Я]+$", ErrorMessage = "Полето \"Презиме\" може да съдържа само букви на кирилица.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Полето \"Презиме\" не може да е по-малко от 3 и по-голямо от 15 символа")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Полето \"Фамилия\" е задължително.")]
        [RegularExpression(@"^[а-яА-Я]+$", ErrorMessage = "Полето \"Фамилия\" може да съдържа само букви на кирилица.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Полето \"Фамилия\" не може да е по-малко от 3 и по-голямо от 15 символа")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Полето \"Парола\" е задължително.")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Полето \"Парола\" не може да е по-малко от 6 и по-голямо от 15 символа")]
        public string Password { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Полето \"Роля\" е задължително.")]
        public int RoleId { get; set; }
    }
}
