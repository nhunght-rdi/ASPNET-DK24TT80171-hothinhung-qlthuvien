using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThuVienSo.Data
{
    public class Book
    {
        [Key]
        [StringLength(20)]
        public string BookId { get; set; } = string.Empty;

        [Required, StringLength(255)]
        [Display(Name = "Tên Sách")]
        public string BookTitle { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Tác Giả")]
        public string? Author { get; set; }

        [StringLength(100)]
        [Display(Name = "Nhà Xuất Bản")]
        public string? Publisher { get; set; }

        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [StringLength(50)]
        [Display(Name = "Tình Trạng")]
        public string Status { get; set; } = "Còn sách";

        [Display(Name = "Mô Tả")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh Bìa")]
        public string? CoverImage { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}
