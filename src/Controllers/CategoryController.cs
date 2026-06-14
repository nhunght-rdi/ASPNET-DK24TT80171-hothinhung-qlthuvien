using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class CategoryController(AppDbContext db) : Controller
    {
        private readonly AppDbContext _db = db;

        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var categories = await _db.Categories
                .Include(c => c.Books)
                .ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (await _db.Categories.AnyAsync(c => c.CategoryName == model.CategoryName))
                ModelState.AddModelError("CategoryName", "Tên thể loại đã tồn tại.");

            if (ModelState.IsValid)
            {
                _db.Categories.Add(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm thể loại thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (await _db.Categories.AnyAsync(c => c.CategoryName == model.CategoryName && c.CategoryId != model.CategoryId))
                ModelState.AddModelError("CategoryName", "Tên thể loại đã tồn tại.");

            if (ModelState.IsValid)
            {
                var category = await _db.Categories.FindAsync(model.CategoryId);
                if (category == null) return NotFound();
                category.CategoryName = model.CategoryName;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thể loại thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var category = await _db.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null) return NotFound();

            if (category.Books.Count > 0)
            {
                TempData["Warning"] = $"Không thể xóa thể loại \"{category.CategoryName}\" vì còn {category.Books.Count} sách thuộc thể loại này.";
                return RedirectToAction(nameof(Index));
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa thể loại \"{category.CategoryName}\".";
            return RedirectToAction(nameof(Index));
        }
    }
}
