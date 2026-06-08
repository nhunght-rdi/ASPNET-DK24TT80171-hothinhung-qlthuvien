using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        public UserController(AppDbContext db) { _db = db; }
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index(string? search)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var query = _db.Users.Where(u => u.Role == "Reader").AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search) || u.UserId.Contains(search));

            ViewBag.Search = search;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var user = await _db.Users
                .Include(u => u.BorrowRecords).ThenInclude(br => br.Book)
                .Include(u => u.Violations)
                .FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User model)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
                ModelState.AddModelError("Email", "Email đã được sử dụng.");

            if (ModelState.IsValid)
            {
                model.UserId = "DG" + DateTime.Now.ToString("yyyyMMddHHmmss");
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                model.Role = "Reader";
                model.RegisterDate = DateTime.Now;
                _db.Users.Add(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm độc giả thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User model)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var user = await _db.Users.FindAsync(model.UserId);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Status = model.Status;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật độc giả thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user != null) { _db.Users.Remove(user); await _db.SaveChangesAsync(); TempData["Success"] = "Xóa độc giả thành công!"; }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLock(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = user.Status == "Active" ? "Locked" : "Active";
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Tài khoản đã được {(user.Status == "Active" ? "mở khóa" : "khóa")}!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
