using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult DangNhap() => View();

        [HttpPost]
        public async Task<IActionResult> DangNhap(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                return View();
            }
            if (user.Status == "Locked")
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DangKy() => View();

        [HttpPost]
        public async Task<IActionResult> DangKy(User model, string confirmPassword)
        {
            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            {
                ViewBag.Error = "Email đã được sử dụng.";
                return View(model);
            }
            if (model.Password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View(model);
            }

            model.UserId = "DG" + DateTime.Now.ToString("yyyyMMddHHmmss");
            model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.Role = "Reader";
            model.Status = "Active";
            model.RegisterDate = DateTime.Now;

            _db.Users.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("DangNhap");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string oldPassword, string newPassword, string confirm)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("DangNhap");

            var user = await _db.Users.FindAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.Password))
            {
                ViewBag.Error = "Mật khẩu cũ không đúng.";
                return View();
            }
            if (newPassword != confirm)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}
