using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuVienSo.Data
{
    public class Violation
    {
        [Key]
        public int ViolationId { get; set; }

        [Required, StringLength(20)]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string BookId { get; set; } = string.Empty;

        [Display(Name = "Ngày Vi Phạm")]
        public DateTime ViolationDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Hình Thức Phạt")]
        public string PenaltyType { get; set; } = "CanhCao";

        [StringLength(500)]
        [Display(Name = "Tên Vi Phạm")]
        public string ViolationName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số Tiền Phạt")]
        public decimal? PenaltyAmount { get; set; }

        [StringLength(20)]
        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "ChuaXuLy";

        public User? User { get; set; }
    }
}
