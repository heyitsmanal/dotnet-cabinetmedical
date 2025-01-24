using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cabinetMedical.Models;
using Microsoft.AspNetCore.Identity;
using cabinetMedical.Data;
using Microsoft.AspNetCore.Authorization;

namespace cabinetMedical.Controllers
{
    [Authorize(Roles = "admin,patient")]
    public class PatientsController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PatientsController(MedicalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string search)
        {
            var patients = from p in _context.Patients
                           .Include(p => p.IdentityUser) // Include the related IdentityUser data
                           select p;

            if (!string.IsNullOrEmpty(search))
            {
                patients = patients.Where(p => p.Nom.Contains(search)); // Filter by 'Nom' or any other property you want to search by
            }

            return View(await patients.ToListAsync());
        }









        public async Task<IActionResult> Dashboard()
        {
            // Add logic specific to the Patient dashboard
            ViewData["Title"] = "Patient Dashboard";

            // Get the currently logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the patient's own Dossier Medical and Consultations
            var dossierMedicalCount = await _context.DossiersMedicals.CountAsync(dm => dm.PatientId == userId);
            var consultationsCount = await _context.Consultations.CountAsync(c => c.PatientId == userId);
            var FacturesCount = await _context.Factures.CountAsync(f => f.PatientId == userId);

            // Pass counts to the view using ViewData or ViewBag
            ViewBag.DossierMedicalCount = dossierMedicalCount;
            ViewBag.ConsultationsCount = consultationsCount;
            ViewBag.FacturesCount = FacturesCount;

            return View();
        }





        // GET: Patients/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.IdentityUser) // Include the related IdentityUser data
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            // Populate a SelectList with IdentityUser data for selection
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Adresse,IdentityUserId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _context.Users.FindAsync(patient.IdentityUser.Id);
                if (identityUser != null)
                {
                    patient.Id = identityUser.Id; // Set the Patient's Id to the IdentityUser Id
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            // Re-populate the SelectList in case of an invalid form
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", patient.IdentityUser);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.IdentityUser) // Include the related IdentityUser data
                .FirstOrDefaultAsync(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", patient.Id);
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom,Adresse,IdentityUserId")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var identityUser = await _context.Users.FindAsync(patient.IdentityUser.Id);
                    if (identityUser != null)
                    {
                        patient.Id = identityUser.Id; // Set the Patient's Id to the IdentityUser Id
                        _context.Update(patient);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", patient.IdentityUser.Id);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.IdentityUser) // Include the related IdentityUser data
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(string id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
