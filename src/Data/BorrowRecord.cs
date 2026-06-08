using System.ComponentModel.DataAnnotations;

namespace ThuVienSo.Data
{
    public class BorrowRecord
    {
        [Key]
        [StringLength(20)]
        public string BorrowId { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string BookId { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Ngày Mượn")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Hẹn Trả")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Ngày Trả")]
        public DateTime? ReturnDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "DangMuon";

        [StringLength(100)]
        [Display(Name = "Người Tạo Phiếu")]
        public string? CreatedBy { get; set; }

        public Book? Book { get; set; }
        public User? User { get; set; }
    }
}
