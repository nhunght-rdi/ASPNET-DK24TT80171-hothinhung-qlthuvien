using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public BookController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            var query = _db.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b =>
                    b.BookTitle.Contains(search) ||
                    (b.Author != null && b.Author.Contains(search)) ||
                    (b.Publisher != null && b.Publisher.Contains(search)));

            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);

            var dsTheLoai = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(dsTheLoai, "CategoryId", "CategoryName");
            ViewBag.Search = search;
            ViewBag.SelectedCategory = categoryId;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            var book = await _db.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var dsTheLoai = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(dsTheLoai, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book model, IFormFile? coverImage)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (await _db.Books.AnyAsync(b => b.BookId == model.BookId))
                ModelState.AddModelError("BookId", "Mã sách đã tồn tại.");

            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                    var path = Path.Combine(_env.WebRootPath, "images/books", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using var stream = new FileStream(path, FileMode.Create);
                    await coverImage.CopyToAsync(stream);
                    model.CoverImage = "/images/books/" + fileName;
                }

                _db.Books.Add(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm sách thành công!";
                return RedirectToAction(nameof(Index));
            }

            var dsTheLoai = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(dsTheLoai, "CategoryId", "CategoryName");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();
            var dsTheLoai = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(dsTheLoai, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book model, IFormFile? coverImage)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (ModelState.IsValid)
            {
                var book = await _db.Books.FindAsync(model.BookId);
                if (book == null) return NotFound();

                if (coverImage != null && coverImage.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(coverImage.FileName);
                    var path = Path.Combine(_env.WebRootPath, "images/books", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    using var stream = new FileStream(path, FileMode.Create);
                    await coverImage.CopyToAsync(stream);
                    model.CoverImage = "/images/books/" + fileName;
                }
                else
                {
                    model.CoverImage = book.CoverImage;
                }

                _db.Entry(book).CurrentValues.SetValues(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sách thành công!";
                return RedirectToAction(nameof(Index));
            }

            var dsTheLoai = await _db.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(dsTheLoai, "CategoryId", "CategoryName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var book = await _db.Books.FindAsync(id);
            if (book != null)
            {
                _db.Books.Remove(book);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa sách thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
