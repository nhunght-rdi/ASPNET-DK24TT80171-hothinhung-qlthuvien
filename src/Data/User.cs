using System.ComponentModel.DataAnnotations;

namespace ThuVienSo.Data
{
    public class User
    {
        [Key]
        [StringLength(20)]
        public string UserId { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Họ Tên")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(15)]
        [Display(Name = "Số Điện Thoại")]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa Chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Ngày Đăng Ký")]
        public DateTime RegisterDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        [Display(Name = "Vai Trò")]
        public string Role { get; set; } = "Reader";

        [StringLength(20)]
        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "Active";

        [Display(Name = "Số Lần Vi Phạm")]
        public int ViolationCount { get; set; } = 0;

        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
        public ICollection<Violation> Violations { get; set; } = new List<Violation>();
    }
}
