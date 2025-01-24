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
    [Authorize(Roles = "medecin,infirmiere,patient")]
    public class ConsultationsController : Controller
    {
        
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ConsultationsController(MedicalContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Consultations
        public async Task<IActionResult> Index()
        {
            var medicalContext = _context.Consultations.Include(c => c.DossierMédical).Include(c => c.Médecin).Include(c => c.Patient);
            return View(await medicalContext.ToListAsync());
        }

        // GET: Consultations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
                .Include(c => c.DossierMédical)
                .Include(c => c.Médecin)
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        // GET: Consultations/Create
        public IActionResult Create()
        {
            // Populate the select lists with patient names and doctors' names for display
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom"); // Médecin dropdown shows Nom
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom"); // Patient dropdown shows Nom
            return View();
        }

        // POST: Consultations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientId,MédecinId,Date")] Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                // Fetch the DossierMedical for the selected PatientId
                var dossierMedical = await _context.DossiersMedicals
                    .FirstOrDefaultAsync(dm => dm.PatientId == consultation.PatientId);

                if (dossierMedical != null)
                {
                    // Automatically set the DossierMédicalId based on the found DossierMedical
                    consultation.DossierMédicalId = dossierMedical.Id;
                }
                else
                {
                    // If no DossierMedical is found, add an error and return the view
                    ModelState.AddModelError("PatientId", "No dossier médical found for the selected patient.");
                    ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom", consultation.MédecinId);
                    ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", consultation.PatientId);
                    return View(consultation);
                }

                // Add the consultation to the database
                _context.Add(consultation);
                await _context.SaveChangesAsync();

                // Redirect to the list of consultations
                return RedirectToAction(nameof(Index));
            }

            // If model is invalid, populate dropdowns again before returning to the view
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom", consultation.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", consultation.PatientId);
            return View(consultation);
        }



        // GET: Consultations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null)
            {
                return NotFound();
            }

            // Update to pass names instead of Ids
            ViewData["DossierMédicalId"] = new SelectList(_context.DossiersMedicals, "Id", "Id", consultation.DossierMédicalId);

            // For Médecin and Patient, we display the Nom instead of Id
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom", consultation.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", consultation.PatientId);

            return View(consultation);
        }

        // POST: Consultations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,MédecinId,Date,DossierMédicalId")] Consultation consultation)
        {
            if (id != consultation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the consultation in the database
                    _context.Update(consultation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultationExists(consultation.Id))
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

            // Re-load the SelectList with 'Nom' for Médecin and Patient
            ViewData["DossierMédicalId"] = new SelectList(_context.DossiersMedicals, "Id", "Id", consultation.DossierMédicalId);
            ViewData["MédecinId"] = new SelectList(_context.Medecins, "Id", "Nom", consultation.MédecinId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", consultation.PatientId);

            return View(consultation);
        }

        // GET: Consultations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consultation = await _context.Consultations
                .Include(c => c.DossierMédical)
                .Include(c => c.Médecin)
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (consultation == null)
            {
                return NotFound();
            }

            return View(consultation);
        }

        // POST: Consultations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation != null)
            {
                _context.Consultations.Remove(consultation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultationExists(int id)
        {
            return _context.Consultations.Any(e => e.Id == id);
        }








        public async Task<IActionResult> PatientCons()
        {
            // Get the authenticated user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch consultations for the authenticated patient
            var consultations = await _context.Consultations
                .Where(c => c.PatientId == userId)
                .Include(c => c.Médecin) // Include the doctor if needed
                .Include(c => c.DossierMédical) // Eagerly load DossierMédical
                .Include(c=>c.Medicaments)
                .ToListAsync();

            return View(consultations); // Pass the consultations to the view
        }


        




    }
}
