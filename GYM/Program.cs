//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;

//namespace GYM
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            CreateWebHostBuilder(args).Build().Run();
//        }

//        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//            WebHost.CreateDefaultBuilder(args)
//                .UseStartup<Startup>();
//    }
//}

using GYM;
using GYM.Data;
using GYM.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.WebSockets.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();

                // dotnet user-secrets set  "GYM:AdminPW" "FooBar77!"

                var AdminPw = config["GYM:AdminPW"];
                try
                {
                    SeedData.Initialize(services, AdminPw).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string adminPw)
        {
            using (var context = new ApplicationDbContext(
             serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (roleManager == null || userManager == null)
                {
                    throw new Exception("roleManager or userManager is null");
                }

                var roleNames = new[] { "Admin", "Member" };

                foreach (var name in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(name)) continue;
                    var role = new IdentityRole { Name = name };
                    var result = await roleManager.CreateAsync(role);
                    if(!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                var emails = new[] {  "admin@gym.se" };
                foreach (var email in emails)
                {
                    if (!await roleManager.RoleExistsAsync(email)) continue;
                    var user = new ApplicationUser { UserName = email, Email = email };
                    var result = await userManager.CreateAsync(user, adminPw);
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                var adminUser = await userManager.FindByNameAsync("admin@gym.se");
                var ok = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!ok.Succeeded)
                {
                    throw new Exception(string.Join("\n", ok.Errors));
                }

            }
        }
    }
}
