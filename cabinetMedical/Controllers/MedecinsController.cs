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
    [Authorize(Roles = "admin,medecin")]
    public class MedecinsController : Controller
    {
        private readonly MedicalContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MedecinsController(MedicalContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Medecins
        public async Task<IActionResult> Index(string search)
        {
            var medecins = from m in _context.Medecins
                           .Include(m => m.IdentityUser) // Include the related IdentityUser data
                           select m;

            if (!string.IsNullOrEmpty(search))
            {
                medecins = medecins.Where(m => m.Nom.Contains(search));
            }

            return View(await medecins.ToListAsync());
        }


        // GET: Medecins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medecin = await _context.Medecins
                .Include(m => m.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medecin == null)
            {
                return NotFound();
            }

            return View(medecin);
        }


        // GET: Medecins/Create
        public IActionResult Create()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom,Spécialité")] Medecin medecin, string email, string password)
        {
            Console.WriteLine("Create method called.");

            // Remove fields from ModelState that shouldn't be validated
            ModelState.Remove("Id");
            ModelState.Remove("IdentityUser");

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid.");

                // Validate email and password
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Email or Password is empty.");
                    ModelState.AddModelError(string.Empty, "Email and Password are required.");
                    return View(medecin);
                }

                // Ensure the "Medecin" role exists
                if (!await _roleManager.RoleExistsAsync("Medecin"))
                {
                    Console.WriteLine("Role 'Medecin' does not exist. Creating role.");
                    await _roleManager.CreateAsync(new IdentityRole("Medecin"));
                }

                // Create a new IdentityUser
                var user = new IdentityUser { UserName = email, Email = email };
                Console.WriteLine($"Attempting to create user: {email}");
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    Console.WriteLine("User created successfully.");

                    // Assign the Medecin role to the user
                    var roleResult = await _userManager.AddToRoleAsync(user, "Medecin");
                    if (roleResult.Succeeded)
                    {
                        Console.WriteLine("Role 'Medecin' assigned successfully.");

                        // Populate the Medecin table
                        medecin.Id = user.Id; // Assign the user's Id to Medecin's Id
                        medecin.IdentityUser = user; // Explicitly set the IdentityUser relationship
                        _context.Add(medecin);
                        await _context.SaveChangesAsync();

                        Console.WriteLine("Medecin added to database successfully.");
                        return RedirectToAction(nameof(Index));
                    }

                    Console.WriteLine("Failed to assign role 'Medecin'.");
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Role assignment error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to create user.");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"User creation error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($" - Error: {subError.ErrorMessage}");
                    }
                }
            }

            Console.WriteLine("Returning to Create view with ModelState errors.");
            return View(medecin);
        }






        public async Task<IActionResult> Edit(string id)
        {
            Console.WriteLine($"Fetching Medecin with ID: {id}");

            if (id == null)
            {
                Console.WriteLine("ID is null.");
                return NotFound();
            }

            var medecin = await _context.Medecins
                .Include(m => m.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medecin == null)
            {
                Console.WriteLine($"Medecin with ID {id} not found.");
                return NotFound();
            }

            Console.WriteLine($"Fetched Medecin: {medecin.Nom}, {medecin.Spécialité}");
            ViewData["Title"] = "Edit";
            return View(medecin);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nom,Spécialité")] Medecin medecin)
        {
            Console.WriteLine($"Edit POST called for ID: {id}");
            Console.WriteLine($"Incoming Data: Id={medecin.Id}, Nom={medecin.Nom}, Spécialité={medecin.Spécialité}");

            // Exclude IdentityUser from validation
            ModelState.Remove("IdentityUser");

            if (id != medecin.Id)
            {
                Console.WriteLine("ID mismatch.");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing Medecin from the database
                    var existingMedecin = await _context.Medecins
                        .Include(m => m.IdentityUser)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingMedecin == null)
                    {
                        Console.WriteLine("Medecin not found in database.");
                        return NotFound();
                    }

                    // Update only the Medecin-specific fields
                    existingMedecin.Nom = medecin.Nom;
                    existingMedecin.Spécialité = medecin.Spécialité;

                    _context.Update(existingMedecin);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Medecin updated successfully.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedecinExists(medecin.Id))
                    {
                        Console.WriteLine("Concurrency issue: Medecin no longer exists.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("ModelState is invalid. Errors:");
            foreach (var error in ModelState)
            {
                Console.WriteLine($"Key: {error.Key}");
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($" - Error: {subError.ErrorMessage}");
                }
            }

            return View(medecin);
        }



















        // GET: Medecins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medecin = await _context.Medecins
                .Include(m => m.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medecin == null)
            {
                return NotFound();
            }

            return View(medecin);
        }

        // POST: Medecins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medecin = await _context.Medecins.FindAsync(id);
            if (medecin != null)
            {
                _context.Medecins.Remove(medecin);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MedecinExists(string id)
        {
            return _context.Medecins.Any(e => e.Id == id);
        }





        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Medecin Dashboard";

            // Retrieve counts for consultations, dossier medicals, and medicaments
            ViewBag.ConsultationsCount = _context.Consultations.Count();
            ViewBag.DossierMedicalsCount = _context.DossiersMedicals.Count();
            ViewBag.MedicamentsCount = _context.Medicaments.Count();

            // Retrieve plannings from the database
            var plannings = _context.Plannings.ToList();

            // Pass plannings to the view
            ViewBag.Plannings = plannings;

            return View();
        }





    }
}
