using EnterpriseTicketing.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Infrastructure.Persistence.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await SeedRoles(roleManager);
            await SeedUsers(userManager);
            await SeedSLAs(dbContext);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { "Admin", "Agent", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }

        private static async Task SeedUsers(UserManager<User> userManager)
        {
            // Admin
            if (await userManager.FindByEmailAsync("admin@test.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                    FirstName = "مدیر",
                    LastName = "سیستم",
                    Department = "مدیریت",
                    PhoneNumber = "09123456789",
                    EmailConfirmed = true,
                    PreferredLanguage = "fa",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Agent
            if (await userManager.FindByEmailAsync("agent@test.com") == null)
            {
                var agent = new User
                {
                    UserName = "agent@test.com",
                    Email = "agent@test.com",
                    FirstName = "کارشناس",
                    LastName = "پشتیبانی",
                    Department = "پشتیبانی",
                    PhoneNumber = "09123456788",
                    EmailConfirmed = true,
                    PreferredLanguage = "fa",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(agent, "Agent@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(agent, "Agent");
                }
            }

            // Normal User
            if (await userManager.FindByEmailAsync("user@test.com") == null)
            {
                var user = new User
                {
                    UserName = "user@test.com",
                    Email = "user@test.com",
                    FirstName = "کاربر",
                    LastName = "عادی",
                    Department = "فروش",
                    PhoneNumber = "09123456787",
                    EmailConfirmed = true,
                    PreferredLanguage = "fa",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }

        private static async Task SeedSLAs(ApplicationDbContext context)
        {
            if (!await context.SLAs.AnyAsync())
            {
                var slas = new List<SLA>
                {
                    new SLA
                    {
                        Id = Guid.NewGuid(),
                        Name = "فوری",
                        Description = "رسیدگی فوری به مشکلات حیاتی",
                        Priority = (TicketPriority)1,
                        ResponseTimeHours = 1,
                        ResolutionTimeHours = 4,
                        Department = "پشتیبانی",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SLA
                    {
                        Id = Guid.NewGuid(),
                        Name = "بالا",
                        Description = "رسیدگی به مشکلات مهم",
                        Priority = (TicketPriority)2,
                        ResponseTimeHours = 4,
                        ResolutionTimeHours = 12,
                        Department = "پشتیبانی",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SLA
                    {
                        Id = Guid.NewGuid(),
                        Name = "متوسط",
                        Description = "رسیدگی به مشکلات معمولی",
                        Priority = (TicketPriority)3,
                        ResponseTimeHours = 8,
                        ResolutionTimeHours = 24,
                        Department = "پشتیبانی",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SLA
                    {
                        Id = Guid.NewGuid(),
                        Name = "پایین",
                        Description = "رسیدگی به مشکلات غیر ضروری",
                        Priority = (TicketPriority)4,
                        ResponseTimeHours = 24,
                        ResolutionTimeHours = 72,
                        Department = "پشتیبانی",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.SLAs.AddRangeAsync(slas);
            }
        }
    }
}