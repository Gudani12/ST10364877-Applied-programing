using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

using ST10364877.Models;
using ST10364877;

namespace Zee_ST10379243_Part2.Controllers
{
    public class HomeController : Controller
    {
        private readonly The_DBContext _context;

        public HomeController(The_DBContext context)
        {
            _context = context;
        }

        // Home/Index - Main landing page
        public IActionResult Index()
        {
            return View();
        }
        // Report page
        [HttpGet]
        [Authorize]
        public IActionResult Report()
        {
            return View(); // Make sure Views/Home/Report.cshtml exists
        }

        // Create page (if you want a generic Create page)
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View(); // Make sure Views/Home/Create.cshtml exists
        }

        // Authentication Actions
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return View(model);
                }

                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                // Create new user
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Incident Actions
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Incidents()
        {
            var incidents = await _context.IncidentReports
                .Include(i => i.User)
                .OrderByDescending(i => i.ReportedAt)
                .ToListAsync();

            return View(incidents);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ReportIncident()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIncident(IncidentReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    // Create new IncidentReport from ViewModel
                    var incident = new IncidentReport
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Location = model.Location,
                        IncidentDate = model.IncidentDate,
                        DisasterType = model.DisasterType,
                        AffectedAreas = model.AffectedAreas,
                        UrgencyLevel = model.UrgencyLevel,
                        UserId = userId,
                        ReportedAt = DateTime.UtcNow,
                        Status = "Pending"
                    };

                    _context.IncidentReports.Add(incident);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Incident report submitted successfully!";
                    return RedirectToAction("Incidents");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the incident report. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IncidentDetails(int id)
        {
            var incident = await _context.IncidentReports
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.IncidentId == id);

            if (incident == null)
            {
                return NotFound();
            }

            return View(incident);
        }

        // Donation Actions
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Donations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var donations = await _context.Donations
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            return View(donations);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateDonation()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDonation(DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    // Create new Donation from ViewModel
                    var donation = new Donation
                    {
                        DonationType = model.DonationType,
                        ItemDescription = model.ItemDescription,
                        Quantity = model.Quantity,
                        Unit = model.Unit,
                        TargetArea = model.TargetArea,
                        SpecialInstructions = model.SpecialInstructions,
                        UserId = userId,
                        DonationDate = DateTime.UtcNow,
                        Status = "Pending"
                    };

                    _context.Donations.Add(donation);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Donation submitted successfully! Thank you for your generosity.";
                    return RedirectToAction("Donations");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your donation. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AllDonations()
        {
            var donations = await _context.Donations
                .Include(d => d.User)
                .OrderByDescending(d => d.DonationDate)
                .ToListAsync();

            return View(donations);
        }

        // Volunteer Actions
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Volunteers()
        {
            var volunteers = await _context.Volunteers
                .Include(v => v.User)
                .Where(v => v.Status == "Active")
                .ToListAsync();

            return View(volunteers);
        }

        [HttpGet]
        [Authorize]
        public IActionResult RegisterVolunteer()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterVolunteer(VolunteerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    // Check if user is already registered as volunteer
                    var existingVolunteer = await _context.Volunteers
                        .FirstOrDefaultAsync(v => v.UserId == userId);

                    if (existingVolunteer != null)
                    {
                        TempData["ErrorMessage"] = "You are already registered as a volunteer.";
                        return RedirectToAction("MyVolunteerRegistration");
                    }

                    // Create new Volunteer from ViewModel
                    var volunteer = new Volunteer
                    {
                        Skills = model.Skills,
                        Availability = model.Availability,
                        HasTransportation = model.HasTransportation,
                        PreferredLocation = model.PreferredLocation,
                        EmergencyContact = model.EmergencyContact,
                        UserId = userId,
                        RegisteredAt = DateTime.UtcNow,
                        Status = "Active"
                    };

                    _context.Volunteers.Add(volunteer);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Volunteer registration successful! Thank you for your commitment.";
                    return RedirectToAction("MyVolunteerRegistration");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving your volunteer registration. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyVolunteerRegistration()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var volunteer = await _context.Volunteers
                .FirstOrDefaultAsync(v => v.UserId == userId);

            return View(volunteer);
        }
    }
}