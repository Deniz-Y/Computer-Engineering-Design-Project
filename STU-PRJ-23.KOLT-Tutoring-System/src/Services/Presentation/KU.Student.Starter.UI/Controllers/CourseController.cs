using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StackExchange.Redis;

namespace KU.Student.Starter.UI.Controllers
{
    public class CourseController : Controller
    {
        private readonly Models.TutorDataContext context;
        private ExportHelper exportHelper = new ExportHelper();


        public CourseController(Models.TutorDataContext context)
        {
            this.context = context;

        }

        public IActionResult Course(string searchBy, string searchString)
        {
            var courses = this.context.Courses.OrderBy(c => c.Code).ToList().Select(m => new CourseViewModel
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name

            });

            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Code"))
                {
                    // courses = courses.Where(s => s.Code!.Contains(searchString)).ToList();
                    courses = courses.Where(s => s.Code!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("Name"))
                {

                    // courses = courses.Where(s => s.Name!.Contains(searchString)).ToList();
                    courses = courses.Where(s => s.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }

            }

            return View(courses);

        }

        /*public IActionResult OrderBy(string filter, int direction)
        {
            var courses = this.context.Courses.ToList().Select(m => new CourseViewModel
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name

            });

            if (filter.Equals("Code"))
            {
                courses = courses.OrderBy(c => c.Code).ToList();
            }
            else if (filter.Equals("Name"))
            {
                courses = courses.OrderBy(c => c.Code).ToList();
            }

            return View(courses);
        }*/
        
        [HttpGet]
        public IActionResult AddCourse()
        {

            return View();
        }

        [HttpPost]
        public IActionResult AddCourse(Course m)
        {
            /*context.Courses.Add(m);
            context.SaveChanges();
            return RedirectToAction("Course");*/
           if (!ModelState.IsValid)
                {
                var viewModel = new CourseViewModel();
                viewModel.Code = m.Code;
                viewModel.Name = m.Name;
                return View(viewModel);
                }
            

            try
            {
                context.Courses.Add(m);
                context.SaveChanges();
                return RedirectToAction("Course");
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Code", "A course with this code already exists.");
                    var viewModel = new CourseViewModel();
                    viewModel.Code = m.Code;
                    viewModel.Name = m.Name;
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

        public IActionResult DeleteCourse(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = context.Courses
                .FirstOrDefault(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {

            var course = context.Courses.Find(id);
            var headTutors = context.HeadTutors.ToList();
            var headTutorCon = context.HeadTutorConnections.ToList();
            List<HeadTutorConnection> deletehtc = new List<HeadTutorConnection>();
            List<HeadTutor> deleteht = new List<HeadTutor>();
            if (course != null)
            {
                foreach(var ht in headTutors)
                {
                    if (ht.CourseId == id)
                    {
                        deleteht.Add(ht);
                        headTutorCon.Where(htc => htc.HeadTutorId == ht.Id).ToList().ForEach(p => { deletehtc.Add(p); });
                    }
                }
                if (deleteht != null)
                {
                    context.HeadTutors.RemoveRange(deleteht);
                }
                if (deletehtc != null)
                {
                    context.HeadTutorConnections.RemoveRange(deletehtc);
                }
                context.Courses.Remove(course);
                context.SaveChanges();
            }

            return RedirectToAction("Course");
        }

        public IActionResult GetCourse(int? id)
        {

            var course = context.Courses.Find(id);
            return View("GetCourse", course);
        }

        public IActionResult EditCourse(Course c)
        {

            var course = context.Courses.Find(c.Id);
            course.Code = c.Code;
            course.Name = c.Name;


            if (!ModelState.IsValid)
            {
                return View("GetCourse", course);
            }


            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Code", "A course with this code already exists.");
                    return View("GetCourse", course);
                }
                else
                {
                    throw; // Rethrow the exception if it wasn't caused by a unique constraint violation
                }

            }

            return RedirectToAction("Course");
        }

        public IActionResult GetTerm(string term)
        {
            CourseDataSource.GetCourses(term: term);
            List<TempCourse> courses = CourseDataSource.GetCourseList();
            List<TempCourse> coursesDistinct = courses.DistinctBy(i => i.Subject + i.CatalogNumber).ToList();

            var codes = context.Courses.ToList().Select(f => f.Code).ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }


            try
            {
                foreach (TempCourse course in coursesDistinct)
                {
                    if (codes.Contains(course.Subject + course.CatalogNumber))
                    {
                        continue;
                    }
                    else
                    {
                        context.Courses.Add(new Course { Name = course.CourseTitle, Code = course.Subject + course.CatalogNumber });
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("Course");
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Code", "A course with this code already exists.");
                    return View();
                }
                else
                {
                    throw; // Rethrow the exception if it wasn't caused by a unique constraint violation
                }
            }
        }

        public IActionResult Import()
        {
            return View("GetTerm");
        }

        public IActionResult Export()
        {

            List<CourseViewModel> courses = this.context.Courses.Select(m => new CourseViewModel
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Tutors = String.Join(" ", m.Tutors.Select(x=> x.Tutor.Name)),

            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: courses);
            return File(stream.ToArray(), "text/plain", "Courses.csv");
        }
    }
}
