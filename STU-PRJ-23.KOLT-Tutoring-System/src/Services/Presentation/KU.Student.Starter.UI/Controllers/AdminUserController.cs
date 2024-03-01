using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KU.Student.Starter.UI.Controllers
{
    public class AdminUserController : Controller
    {
        private readonly TutorDataContext _context;
        private ExportHelper exportHelper = new ExportHelper();

        public AdminUserController(TutorDataContext context)
        {
            _context = context;
        }

        // GET: /AdminUser/AdminUser
        public IActionResult AdminUser(string searchBy, string searchString)
        {
            var adminUser = this._context.AdminUsers.OrderBy(c => c.FirstName).ToList().Select(m => new AdminUserViewModel
            {
                Id = m.Id,
                Username = m.Username,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName

            });

            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Username"))
                {
                    adminUser = adminUser.Where(s => s.Username!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("Email"))
                {

                    adminUser = adminUser.Where(s => s.Email!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("FirstName"))
                {

                    adminUser = adminUser.Where(s => s.FirstName!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("LastName"))
                {

                    adminUser = adminUser.Where(s => s.LastName!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }

            }

            return View(adminUser);
        }

        [HttpGet]
        public IActionResult AddAdminUser()
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddAdminUser(AdminUser m)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new AdminUserViewModel();
                viewModel.Email = m.Email;
                viewModel.FirstName = m.FirstName;
                viewModel.LastName = m.LastName;
                viewModel.Username = m.Username;
                return View(viewModel);
            }

            try
            {

                _context.AdminUsers.Add(m);
                _context.SaveChanges();
                return RedirectToAction("AdminUser");
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "An admin user with this email already exists.");
                    var viewModel = new AdminUserViewModel();
                    viewModel.Email = m.Email;
                    viewModel.FirstName = m.FirstName;
                    viewModel.LastName = m.LastName;
                    viewModel.Username = m.Username;
                    return View(viewModel);
                }
                else
                {
                    throw; // Rethrow the exception if it wasn't caused by a unique constraint violation
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteSelected(int[] ids)
        {
            foreach (int id in ids)
            {
                DeleteConfirmed(id);
            }
            return Ok();
        }

        public IActionResult DeleteAdminUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = _context.AdminUsers
                .FirstOrDefault(m => m.Id == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {

            var adminUser = _context.AdminUsers.Find(id);
            if (adminUser != null)
            {
                _context.AdminUsers.Remove(adminUser);
                _context.SaveChanges();
            }

            return RedirectToAction("AdminUser");
        }

        public IActionResult GetAdminUser(int? id)
        {

            var adminUser = _context.AdminUsers.Find(id);
            return View("GetAdminUser", adminUser);
        }

        public IActionResult EditAdminUser(AdminUser c)
        {

            var adminUser = _context.AdminUsers.Find(c.Id);
            adminUser.Username = c.Username;
            adminUser.Email = c.Email;
            adminUser.FirstName = c.FirstName;
            adminUser.LastName = c.LastName;


            if (!ModelState.IsValid)
            {
                return View("GetAdminUser", adminUser);
            }


            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "An admin user with this email already exists.");
                    return View("GetAdminUser", adminUser);
                }
                else
                {
                    throw; // Rethrow the exception if it wasn't caused by a unique constraint violation
                }           
            }

            return RedirectToAction("AdminUser");
        }

        public IActionResult Export()
        {

            List<AdminUserViewModel> adminUsers = this._context.AdminUsers.Select(m => new AdminUserViewModel
            {
                Id = m.Id,
                Username = m.Username,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName

            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: adminUsers);
            return File(stream.ToArray(), "text/plain", "AdminUsers.csv");
        }
    }
}

