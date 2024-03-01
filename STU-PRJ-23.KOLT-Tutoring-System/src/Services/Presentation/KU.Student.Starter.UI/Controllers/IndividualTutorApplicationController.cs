using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KU.Student.Starter.UI.Controllers
{
    public class IndividualTutorApplicationController : Controller
    {
        private readonly TutorDataContext _context;

        public IndividualTutorApplicationController(TutorDataContext context)
        {
            _context = context;
        }

        public IActionResult IndividualTutorApplication()
        {
            string userEmail = GetUserEmail(User.Identity);
            var tutorApplication = _context.TutorApplications/*.Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(t => t.Periods).ThenInclude(pt => pt.Period)*/.FirstOrDefault(t => t.Email == userEmail);
            if (tutorApplication != null)
            {
                var viewModel = new TutorApplicationViewModel()
                {
                    Id = tutorApplication.Id,
                    Name = tutorApplication.Name,
                    Email = tutorApplication.Email,
                    WeeklyHour = tutorApplication.WeeklyHour,
                    Courses = tutorApplication.CourseIds != null ? string.Join(", ", _context.Courses.Where(c => tutorApplication.CourseIds.Contains(c.Id)).Select(c => c.Code)) : null,
                    GPA = tutorApplication.GPA,
                    Periods = tutorApplication.PeriodIds != null ? string.Join(", ", _context.Periods.Where(c => tutorApplication.PeriodIds.Contains(c.Id)).Select(a => string.Join(", ", a.Day, string.Join("-", a.StartHour, a.EndHour)))) : null,
                    Status = tutorApplication.Status
                };
                return View(viewModel);
            }
            else
            {
                return View(tutorApplication);
            }
        }

        [HttpGet]
        public IActionResult AddIndividualTutorApplication()
        {

            var viewModel = new TutorApplicationViewModel();
            viewModel.Email = GetUserEmail(User.Identity);
            viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
            viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult AddIndividualTutorApplication(TutorApplicationViewModel model)
        {
            TutorApplication tutorApplication = new TutorApplication();
            tutorApplication.Name = model.Name;
            tutorApplication.WeeklyHour = model.WeeklyHour;
            tutorApplication.Email = model.Email;
            tutorApplication.GPA = model.GPA;

            List<int> courseIds = new List<int>();
			List<int> periodIds = new List<int>();
			if (model.CourseIds != null && model.CourseIds.Length > 0)
            {
                foreach (var courseId in model.CourseIds)
                {
                    var course = _context.Courses.Where(x => x.Id == courseId).First();
                    if (course != null)
                    {
						courseIds.Add(courseId);
                    }
                }
            }

            if (model.PeriodIds != null && model.PeriodIds.Length > 0)
            {
                foreach (var periodId in model.PeriodIds)
                {
                    var period = _context.Periods.Where(x => x.Id == periodId).FirstOrDefault();
                    if (period != null)
                    {
                        periodIds.Add(periodId);
                    }
                }
            }
            tutorApplication.CourseIds = courseIds.ToArray();
			tutorApplication.PeriodIds = periodIds.ToArray();
            tutorApplication.Status = "Pending";

            try
            {

                ModelState["Name"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                ModelState["Courses"].Errors.Clear();
                ModelState["Periods"].Errors.Clear();
                ModelState["GPA"].Errors.Clear();
                ModelState["WeeklyHour"].Errors.Clear();

                if (model.CourseIds == null || model.CourseIds.Length == 0 || model.PeriodIds == null || model.PeriodIds.Length == 0 || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || model.GPA == 0 || model.WeeklyHour == 0)
                {
                    throw new Exception("error");
                }
                // Check if the email address is already taken by another tutor
                var existingTutor = _context.TutorApplications.FirstOrDefault(t => t.Email == model.Email && t.Id != model.Id);
                if (existingTutor != null)
                {
                    ModelState.AddModelError("Email", "A tutor application with this email already exists.");
                    model.Email = GetUserEmail(User.Identity);
                    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                    return View(model);
                }

                _context.TutorApplications.Add(tutorApplication);
                _context.SaveChanges();
                return RedirectToAction("IndividualTutorApplication");
            }
            catch (Exception e)
            {
                model.Email = GetUserEmail(User.Identity);
                model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();



                if (e.Message == "error")
                {

                    if (string.IsNullOrEmpty(model.Name))
                    {

                        ModelState.AddModelError("Name", "The Name field is required.");
                    }

                    if (string.IsNullOrEmpty(model.Email))
                    {
                        ModelState.AddModelError("Email", "The Email field is required.");
                    }
                    if (model.GPA == 0)
                    {
                        ModelState.AddModelError("GPA", "The GPA must be larger than 0.");
                    }

                    if (model.WeeklyHour == 0)
                    {
                        ModelState.AddModelError("WeeklyHour", "The Weekly Hour is required.");
                    }

                    if (model.CourseIds == null || model.CourseIds.Length == 0)
                    {
                        ModelState.AddModelError("Courses", "The Courses field is required.");
                    }

                    if (model.PeriodIds == null || model.PeriodIds.Length == 0)
                    {
                        ModelState.AddModelError("Periods", "The Periods field is required.");
                    }
                }
                
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult UpdateIndividualTutorApplication(int? id)
        {

            var viewModel = new TutorApplicationViewModel();

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var tutorApplication = _context.TutorApplications.FirstOrDefault(x => x.Id == id.Value);
                viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                viewModel.Id = tutorApplication.Id;
                viewModel.Name = tutorApplication.Name;
                viewModel.Email = tutorApplication.Email;
                viewModel.GPA = tutorApplication.GPA;
                viewModel.WeeklyHour = tutorApplication.WeeklyHour;
                viewModel.CourseIds = tutorApplication.CourseIds;
                viewModel.PeriodIds = tutorApplication.PeriodIds;
                viewModel.Courses = tutorApplication.CourseIds != null ? string.Join(", ", _context.Courses.Where(c => tutorApplication.CourseIds.Contains(c.Id)).Select(c => c.Code)) : null;
                viewModel.Periods = tutorApplication.PeriodIds != null ? string.Join(", ", _context.Periods.Where(c => tutorApplication.PeriodIds.Contains(c.Id)).Select(a => string.Join(", ", a.Day, string.Join("-", a.StartHour, a.EndHour)))) : null;

            }

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult UpdateIndividualTutorApplication(TutorApplicationViewModel model)
        {
            model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
            model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
            try
            {

                ModelState["Name"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                ModelState["Courses"].Errors.Clear();
                ModelState["Periods"].Errors.Clear();
                ModelState["GPA"].Errors.Clear();
                ModelState["WeeklyHour"].Errors.Clear();

                if (model.CourseIds == null || model.CourseIds.Length == 0 || model.PeriodIds == null || model.PeriodIds.Length == 0 || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || model.GPA == 0 || model.WeeklyHour == 0)
                {
                    throw new Exception("error");
                }
            /*if (string.IsNullOrEmpty((model.GPA).ToString()))
            {
                ModelState.AddModelError("GPA", "The GPA field is required.");
                return View(model);
            }*/

                // Check if the email address is already taken by another tutor
                var existingTutor = _context.TutorApplications.FirstOrDefault(t => t.Email == model.Email && t.Id != model.Id);
                if (existingTutor != null)
                {
                    ModelState.AddModelError("Email", "A tutor application with this email already exists.");
                    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                    return View(model);
                }

            /*if (model.Id != null)
            {
                var tutorApplication = _context.TutorApplications.FirstOrDefault(x => x.Id == model.Id);

                tutorApplication.Name = model.Name;
                tutorApplication.WeeklyHour = model.WeeklyHour;
                tutorApplication.Email = model.Email;
                tutorApplication.GPA = (model.GPA).ToString();
                List<int> courseIds = new List<int>();
                List<int> periodIds = new List<int>();
                if (model.CourseIds != null && model.CourseIds.Length > 0)*/


                if (model.Id != null)
                {
                    var tutorApplication = _context.TutorApplications.FirstOrDefault(x => x.Id == model.Id);

                    tutorApplication.Name = model.Name;
                    tutorApplication.WeeklyHour = model.WeeklyHour;
                    tutorApplication.Email = model.Email;
                    tutorApplication.GPA = model.GPA;
                    List<int> courseIds = new List<int>();
                    List<int> periodIds = new List<int>();
                    if (model.CourseIds != null && model.CourseIds.Length > 0)
                    {
                        foreach (var courseId in model.CourseIds)
                        {
                            var course = _context.Courses.Where(x => x.Id == courseId).First();
                            if (course != null)
                            {
                                courseIds.Add(courseId);
                            }
                        }
                    }

                    if (model.PeriodIds != null && model.PeriodIds.Length > 0)
                    {
                        foreach (var periodId in model.PeriodIds)
                        {
                            var period = _context.Periods.Where(x => x.Id == periodId).FirstOrDefault();
                            if (period != null)
                            {
                                periodIds.Add(periodId);
                            }
                        }
                    }
                    tutorApplication.CourseIds = courseIds.ToArray();
                    tutorApplication.PeriodIds = periodIds.ToArray();
                    _context.SaveChanges();
                }

                return RedirectToAction("IndividualTutorApplication");
            }
            //catch (DbUpdateException ex)
            //{
            //    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
            //    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

            //    if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
            //    {
            //        ModelState.AddModelError("Email", "A tutor with this email already exists.");

            //    }
            //    return View(model);
            //}

            catch (Exception e)
            {
                model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();



                if (e.Message == "error")
                {
                    if (string.IsNullOrEmpty(model.Name))
                    {

                        ModelState.AddModelError("Name", "The Name field is required.");
                    }

                    if (string.IsNullOrEmpty(model.Email))
                    {
                        ModelState.AddModelError("Email", "The Email field is required.");
                    }

                    if (model.WeeklyHour == 0)
                    {
                        ModelState.AddModelError("WeeklyHour", "The Weekly Hour field is required.");
                    }

                    if (model.GPA == 0)
                    {
                        ModelState.AddModelError("GPA", "The GPA must be larger than 0.");
                    }

                    if (model.CourseIds == null || model.CourseIds.Length == 0)
                    {
                        ModelState.AddModelError("Courses", "The Courses field is required.");
                    }

                    if (model.PeriodIds == null || model.PeriodIds.Length == 0)
                    {
                        ModelState.AddModelError("Periods", "The Periods field is required.");
                    }
                }

                return View(model);
            }
        }

        public IActionResult DeleteIndividualTutorApplication(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutorApplication = _context.TutorApplications
                .FirstOrDefault(m => m.Id == id);
            if (tutorApplication == null)
            {
                return NotFound();
            }
            else
            {
                var viewModel = new TutorApplicationViewModel();
                viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                viewModel.Id = tutorApplication.Id;
                viewModel.Name = tutorApplication.Name;
                viewModel.Email = tutorApplication.Email;
                viewModel.GPA = tutorApplication.GPA;
                viewModel.WeeklyHour = tutorApplication.WeeklyHour;
                viewModel.CourseIds = tutorApplication.CourseIds;
                viewModel.PeriodIds = tutorApplication.PeriodIds;
                viewModel.Courses = tutorApplication.CourseIds != null ? string.Join(", ", _context.Courses.Where(c => tutorApplication.CourseIds.Contains(c.Id)).Select(c => c.Code)) : null;
                viewModel.Periods = tutorApplication.PeriodIds != null ? string.Join(", ", _context.Periods.Where(c => tutorApplication.PeriodIds.Contains(c.Id)).Select(a => string.Join(", ", a.Day, string.Join("-", a.StartHour, a.EndHour)))) : null;
                return View(viewModel);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tutorApplication = _context.TutorApplications.Find(id);
            /*var pcs = _context.PeriodCubicles.ToList();
            foreach (var pc in pcs)
            {
                var pt = _context.TutorPeriods.Where(tp => tp.PeriodTutorId == pc.PeriodTutorId).ToList();
                if (pt.Count > 0 && pt.First().TutorId == id)
                {
                    _context.PeriodCubicles.Remove(pc);
                }
            }*/
            if (tutorApplication != null)
            {
                _context.TutorApplications.Remove(tutorApplication);

            }
            _context.SaveChanges();
            return RedirectToAction("IndividualTutorApplication");
        }

        public IActionResult GetIndividualTutorApplication(int? id)
        {
            var tutor = _context.TutorApplications.Find(id);
            return View("GetIndividualTutorApplication", tutor);
        }
        

        public IActionResult Appliable()
        {
            DateTime currentDateTime = DateTime.Now;
            if (_context.Configurations != null && _context.Configurations.Count() > 0)
            {
                var applicationDueDate = _context.Configurations.ToList().Last().Date;
                var applicationDueTime = _context.Configurations.ToList().Last().Time;

                int hour = int.Parse(applicationDueTime.Split(":")[0]);
                int minute = int.Parse(applicationDueTime.Split(":")[1]);
                int year = int.Parse(applicationDueDate.Split("-")[0]);
                int month = int.Parse(applicationDueDate.Split("-")[1]);
                int day = int.Parse(applicationDueDate.Split("-")[2]);

                DateTime dateTime = new DateTime(year, month, day, hour, minute, 59);
                if (currentDateTime <= dateTime)
                {
                    return Json(1);
                }
                else
                {
                    return Json(-1);
                }
            }
            else
            {
                return Json(0);
            }
        }

        public static string GetUserEmail(IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.FirstOrDefault((Claim f) => f.Type == "email")?.Value;
        }
    }
}

