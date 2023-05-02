using LinkedinClone.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LinkedinClone.Models;
using PagedList;

namespace LinkedinClone.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(ILogger<AdminController> logger, IWebHostEnvironment hostingEnvironment, AppDbContext context)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public  IActionResult Index(string searchString, int? page)
        {
            var candidates = from c in _context.Candidates
                             select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                candidates = candidates.Where(c =>
                    c.FirstName.Contains(searchString) ||
                    c.LastName.Contains(searchString) ||
                    c.Email.Contains(searchString) ||
                    c.Phone.Contains(searchString));
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(candidates.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var candidates = from c in _context.Candidates
                             select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                candidates = candidates.Where(c => c.FirstName.Contains(searchString)
                                                    || c.LastName.Contains(searchString)
                                                    || c.Email.Contains(searchString)
                                                    || c.Phone.Contains(searchString));
            }

            return View(await candidates.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewCV(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var candidate = await _context.Candidates.FirstOrDefaultAsync(m => m.Id == id);
                if (candidate == null)
                {
                    return NotFound();
                }
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", candidate.CvFilePath);
                if (!System.IO.File.Exists(filePath))
                {
                    // File not found, return a 404 Not Found response
                    return NotFound();
                }

                return PhysicalFile(filePath, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while viewing the CV.");
                throw;
            }
        }
    }
}
