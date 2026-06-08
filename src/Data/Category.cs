using System.ComponentModel.DataAnnotations;

namespace ThuVienSo.Data
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên Thể Loại")]
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
