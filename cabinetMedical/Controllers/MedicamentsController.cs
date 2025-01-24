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

namespace cabinetMedical.Controllers
{
    [Authorize(Roles = "medecin")]
    public class MedicamentsController : Controller
    {
       
        private readonly MedicalContext _context;

        public MedicamentsController(MedicalContext context)
        {
            _context = context;
        }

        // GET: Medicaments
        public async Task<IActionResult> Index()
        {
            var medicalContext = _context.Medicaments.Include(m => m.Consultation);
            return View(await medicalContext.ToListAsync());
        }

        // GET: Medicaments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicament = await _context.Medicaments
                .Include(m => m.Consultation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicament == null)
            {
                return NotFound();
            }

            return View(medicament);
        }

        // GET: Medicaments/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom");
            return View();
        }

        // POST: Medicaments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Description")] Medicament medicament, string patientId)
        {
            if (ModelState.IsValid)
            {
                // Fetch the latest Consultation for the given PatientId based on the date
                var latestConsultation = await _context.Consultations
                    .Where(c => c.PatientId == patientId)
                    .OrderByDescending(c => c.Date) // Get the most recent consultation by date
                    .FirstOrDefaultAsync();

                if (latestConsultation != null)
                {
                    medicament.ConsultationId = latestConsultation.Id; // Automatically set the ConsultationId
                }
                else
                {
                    ModelState.AddModelError("patientId", "No consultations found for the selected patient.");
                    ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", patientId);
                    return View(medicament);
                }

                _context.Add(medicament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Populate the Patient dropdown
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Nom", patientId);
            return View(medicament);
        }



        // GET: Medicaments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicament = await _context.Medicaments.FindAsync(id);
            if (medicament == null)
            {
                return NotFound();
            }
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "Id", "Id", medicament.ConsultationId);
            return View(medicament);
        }

        // POST: Medicaments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Description,ConsultationId")] Medicament medicament)
        {
            if (id != medicament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentExists(medicament.Id))
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
            ViewData["ConsultationId"] = new SelectList(_context.Consultations, "Id", "Id", medicament.ConsultationId);
            return View(medicament);
        }

        // GET: Medicaments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicament = await _context.Medicaments
                .Include(m => m.Consultation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicament == null)
            {
                return NotFound();
            }

            return View(medicament);
        }

        // POST: Medicaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicament = await _context.Medicaments.FindAsync(id);
            if (medicament != null)
            {
                _context.Medicaments.Remove(medicament);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentExists(int id)
        {
            return _context.Medicaments.Any(e => e.Id == id);
        }
    }
}
