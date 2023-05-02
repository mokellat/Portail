using LinkedinClone.Areas.Identity.Data;
using LinkedinClone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace LinkedinClone.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ILogger<CandidateController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public CandidateController(ILogger<CandidateController> logger, IWebHostEnvironment hostingEnvironment, IConfiguration configuration, AppDbContext context)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _configuration = configuration;
        }

        // GET: Candidate
        public async Task<IActionResult> Index(string searchString)
        {
            var candidates = from c in _context.Candidates
                             select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                candidates = candidates.Where(c => c.FirstName.Contains(searchString)
                                       || c.LastName.Contains(searchString)
                                       || c.Email.Contains(searchString));
            }

            return View(await candidates.ToListAsync());
        }

        // GET: Candidate/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candidate/Step1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step1([Bind("FirstName,LastName,Email,Phone,EducationLevel,YearsOfExperience,LastEmployer")] Candidate candidate)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(candidate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Step2), new { id = candidate.Id});
            //}
            return View("Create", candidate);
        }

        // GET: Candidate/Step2/5
        public IActionResult Step2(int id)
        {
            var candidate = _context.Candidates.FirstOrDefault(c => c.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }
            //ViewBag.Candidate = candidate;
            return View(candidate);
        }

        // POST: Candidate/Step2/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step2(int id, Candidate candidate, [FromForm(Name = "cvFile")] IFormFile cvFile)
        {
            if (id != candidate.Id)
            {
                return NotFound();
            }

            if (cvFile != null)
            {
                var fileName = $"{candidate.FirstName}-{candidate.LastName}.pdf";
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(stream);
                }

                candidate.CvFilePath = fileName;
                _context.Update(candidate);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Step3), new { id = candidate.Id });
            }
            else
            {
                ModelState.AddModelError("cvFile", "Please upload your CV.");
            }

            ViewBag.Candidate = candidate;
            return View();
        }


        //GET: Candidate/Step3/5
        public async Task<IActionResult> Step3(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step3(Candidate candidate)
        {
            //var candidate = await _context.Candidates.FindAsync(candidate.Id);
            //if (ModelState.IsValid)
            //{
            var smtpSection = _configuration.GetSection("SmtpSettings");

                var smtpClient = new SmtpClient(smtpSection["Server"])
                {
                    Port = int.Parse(smtpSection["Port"]),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                    EnableSsl = bool.Parse(smtpSection["UseSsl"]),
                };

                var message = new MailMessage
                {
                    From = new MailAddress(smtpSection["Username"]),
                    Subject = "Application Submitted",
                    Body = $"Dear {candidate.FirstName} {candidate.LastName},<br><br>Your application has been submitted successfully.<br><br>Best regards,<br>My Company"
                };

                message.To.Add(candidate.Email);

                message.IsBodyHtml = true;

                await smtpClient.SendMailAsync(message);

                //return View();
            //}

            return Redirect("/Home/Index");
        }

        // POST: Candidate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            // delete uploaded files
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            if (candidate.CvFilePath != null)
            {
                var cvPath = Path.Combine(uploadsFolder, candidate.CvFilePath);
                if (System.IO.File.Exists(cvPath))
                {
                    System.IO.File.Delete(cvPath);
                }
            }

            //if (candidate.CoverLetterPath != null)
            //{
            //    var coverLetterPath = Path.Combine(uploadsFolder, candidate.CoverLetterPath);
            //    if (System.IO.File.Exists(coverLetterPath))
            //    {
            //        System.IO.File.Delete(coverLetterPath);
            //    }
            //}

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidates.Any(c => c.Id == id);
        }
    }
}
