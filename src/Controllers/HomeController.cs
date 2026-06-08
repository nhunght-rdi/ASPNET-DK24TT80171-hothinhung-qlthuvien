using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? search, string? category)
        {
            var query = _db.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(b =>
                    b.BookTitle.Contains(search) ||
                    (b.Author != null && b.Author.Contains(search)) ||
                    (b.Category != null && b.Category.CategoryName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(b => b.Category != null && b.Category.CategoryName == category);

            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.TotalBooks = await _db.Books.CountAsync();
            ViewBag.TotalUsers = await _db.Users.Where(u => u.Role == "Reader").CountAsync();
            ViewBag.Borrowing = await _db.BorrowRecords.Where(br => br.Status == "DangMuon").CountAsync();
            ViewBag.Violations = await _db.Violations.Where(v => v.Status == "ChuaXuLy").CountAsync();
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;

            return View(await query.ToListAsync());
        }

        public IActionResult Error() => View();
    }
}
