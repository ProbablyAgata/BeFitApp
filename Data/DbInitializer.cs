using BeFit.Models;
using Microsoft.AspNetCore.Identity;

namespace BeFit.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Dodaj rolę admina
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // Dodaj konto admina
            var admin = await userManager.FindByEmailAsync("admin@befit.pl");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@befit.pl",
                    Email = "admin@befit.pl",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Test123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Dodaj przykładowe ćwiczenia
            if (!context.ExerciseTypes.Any())
            {
                context.ExerciseTypes.AddRange(
                    new ExerciseType { Name = "Przysiady" },
                    new ExerciseType { Name = "Martwy ciąg" },
                    new ExerciseType { Name = "Wyciskanie na ławce" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
