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
    [Authorize(Roles = "admin,medecin,infirmiere")]
    public class PlanningsController : Controller
    {
        private readonly MedicalContext _context;

        public PlanningsController(MedicalContext context)
        {
            _context = context;
        }

        // GET: Plannings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Plannings.ToListAsync());
        }

        // GET: Plannings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (planning == null)
            {
                return NotFound();
            }

            return View(planning);
        }

        // GET: Plannings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plannings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,DateDébut,DateFin")] Planning planning)
        {
            if (ModelState.IsValid)
            {
                _context.Add(planning);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(planning);
        }

        // GET: Plannings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings.FindAsync(id);
            if (planning == null)
            {
                return NotFound();
            }
            return View(planning);
        }

        // POST: Plannings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,DateDébut,DateFin")] Planning planning)
        {
            if (id != planning.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planning);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanningExists(planning.Id))
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
            return View(planning);
        }

        // GET: Plannings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planning = await _context.Plannings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (planning == null)
            {
                return NotFound();
            }

            return View(planning);
        }

        // POST: Plannings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planning = await _context.Plannings.FindAsync(id);
            if (planning != null)
            {
                _context.Plannings.Remove(planning);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanningExists(int id)
        {
            return _context.Plannings.Any(e => e.Id == id);
        }
    }
}
