using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThuVienSo.Data;

namespace ThuVienSo.Controllers
{
    public class BorrowController : Controller
    {
        private readonly AppDbContext _db;
        public BorrowController(AppDbContext db) { _db = db; }
        private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

        public async Task<IActionResult> Index(string? status, string? searchName, string? searchPhone, string? searchBookTitle, string? searchBookId, DateTime? fromDate, DateTime? toDate)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            // Auto update overdue
            var overdue = await _db.BorrowRecords
                .Where(br => br.Status == "DangMuon" && br.DueDate < DateTime.Today)
                .ToListAsync();
            foreach (var br in overdue) br.Status = "QuaHan";
            if (overdue.Any()) await _db.SaveChangesAsync();

            var query = _db.BorrowRecords.Include(br => br.Book).Include(br => br.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(br => br.Status == status);
            if (!string.IsNullOrWhiteSpace(searchName))
                query = query.Where(br => br.User != null && br.User.FullName.Contains(searchName));
            if (!string.IsNullOrWhiteSpace(searchPhone))
                query = query.Where(br => br.User != null && br.User.PhoneNumber != null && br.User.PhoneNumber.Contains(searchPhone));
            if (!string.IsNullOrWhiteSpace(searchBookTitle))
                query = query.Where(br => br.Book != null && br.Book.BookTitle.Contains(searchBookTitle));
            if (!string.IsNullOrWhiteSpace(searchBookId))
                query = query.Where(br => br.BookId.Contains(searchBookId));
            if (fromDate.HasValue)
                query = query.Where(br => br.BorrowDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(br => br.BorrowDate.Date <= toDate.Value.Date);

            ViewBag.Status = status;
            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchBookTitle = searchBookTitle;
            ViewBag.SearchBookId = searchBookId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.DueSoon = await _db.BorrowRecords
                .Where(br => br.Status == "DangMuon" && br.DueDate <= DateTime.Today.AddDays(3))
                .CountAsync();

            return View(await query.OrderByDescending(br => br.BorrowDate).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            ViewBag.CreatedBy = HttpContext.Session.GetString("FullName");

            var dsSach = await _db.Books.Where(b => b.Quantity > 0).OrderBy(b => b.BookTitle).ToListAsync();
            ViewBag.Books = new SelectList(dsSach, "BookId", "BookTitle");

            var dsDocGia = await _db.Users.Where(u => u.Role == "Reader" && u.Status == "Active").OrderBy(u => u.FullName).ToListAsync();
            ViewBag.Users = new SelectList(dsDocGia, "UserId", "FullName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BorrowRecord model)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var book = await _db.Books.FindAsync(model.BookId);
            if (book == null || book.Quantity <= 0)
                ModelState.AddModelError("", "Sách không còn trong kho.");

            var user = await _db.Users.FindAsync(model.UserId);
            if (user?.Status == "Locked")
                ModelState.AddModelError("", "Tài khoản độc giả đang bị khóa.");

            if (ModelState.IsValid)
            {
                model.BorrowId = "PM" + DateTime.Now.ToString("yyyyMMddHHmmss");
                model.BorrowDate = DateTime.Now;
                model.Status = "DangMuon";
                model.CreatedBy = HttpContext.Session.GetString("FullName");
                book!.Quantity--;

                _db.BorrowRecords.Add(model);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Tạo phiếu mượn thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CreatedBy = HttpContext.Session.GetString("FullName");

            var dsSach2 = await _db.Books.Where(b => b.Quantity > 0).OrderBy(b => b.BookTitle).ToListAsync();
            ViewBag.Books = new SelectList(dsSach2, "BookId", "BookTitle");

            var dsDocGia2 = await _db.Users.Where(u => u.Role == "Reader" && u.Status == "Active").OrderBy(u => u.FullName).ToListAsync();
            ViewBag.Users = new SelectList(dsDocGia2, "UserId", "FullName");

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> ProcessReturn(string borrowId, string bookCondition)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            var record = await _db.BorrowRecords
                .Include(br => br.Book).Include(br => br.User)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId);
            if (record == null) return NotFound();

            record.ReturnDate = DateTime.Now;
            bool isLate = record.ReturnDate > record.DueDate;
            record.Status = isLate ? "DaTraMuon" : "DaTraDung";

            if (record.Book != null) record.Book.Quantity++;

            if (isLate || bookCondition != "Good")
            {
                var violation = new Violation
                {
                    UserId = record.UserId,
                    BookId = record.BookId,
                    ViolationDate = DateTime.Now
                };

                if (isLate && bookCondition == "Good")
                {
                    violation.ViolationName = "Trả sách quá hạn";
                    violation.PenaltyType = "CanhCao";
                }
                else if (bookCondition == "Torn") { violation.ViolationName = "Sách bị rách"; violation.PenaltyType = "DenTien"; violation.PenaltyAmount = record.Book?.Price * 0.5m; }
                else if (bookCondition == "Lost") { violation.ViolationName = "Sách bị mất"; violation.PenaltyType = "DenSach"; violation.PenaltyAmount = record.Book?.Price; }
                else if (bookCondition == "Wet") { violation.ViolationName = "Sách bị ướt"; violation.PenaltyType = "DenTien"; violation.PenaltyAmount = record.Book?.Price * 0.3m; }

                _db.Violations.Add(violation);

                if (record.User != null)
                {
                    record.User.ViolationCount++;
                    if (record.User.ViolationCount >= 3)
                    {
                        record.User.Status = "Locked";
                        TempData["Warning"] = $"Độc giả {record.User.FullName} đã bị khóa tài khoản do vi phạm từ 3 lần trở lên!";
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Xử lý trả sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var record = await _db.BorrowRecords.FindAsync(id);
            if (record != null) { _db.BorrowRecords.Remove(record); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Xóa phiếu mượn thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DueSoon()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var list = await _db.BorrowRecords
                .Include(br => br.Book).Include(br => br.User)
                .Where(br => br.Status == "DangMuon" && br.DueDate <= DateTime.Today.AddDays(3))
                .OrderBy(br => br.DueDate)
                .ToListAsync();
            return View(list);
        }
    }
}
