#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using cabinetMedical.Models;

namespace cabinetMedical.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly MedicalContext _context;  // Inject ApplicationDbContext

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            MedicalContext context)  // Add the context to the constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;  // Assign the injected context to the private variable
        }

        public string Username { get; set; }
        public string Email { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Nom { get; set; }
        public string Adresse { get; set; }

        // Other properties for Patient
        public string PatientNom { get; set; }
        public string PatientAdresse { get; set; }

        public class InputModel
        {

            [Display(Name = "Nom")]
            public string Nom { get; set; } // Add this property


            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Adresse")]
            public string Adresse { get; set; }


        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = user.Email;

            Username = userName;

            // Debugging: Log values to check if they are being retrieved correctly
            Console.WriteLine($"LoadAsync - Username: {Username}, PhoneNumber: {phoneNumber}, Email: {email}");

            // Ensure the Input model is populated with phone number and email
            Input = new InputModel
            {
                PhoneNumber = phoneNumber ?? "No phone number set",  // Ensure PhoneNumber is populated here
                Email = email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Initialize Input to avoid NullReferenceException
            Input = new InputModel();

            // Get user-related data from the relevant table (Admin, Medecin, Patient, Infirmiere)
            var admin = await _context.Admins
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(a => a.IdentityUser.Id == user.Id);
            var medic = await _context.Medecins
                .Include(m => m.IdentityUser)
                .FirstOrDefaultAsync(m => m.IdentityUser.Id == user.Id);
            var patient = await _context.Patients
                .Include(p => p.IdentityUser)
                .FirstOrDefaultAsync(p => p.IdentityUser.Id == user.Id);
            var infirmiere = await _context.Infirmieres
                .Include(i => i.IdentityUser)
                .FirstOrDefaultAsync(i => i.IdentityUser.Id == user.Id);

            // Set the Nom and Adresse in the InputModel
            if (admin != null)
            {
                Input.Nom = admin.Nom;
            }
            else if (medic != null)
            {
                Input.Nom = medic.Nom;
            }
            else if (patient != null)
            {
                Input.Nom = patient.Nom;
                Input.Adresse = patient.Adresse;  // Set Adresse for Patient
            }
            else if (infirmiere != null)
            {
                Input.Nom = infirmiere.Nom;
            }

            // Ensure InputModel is populated correctly
            Input.PhoneNumber = user.PhoneNumber ?? "No phone number set";
            Input.Email = user.Email;

            return Page();
        }





        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Update Nom if the value has changed
            if (!string.IsNullOrEmpty(Input.Nom) && Input.Nom != Nom)
            {
                var admin = await _context.Admins
                    .Include(a => a.IdentityUser)
                    .FirstOrDefaultAsync(a => a.IdentityUser.Id == user.Id);

                if (admin != null)
                {
                    admin.Nom = Input.Nom;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var medic = await _context.Medecins
                        .Include(m => m.IdentityUser)
                        .FirstOrDefaultAsync(m => m.IdentityUser.Id == user.Id);
                    if (medic != null)
                    {
                        medic.Nom = Input.Nom;
                        await _context.SaveChangesAsync();
                    }

                    var patient = await _context.Patients
                        .Include(p => p.IdentityUser)
                        .FirstOrDefaultAsync(p => p.IdentityUser.Id == user.Id);
                    if (patient != null)
                    {
                        patient.Nom = Input.Nom;
                        await _context.SaveChangesAsync();
                    }

                    var infirmiere = await _context.Infirmieres
                        .Include(i => i.IdentityUser)
                        .FirstOrDefaultAsync(i => i.IdentityUser.Id == user.Id);
                    if (infirmiere != null)
                    {
                        infirmiere.Nom = Input.Nom;
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // Update Adresse for Patient only
            if (!string.IsNullOrEmpty(Input.Adresse))
            {
                var patient = await _context.Patients
                    .Include(p => p.IdentityUser)
                    .FirstOrDefaultAsync(p => p.IdentityUser.Id == user.Id);
                if (patient != null)
                {
                    patient.Adresse = Input.Adresse; // Update Adresse for Patient
                    await _context.SaveChangesAsync();
                }
            }

            // Update Email if modified
            var email = user.Email;
            if (Input.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set email.";
                    return RedirectToPage();
                }
            }

            // Update phone number if changed
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Refresh sign-in to apply changes
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }





    }
}
