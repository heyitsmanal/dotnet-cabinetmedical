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

namespace cabinetMedical.Controllers
{
    public class AdminsController : Controller
    {
        private readonly MedicalContext _context;

        public AdminsController(MedicalContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            var admins = await _context.Admins
                .Include(a => a.IdentityUser)  // Include related IdentityUser
                .ToListAsync();
            return View(admins);
        }




        public IActionResult Dashboard()
        {
            // Add logic specific to the Admin dashboard
            ViewData["Title"] = "Admin Dashboard";

            // Fetch counts from the database
            var planningCount = _context.Plannings.Count();
            var medecinCount = _context.Medecins.Count();
            var infermiereCount = _context.Infirmieres.Count();
            var patientCount = _context.Patients.Count(); // Add Patient count

            // Pass counts to the view using ViewData or ViewBag
            ViewBag.PlanningCount = planningCount;
            ViewBag.MedecinCount = medecinCount;
            ViewBag.InfermiereCount = infermiereCount;
            ViewBag.PatientCount = patientCount; // Pass Patient count to ViewBag

            return View();
        }



        // GET: Admins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.IdentityUser)  // Include related IdentityUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            // Populate a SelectList with IdentityUser data for selection
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,IdentityUserId")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _context.Users.FindAsync(admin.IdentityUser.Id);
                if (identityUser != null)
                {
                    admin.Id = identityUser.Id;  // Ensure Admin uses the correct IdentityUser Id
                    _context.Add(admin);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", admin.IdentityUser.Id);
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.IdentityUser)  // Include related IdentityUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            // Populate a SelectList with IdentityUser data for editing
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", admin.Id);
            return View(admin);
        }

        // POST: Admins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom,IdentityUserId")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var identityUser = await _context.Users.FindAsync(admin.IdentityUser.Id);
                    if (identityUser != null)
                    {
                        admin.Id = identityUser.Id;  // Ensure Admin uses the correct IdentityUser Id
                        _context.Update(admin);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
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
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", admin.IdentityUser.Id);
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .Include(a => a.IdentityUser)  // Include related IdentityUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(string id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }
    }
}
