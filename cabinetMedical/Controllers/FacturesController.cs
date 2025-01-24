using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CabinetMedical.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using cabinetMedical.Models;

namespace cabinetMedical.Controllers
{
    [Authorize(Roles = "patient,infirmiere")]

    public class FacturesController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public FacturesController(MedicalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Factures
        public async Task<IActionResult> Index()
        {
            // Fetch factures
            var factures = await _context.Factures.ToListAsync();

            // Fetch patient names
            ViewBag.PatientNames = await _context.Patients
                .ToDictionaryAsync(p => p.Id, p => p.Nom);

            // Fetch consultation dates
            ViewBag.ConsultationDates = await _context.Consultations
                .ToDictionaryAsync(c => c.Id, c => c.Date);

            return View(factures);
        }


        // GET: Factures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the facture
            var facture = await _context.Factures
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facture == null)
            {
                return NotFound();
            }

            // Fetch additional details
            ViewBag.PatientName = await _context.Patients
                .Where(p => p.Id == facture.PatientId)
                .Select(p => p.Nom)
                .FirstOrDefaultAsync();

            ViewBag.ConsultationDate = await _context.Consultations
                .Where(c => c.Id == facture.ConsultationId)
                .Select(c => c.Date)
                .FirstOrDefaultAsync();

            return View(facture);
        }





        // GET: Factures/Create
        public IActionResult Create()
        {
            // Populate the dropdown for Patients
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "Nom");

            // Populate the dropdown for Consultations
            ViewBag.Consultations = new SelectList(_context.Consultations, "Id", "Date");

            return View();
        }

        // POST: Factures/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Date,PatientId,ConsultationId")] Facture facture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns in case of validation error
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "Nom", facture.PatientId);
            ViewBag.Consultations = new SelectList(_context.Consultations, "Id", "Date", facture.ConsultationId);

            return View(facture);
        }








        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                return NotFound();
            }

            // Populate dropdown for Patients
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "Nom", facture.PatientId);

            // Populate dropdown for Consultations
            ViewBag.Consultations = new SelectList(
                _context.Consultations.Select(c => new
                {
                    c.Id,
                    Display = (c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "No Date") + " - " +
                              _context.Patients.Where(p => p.Id == c.PatientId).Select(p => p.Nom).FirstOrDefault()
                }),
                "Id",
                "Display",
                facture.ConsultationId);

            return View(facture);
        }









        // POST: Factures/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Date,PatientId,ConsultationId")] Facture facture)
        {
            if (id != facture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FactureExists(facture.Id))
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

            // Repopulate dropdowns if validation fails
            ViewBag.Patients = new SelectList(_context.Patients, "Id", "Nom", facture.PatientId);
            ViewBag.Consultations = new SelectList(
                _context.Consultations.Select(c => new
                {
                    c.Id,
                    Date = c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "No Date"
                }),
                "Id",
                "Date",
                facture.ConsultationId);

            return View(facture);
        }










        // GET: Factures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facture = await _context.Factures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facture == null)
            {
                return NotFound();
            }

            ViewBag.ConsultationDate = await _context.Consultations
                .Where(c => c.Id == facture.ConsultationId)
                .Select(c => c.Date)
                .FirstOrDefaultAsync();

            return View(facture);
        }

        // POST: Factures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facture = await _context.Factures.FindAsync(id);
            if (facture != null)
            {
                _context.Factures.Remove(facture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FactureExists(int id)
        {
            return _context.Factures.Any(e => e.Id == id);
        }




        public async Task<IActionResult> PatientFactures()
        {
            // Get the authenticated user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the factures for the authenticated patient
            var factures = await _context.Factures
                .Where(f => f.PatientId == userId)
                .ToListAsync();

            // Fetch the patient's name
            var patientName = await _context.Patients
                .Where(p => p.Id == userId)
                .Select(p => p.Nom)
                .FirstOrDefaultAsync();

            // Pass the patient's name and factures to the view
            ViewBag.PatientName = patientName;

            return View("PatientFactures", factures);  // Ensure the view name is correct
        }








    }
}
