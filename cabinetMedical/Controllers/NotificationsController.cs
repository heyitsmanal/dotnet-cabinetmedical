using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cabinetMedical.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace cabinetMedical.Controllers
{
    [Authorize(Roles = "admin,infirmiere,medecin,patient")]
    public class NotificationsController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public NotificationsController(MedicalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            // Get the authenticated user's ID
            var userId = _userManager.GetUserId(User);

            // Check the roles of the authenticated user
            var isPatient = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Patient");
            var isMedecin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Medecin");
            var isAdmin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Admin");

            // Fetch all notifications
            var notifications = await _context.Notifications
                .Select(n => new
                {
                    n.Id,
                    n.Message,
                    n.Date,
                    MedecinName = _context.Medecins.Where(m => m.Id == n.MedecinId).Select(m => m.Nom).FirstOrDefault(),
                    PatientName = _context.Patients.Where(p => p.Id == n.PatientId).Select(p => p.Nom).FirstOrDefault(),
                    n.MedecinId,
                    n.PatientId
                })
                .ToListAsync();

            // Pass the data to the view using ViewBag
            ViewBag.Notifications = notifications;
            ViewBag.UserId = userId;  // Pass the user ID
            ViewBag.IsPatient = isPatient;  // Pass if the user is a patient
            ViewBag.IsMedecin = isMedecin;  // Pass if the user is a doctor
            ViewBag.IsAdmin = isAdmin;  // Pass if the user is an admin

            return View();
        }




        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the notification
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            // Fetch the Medecin and Patient names based on the MedecinId and PatientId
            var medecinNom = await _context.Medecins
                .Where(m => m.Id == notification.MedecinId)
                .Select(m => m.Nom)
                .FirstOrDefaultAsync();

            var patientNom = await _context.Patients
                .Where(p => p.Id == notification.PatientId)
                .Select(p => p.Nom)
                .FirstOrDefaultAsync();

            // Pass the notification and the names to the view
            ViewData["MedecinNom"] = medecinNom;
            ViewData["PatientNom"] = patientNom;

            return View(notification);
        }



        // GET: Notifications/Create
        public IActionResult Create()
        {
            // Load consultations for dropdown
            ViewBag.Consultations = new SelectList(
                _context.Consultations.Select(c => new
                {
                    Id = c.Id,
                    Display = (c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "No Date") + " - " + c.Patient.Nom
                }),
                "Id",
                "Display");

            var consultation = _context.Consultations.FirstOrDefault(); // Default consultation
            var message = consultation != null && consultation.Date.HasValue
                ? $"Votre rendez-vous de {consultation.Date.Value.ToString("dd/MM/yyyy")} est à venir."
                : "Votre rendez-vous de (date) est à venir.";

            var notification = new Notification
            {
                Message = message
            };

            return View(notification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int consultationId)
        {
            // Fetch the consultation to retrieve related data
            var consultation = await _context.Consultations
                .Include(c => c.Médecin)
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(c => c.Id == consultationId);

            if (consultation == null)
            {
                ModelState.AddModelError("", "Invalid Consultation ID.");
                ViewBag.Consultations = new SelectList(
                    _context.Consultations.Select(c => new
                    {
                        Id = c.Id,
                        Display = (c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "No Date") + " - " + c.Patient.Nom
                    }),
                    "Id",
                    "Display");
                return View();
            }

            // Create notification
            var notification = new Notification
            {
                Message = $"Votre rendez-vous de {consultation.Date:dd/MM/yyyy} est à venir.",
                Date = DateTime.Now,
                MedecinId = consultation.Médecin.Id,
                PatientId = consultation.Patient.Id
            };

            // Add to database
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Consultations = new SelectList(
                _context.Consultations.Select(c => new
                {
                    Id = c.Id,
                    Display = (c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "No Date") + " - " + c.Patient.Nom
                }),
                "Id",
                "Display");

            return View(notification);
        }



        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            // Fetch Medecins and Patients to populate dropdowns
            ViewData["Medecins"] = new SelectList(await _context.Medecins.ToListAsync(), "Id", "Nom", notification.MedecinId);
            ViewData["Patients"] = new SelectList(await _context.Patients.ToListAsync(), "Id", "Nom", notification.PatientId);

            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,Date,MedecinId,PatientId")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Reload the drop-down lists if the model is not valid
            ViewData["Medecins"] = new SelectList(await _context.Medecins.ToListAsync(), "Id", "Nom", notification.MedecinId);
            ViewData["Patients"] = new SelectList(await _context.Patients.ToListAsync(), "Id", "Nom", notification.PatientId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }



        public async Task<IActionResult> PatientCreateNotification([Bind("Message, MedecinId, Date")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                // Set the Date to the current date and time directly
                notification.Date = DateTime.Now;

                // Fetch the logged-in user's ID (which is the Id from the AspNetUsers table)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Fetch the PatientId associated with the logged-in user
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == userId);

                if (patient == null)
                {
                    // Handle the case where the patient does not exist
                    ModelState.AddModelError("PatientId", "No patient found for the logged-in user.");
                    return View(notification);
                }

                // Assign the PatientId to the notification
                notification.PatientId = patient.Id;  // Assuming 'Id' is the PatientId

                // Add the notification to the database
                _context.Add(notification);
                await _context.SaveChangesAsync();

                // Redirect to the patient dashboard or another appropriate page
                return RedirectToAction("Dashboard", "Patients");
            }

            // Fetch the list of Medecins
            var medecins = await _context.Medecins.ToListAsync();

            // Create a list of anonymous objects with Nom and Spécialité
            var medecinList = medecins.Select(m => new
            {
                Id = m.Id,
                DisplayText = $"{m.Nom} ({m.Spécialité})"
            }).ToList();

            // Populate the drop-down list for Medecins
            ViewData["MédecinId"] = new SelectList(medecinList, "Id", "DisplayText");

            return View(notification);
        }





    }
}
