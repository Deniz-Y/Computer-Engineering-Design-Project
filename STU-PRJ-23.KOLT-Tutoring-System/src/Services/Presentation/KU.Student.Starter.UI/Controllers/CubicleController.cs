using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KU.Student.Starter.UI.Controllers
{
    public class CubicleController : Controller
    {
        private readonly TutorDataContext _context;
        private ExportHelper exportHelper = new ExportHelper();

        public CubicleController(TutorDataContext context)
        {
            _context = context;
        }

        // GET: /Cubicle/Cubicle
        public IActionResult Cubicle(string searchBy, string searchString)
        {
            var cubicle = this._context.Cubicles.OrderBy(c => c.CubicleName).ToList().Select(m => new CubicleViewModel
            {
                Id = m.Id,
                CubicleName = m.CubicleName,
                CubicleNumber = m.CubicleNumber,
                CubiclePlace = m.CubiclePlace

            });

            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("CubicleName"))
                {
                    cubicle = cubicle.Where(s => s.CubicleName!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("CubicleNumber"))
                {

                    cubicle = cubicle.Where(s => s.CubicleNumber!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("CubiclePlace"))
                {

                    cubicle = cubicle.Where(s => s.CubiclePlace!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }

            }

            return View(cubicle);
        }

        [HttpGet]
        public IActionResult AddCubicle()
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddCubicle(Cubicle m)
        {

            if (!ModelState.IsValid)
            {
                var viewModel = new CubicleViewModel();
                viewModel.CubicleName = m.CubicleName;
                viewModel.CubicleNumber = m.CubicleNumber;
                viewModel.CubiclePlace = m.CubiclePlace;
                return View(viewModel);
            }


            try
            {
                _context.Cubicles.Add(m);
                _context.SaveChanges();
                return RedirectToAction("Cubicle");
            }
            catch (DbUpdateException ex)
            {
                    throw; 
                
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

        public IActionResult DeleteCubicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cubicle = _context.Cubicles
                .FirstOrDefault(m => m.Id == id);
            if (cubicle == null)
            {
                return NotFound();
            }

            return View(cubicle);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {

            var cubicle = _context.Cubicles.Find(id);
            if (cubicle != null)
            {
                var pcs=_context.PeriodCubicles.Where(pc => pc.CubicleId == cubicle.Id).ToList();
                if (pcs != null && pcs.Count > 0)
                {
                    _context.PeriodCubicles.RemoveRange(pcs);
                }
                _context.Cubicles.Remove(cubicle);
                _context.SaveChanges();
            }

            return RedirectToAction("Cubicle");
        }

        public IActionResult GetCubicle(int? id)
        {
           
            var cubicle = _context.Cubicles.Find(id);
            return View("GetCubicle", cubicle);
        }

        public IActionResult EditCubicle(Cubicle c)
        {

            var cubicle = _context.Cubicles.Find(c.Id);
            cubicle.CubicleName = c.CubicleName;
            cubicle.CubicleNumber = c.CubicleNumber;
            cubicle.CubiclePlace = c.CubiclePlace;

            if (!ModelState.IsValid)
            {
                return View("GetCubicle", cubicle);
            }


            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw;

            }
            
            return RedirectToAction("Cubicle");
        }

        public IActionResult Export()
        {

            List<CubicleViewModel> cubicles = this._context.Cubicles.Select(m => new CubicleViewModel
            {
                Id = m.Id,
                CubicleName = m.CubicleName,
                CubicleNumber = m.CubicleNumber,
                CubiclePlace = m.CubiclePlace

            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: cubicles);
            return File(stream.ToArray(), "text/plain", "Cubicles.csv");
        }
    }
}

