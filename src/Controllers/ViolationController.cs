using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class ViolationController : Controller
    {
        private readonly AppDbContext _db;
        public ViolationController(AppDbContext db) { _db = db; }
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index(string? status)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var query = _db.Violations.Include(v => v.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(v => v.Status == status);

            ViewBag.Status = status;
            return View(await query.OrderByDescending(v => v.ViolationDate).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Resolve(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var violation = await _db.Violations.FindAsync(id);
            if (violation != null)
            {
                violation.Status = "DaXuLy";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xử lý vi phạm!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
