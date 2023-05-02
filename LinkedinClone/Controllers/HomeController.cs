using LinkedinClone.Areas.Identity.Data;
using LinkedinClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LinkedinClone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _context;

        private readonly string _cvUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cv");


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public IActionResult Admin()
        //{
        //    var applications = _context.Candidates.ToList();
        //    return View(applications);
        //}

        //public IActionResult Download(int id)
        //{
        //    var application = _context.Candidates.Find(id);
        //    if (application == null)
        //    {
        //        return NotFound();
        //    }

        //    var cvFilePath = Path.Combine(_cvUploadPath, application.CvFilePath);
        //    if (!System.IO.File.Exists(cvFilePath))
        //    {
        //        return NotFound();
        //    }

        //    var fileStream = System.IO.File.OpenRead(cvFilePath);
        //    return File(fileStream, "application/pdf"); // return the file as a PDF
        //}

        //public IActionResult Delete(int id)
        //{
        //    var application = _context.Candidates.Find(id);
        //    if (application == null)
        //    {
        //        return NotFound();
        //    }

        //    var cvFilePath = Path.Combine(_cvUploadPath, application.CvFilePath);
        //    if (System.IO.File.Exists(cvFilePath))
        //    {
        //        System.IO.File.Delete(cvFilePath);
        //    }

        //    _context.Candidates.Remove(application);
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}
    }
}