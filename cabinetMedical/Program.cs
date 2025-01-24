using cabinetMedical.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using cabinetMedical.Data;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace cabinetMedical
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register SmtpEmailSender as IEmailSender
            builder.Services.AddTransient<IEmailSender, FakeEmailSender>();


            // Add DbContext for your application
            builder.Services.AddDbContext<MedicalContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("gestionMedicalContextConnection")));

            // Add Identity with your DbContext
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<MedicalContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Seed roles and admin user into the database
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var context = scope.ServiceProvider.GetRequiredService<MedicalContext>();

                // Seed roles
                RoleAndUserSeeder.SeedRolesAndAdminAsync(roleManager, userManager, context).Wait();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Add Authentication Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }

    public class RoleAndUserSeeder
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, MedicalContext context)
        {
            // Define the roles to seed
            var roles = new[] { "admin", "medecin", "infirmiere", "patient" };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@1234";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign the user to the "admin" role
                    await userManager.AddToRoleAsync(adminUser, "admin");

                    // Create the Admin instance and set its properties
                    var admin = new Admin
                    {
                        Id = adminUser.Id,
                        Nom = "admin",  // Set Nom to "admin"
                        IdentityUser = adminUser // Link to IdentityUser
                    };

                    // Add the admin to the Admin table
                    await context.Admins.AddAsync(admin);
                    await context.SaveChangesAsync();
                }
                else
                {
                    // Handle errors if any
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

}
