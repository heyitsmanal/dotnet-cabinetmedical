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
    [Authorize(Roles = "admin,infirmiere")]
    public class InfirmieresController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InfirmieresController(MedicalContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Infirmieres
        public async Task<IActionResult> Index(string search)
        {
            var infirmieres = from i in _context.Infirmieres
                              .Include(i => i.IdentityUser)
                              select i;

            if (!string.IsNullOrEmpty(search))
            {
                infirmieres = infirmieres.Where(i => i.Nom.Contains(search)); // Filter by 'Nom'
            }

            // Pass the isAdmin value to the view
            var isAdmin = User.IsInRole("admin");

            ViewData["IsAdmin"] = isAdmin;

            return View(await infirmieres.ToListAsync());
        }




        // GET: Infirmieres/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infirmiere = await _context.Infirmieres
                .Include(i => i.IdentityUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (infirmiere == null)
            {
                return NotFound();
            }

            return View(infirmiere);
        }

        // GET: Infirmieres/Create
        public IActionResult Create()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Infirmieres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom")] Infirmiere infirmiere, string email, string password)
        {
            ModelState.Remove("Id");
            ModelState.Remove("IdentityUser");

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError(string.Empty, "Email and Password are required.");
                    return View(infirmiere);
                }

                if (!await _roleManager.RoleExistsAsync("Infirmiere"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Infirmiere"));
                }

                var user = new IdentityUser { UserName = email, Email = email };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Infirmiere");
                    if (roleResult.Succeeded)
                    {
                        infirmiere.Id = user.Id;
                        infirmiere.IdentityUser = user;
                        _context.Add(infirmiere);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(infirmiere);
        }

        // GET: Infirmieres/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infirmiere = await _context.Infirmieres
                .Include(i => i.IdentityUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (infirmiere == null)
            {
                return NotFound();
            }

            return View(infirmiere);
        }

        // POST: Infirmieres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom")] Infirmiere infirmiere)
        {
            ModelState.Remove("IdentityUser");

            if (id != infirmiere.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingInfirmiere = await _context.Infirmieres
                        .Include(i => i.IdentityUser)
                        .FirstOrDefaultAsync(i => i.Id == id);

                    if (existingInfirmiere == null)
                    {
                        return NotFound();
                    }

                    existingInfirmiere.Nom = infirmiere.Nom;

                    _context.Update(existingInfirmiere);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InfirmiereExists(infirmiere.Id))
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
            return View(infirmiere);
        }

        // GET: Infirmieres/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infirmiere = await _context.Infirmieres
                .Include(i => i.IdentityUser)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (infirmiere == null)
            {
                return NotFound();
            }

            return View(infirmiere);
        }

        // POST: Infirmieres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var infirmiere = await _context.Infirmieres.FindAsync(id);
            if (infirmiere != null)
            {
                _context.Infirmieres.Remove(infirmiere);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InfirmiereExists(string id)
        {
            return _context.Infirmieres.Any(e => e.Id == id);
        }




        public IActionResult Dashboard()
        {
            // Get counts for Consultations, Notifications, Factures, and Plannings
            ViewBag.ConsultationsCount = _context.Consultations.Count();
            ViewBag.NotificationsCount = _context.Notifications.Count();
            ViewBag.FacturesCount = _context.Factures.Count();

            return View();
        }




    }
}
