using GYM.Data;
using GYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace GYM.Controllers
{
    [Authorize(Roles ="Admin")]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(await _context.GymClass.ToListAsync());
            }

            var user = await GetCurrentUserAsync();
            var model = await _context.GymClass.Include(a => a.AttendingMembers)
                .ThenInclude(u => u.ApplicationUser)
                .ToListAsync();

            ViewData["Booked"] = model.SelectMany(m => m.AttendingMembers)
                    .Where(u => u.ApplicationUserId == user.Id)
                    .Select(d => new {GymId = d.GymClassId, UserId = d.ApplicationUserId })
                    .ToDictionary(a => a.GymId, a => a.UserId);

            return View(model);
        }

        [Authorize(Roles ="Member")]
        public async Task<IActionResult> BookingToogle(int? id)
        {
            if (id == null) return NotFound();

            ApplicationUser currentUser = await _context.Users.Where
                (u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();

            GymClass currentClass = await _context.GymClass.Where(c => c.Id == id)
                .Include(a => a.AttendingMembers).FirstOrDefaultAsync();

            if (currentClass == null)
            {
                return NotFound();
            }

            ApplicationUserGymClass attending = currentClass.AttendingMembers
                .Where(u => u.ApplicationUserId == currentUser.Id &&
                 u.GymClassId == id).FirstOrDefault();

            if (attending == null)
            {
                var book = new ApplicationUserGymClass()
                {
                    ApplicationUserId = currentUser.Id,
                    GymClassId = (int)id
                };

                _context.UserGym.Add(book);
                _context.SaveChanges();
            }
            else
            {
                _context.UserGym.Remove(attending);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Durartion,De")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Durartion,De")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClass.FindAsync(id);
            _context.GymClass.Remove(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClass.Any(e => e.Id == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);
    }
}
