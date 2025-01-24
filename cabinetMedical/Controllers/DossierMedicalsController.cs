using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cabinetMedical.Models;
using cabinetMedical.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace cabinetMedical.Controllers
{
    [Authorize(Roles = "medecin,patient")]
    public class DossierMedicalsController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DossierMedicalsController(MedicalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DossierMedicals
        public async Task<IActionResult> Index()
        {
            var medicalContext = _context.DossiersMedicals.Include(d => d.Médecin).Include(d => d.Patient);
            return View(await medicalContext.ToListAsync());
        }

        // GET: DossierMedicals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossiersMedicals
                .Include(d => d.Médecin)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dossierMedical == null)
            {
                return NotFound();
            }

            return View(dossierMedical);
        }

        // GET: DossierMedicals/Create
        public IActionResult Create()
        {
            // Use "Nom" for display instead of "Id"
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom");  // Display Nom for Médecin
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom");  // Display Nom for Patient
            return View();
        }
        // POST: DossierMedicals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientId,MédecinId,Description")] DossierMedical dossierMedical)
        {
            if (ModelState.IsValid)
            {
                dossierMedical.DateCréation = DateTime.Now; // Automatically set the creation date
                _context.Add(dossierMedical);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate the SelectList in case of an invalid model
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom", dossierMedical.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", dossierMedical.PatientId);
            return View(dossierMedical);
        }


        // GET: DossierMedicals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossiersMedicals.FindAsync(id);
            if (dossierMedical == null)
            {
                return NotFound();
            }
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Id", dossierMedical.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", dossierMedical.PatientId);
            return View(dossierMedical);
        }

        // POST: DossierMedicals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,MédecinId,Description,DateCréation")] DossierMedical dossierMedical)
        {
            if (id != dossierMedical.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dossierMedical);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DossierMedicalExists(dossierMedical.Id))
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
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Id", dossierMedical.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Id", dossierMedical.PatientId);
            return View(dossierMedical);
        }

        // GET: DossierMedicals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dossierMedical = await _context.DossiersMedicals
                .Include(d => d.Médecin)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dossierMedical == null)
            {
                return NotFound();
            }

            return View(dossierMedical);
        }

        // POST: DossierMedicals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dossierMedical = await _context.DossiersMedicals.FindAsync(id);
            if (dossierMedical != null)
            {
                _context.DossiersMedicals.Remove(dossierMedical);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DossierMedicalExists(int id)
        {
            return _context.DossiersMedicals.Any(e => e.Id == id);
        }







        // GET: DossierMedical/PatientDossMed
        public async Task<IActionResult> PatientDossMed()
        {
            // Get the authenticated user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch the dossier medical for the authenticated patient
            var dossierMedical = await _context.DossiersMedicals
                .Where(dm => dm.PatientId == userId)
                .Include(dm => dm.Médecin) // Include the doctor (Médecin) explicitly
                .Include(dm => dm.Consultations) // Include consultations if needed
                .FirstOrDefaultAsync();

            return View(dossierMedical); // Return the dossier medical to the view
        }






    }
}
