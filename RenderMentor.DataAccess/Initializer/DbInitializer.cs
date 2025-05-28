using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RenderMentor.DataAccess.Data;
using RenderMentor.Models;
using RenderMentor.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderMentor.DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception)
            {

            }

            if (_db.Roles.Any(r => r.Name == SD.Role_Admin)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Instructor)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Individual)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "evrencavusoglu@gmail.com",
                Email = "evrencavusoglu@gmail.com",
                EmailConfirmed = true,
                Name = "Evren Çavuşoğlu",
                Gender = ApplicationUser.Genders.Male,
                PhoneNumber = "0537 529 27 37"
            }, "eAcd3i*9-23**").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUsers.Where(u => u.Email == "evrencavusoglu@gmail.com").FirstOrDefault();

            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, SD.Role_Instructor).GetAwaiter().GetResult();
            _db.Instructors.Add(new Instructor()
            {
                UserId = user.Id,
                ListOrder = 1,
                IsAdmin = true
            });
            _db.CourseLists.Add(new CourseList() { 
                Title = "Online Kurslar",
                SEOUrl = "online-kurslar"
            });
            _db.HomeContent.Add(new HomeContent() { 
                MentorName = "Evren Çavuşoğlu"
            });
            _db.Contact.Add(new Contact());
            _db.About.Add(new About());
            _db.SaveChanges();
        }
    }
}
