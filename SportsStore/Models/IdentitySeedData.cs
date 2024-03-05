using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace SportsStore.Models
{
    public static class IdentitySeedData

    {
        //user credentials
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";

        //seed initial data
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            // Get an instance of the Identity DbContext.
            AppIdentityDbContext context = app.ApplicationServices
            .CreateScope().ServiceProvider
            .GetRequiredService<AppIdentityDbContext>();

            // Check for pending migrations and apply them if any.
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            // Get an instance of the UserManager for managing Identity users.
            UserManager<IdentityUser> userManager = app.ApplicationServices
            .CreateScope().ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

            // Check if the admin user already exists.
            IdentityUser user = await userManager.FindByNameAsync(adminUser);
            
            // If the admin user doesn't exist, create it.
            if (user == null)
            {
                user = new IdentityUser("Admin");
                user.Email = "admin@example.com";
                user.PhoneNumber = "555-1234";

                // Create the admin user with the specified password.
                await userManager.CreateAsync(user, adminPassword);
            }
        }
    }
}