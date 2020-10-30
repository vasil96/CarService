using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Setting
    {
        public int SettingId { get; set; }
        public bool IsLimitDistance { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        public int AllowDistance { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        [Range(1, int.MaxValue, ErrorMessage = "Допустими стойности от {0} до {1}")]
        public int CheckAllowDistanceInterval { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        [Column(TypeName = "decimal(8,6)")]
        public decimal Latitude { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        [Column(TypeName = "decimal(8,6)")]
        public decimal Longitude { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        [Range(1, int.MaxValue, ErrorMessage = "Допустими стойности от {0} до {1}")]
        public int CheckingTimeInPlace { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        [Range(1,int.MaxValue, ErrorMessage = "Допустими стойности от {0} до {1}")]
        public int RefreshDataInterval { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        public int WaitingTime { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        public TimeSpan StartWorkingTime { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        public TimeSpan EndWorkingTime { get; set; }
        [Required(ErrorMessage = "Полето е задължително.")]
        public string Token { get; set; }

    }
}
