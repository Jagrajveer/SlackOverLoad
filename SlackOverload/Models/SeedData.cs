using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SlackOverload.Data;

namespace SlackOverload.Models {

    public static class SeedData {
        public async static Task Initialize(IServiceProvider serviceProvider) {
            var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<string> roles = new List<string>
            {
                "Owner", "admin", "Manager", "User"
            };

            if (!context.Roles.Any()) {
                foreach (string role in roles) {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                context.SaveChanges();
            }

            

            if (!context.Users.Any()) {
                ApplicationUser seededUser = new ApplicationUser {
                    Email = "Admin@test.ca",
                    NormalizedEmail = "ADMIN@TEST.CA",
                    UserName = "Admin@test.ca",
                    NormalizedUserName = "ADMIN@TEST.CA",
                    EmailConfirmed = true,
                };

                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(seededUser, "P@ssword1");
                seededUser.PasswordHash = hashed;

                await userManager.CreateAsync(seededUser);
                await userManager.AddToRoleAsync(seededUser, "admin");

            }

            await context.SaveChangesAsync();
        }
    }
}