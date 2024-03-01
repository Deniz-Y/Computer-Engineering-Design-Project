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
    public class TutorApplicationController : Controller
    {
        private readonly TutorDataContext _context;
        private ExportHelper exportHelper = new ExportHelper();

        public TutorApplicationController(TutorDataContext context)
        {
            _context = context;
        }

        public IActionResult TutorApplication(string searchBy, string searchString)
        {
            var tutorApplications = this._context.TutorApplications.OrderBy(c => c.Name).ThenBy(c => c.GPA).ToList().Select(m => new TutorApplicationViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                WeeklyHour = m.WeeklyHour,
                Courses = m.CourseIds!=null ? string.Join(", ", _context.Courses.Where(c=>m.CourseIds.Contains(c.Id)).Select(c=>c.Code)):null,
                GPA = m.GPA,
                Periods = m.PeriodIds != null ? string.Join(", ", _context.Periods.Where(c => m.PeriodIds.Contains(c.Id)).Select(a => string.Join(", ", a.Day, string.Join("-", a.StartHour, a.EndHour)))):null,
                Status=m.Status
			});



            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Name"))
                {
                    tutorApplications = tutorApplications.Where(s => s.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("Email"))
                {

                    tutorApplications = tutorApplications.Where(s => s.Email!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("Courses"))
                {

                    tutorApplications = tutorApplications.Where(s => s.Courses!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("GPA"))
                {

                    tutorApplications = tutorApplications.Where(s => (s.GPA).ToString()!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("Periods"))
                {

                    tutorApplications = tutorApplications.Where(s => s.Periods!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("WeeklyHour"))
                {

                    tutorApplications = tutorApplications.Where(s => s.WeeklyHour.ToString()!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }

            }
            return View(tutorApplications);

        }

        [HttpGet]
        public IActionResult AddTutorApplication()
        {

            var viewModel = new TutorApplicationViewModel();
            viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
            viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult AddTutorApplication(TutorApplicationViewModel model)
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
                    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                    return View(model);
                }

                _context.TutorApplications.Add(tutorApplication);
                _context.SaveChanges();
                return RedirectToAction("TutorApplication");
            }
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
        public IActionResult UpdateTutorApplication(int? id)
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
        public IActionResult UpdateTutorApplication(TutorApplicationViewModel model)
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


                ////////////////SORRRRRRRR///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // If user is an admin, redirect to the Tutor page
                /*if (User.IsInRole("admin"))
                {
                    return RedirectToAction("TutorApplication");
                }

                // If user is a tutor, redirect to the tutor's own page
                if (User.IsInRole("tutor"))
                {
                    string userEmail = GetUserEmail(User.Identity);
                    if (tutorApplication.Email == userEmail)
                    {
                        return RedirectToAction("IndividualTutorPage");
                    }
                    else
                    {
                        // handle error: user is not authorized to view this tutor's page
                    }
                }*/

                return RedirectToAction("TutorApplication");
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

        [HttpPost]
        public IActionResult DeleteSelected(int[] ids)
        {
            foreach (int id in ids)
            {
                DeleteConfirmed(id);
            }
            return Ok();
        }

        // GET: Tutor/DeleteTutor
        public IActionResult DeleteTutorApplication(int? id)
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
            return RedirectToAction("TutorApplication");
        }

        public IActionResult GetTutorApplication(int? id)
        {
            var tutor = _context.TutorApplications.Find(id);
            return View("GetTutorApplication", tutor);
        }

        /*public IActionResult IndividualTutorApplication()
        {            
            string userEmail = GetUserEmail(User.Identity);
            var tutorApplication = _context.TutorApplications/*.Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(t => t.Periods).ThenInclude(pt => pt.Period)*//*.FirstOrDefault(t => t.Email == userEmail);
            /*if (tutorApplication != null)
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
        }*/

        public IActionResult ApproveorDeny(int id)
        {
            var tutorApplication = _context.TutorApplications.FirstOrDefault(t => t.Id == id);
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
                InformationViewModel info = new InformationViewModel();
                if (_context.Tutors != null && _context.Tutors.Count() > 0 && _context.Tutors.Where(t => t.Email == tutorApplication.Email) != null && _context.Tutors.Where(t => t.Email == tutorApplication.Email).Count() > 0)
                {
                    info.Issue = "A tutor with this email already exists.";

                }
                else
                {
                    info.Issue = "This application is not valid";
                }

                return View("Information", info);
            }
        }

        [HttpPost]
        public IActionResult ApproveSelected(int[] ids)
        {
            foreach (int id in ids)
            {
                ConfirmTutorApplication(id);
            }
            return Ok();
        }

        public IActionResult ConfirmTutorApplication(int id)
        {
            var tutorApplication = _context.TutorApplications
                .FirstOrDefault(m => m.Id == id);
            if (tutorApplication == null)
            {
                return NotFound();
            }

            if (!tutorApplication.Status.Equals("Pending"))
            {
                InformationViewModel info = new InformationViewModel();
                info.Issue = "This application is approved/denied before!";
                return View("Information", info);
            }

            Tutor confirmedApplication = new Tutor();
            confirmedApplication.Id = _context.Tutors.Count() > 0 ? _context.Tutors.OrderBy(t => t.Id).ToList().Last().Id + 1 : 1;
            confirmedApplication.Name = tutorApplication.Name;
            confirmedApplication.Email = tutorApplication.Email;
            confirmedApplication.WeeklyHour = tutorApplication.WeeklyHour.ToString();

            List<CourseTutor> tutorCourses = new List<CourseTutor>();
            if (tutorApplication.CourseIds != null && tutorApplication.CourseIds.Length > 0)
            {
                foreach (var courseId in tutorApplication.CourseIds)
                {
                    tutorCourses.Add(new CourseTutor { CourseId = courseId, TutorId = confirmedApplication.Id });
                }

            }
            confirmedApplication.Courses = tutorCourses;

            List<PeriodTutor> tutorPeriods = new List<PeriodTutor>();
            if (tutorApplication.PeriodIds != null && tutorApplication.PeriodIds.Length > 0)
            {
                foreach (var periodId in tutorApplication.PeriodIds)
                {
                    tutorPeriods.Add(new PeriodTutor { PeriodId = periodId, TutorId = confirmedApplication.Id });
                } 
            }
            confirmedApplication.Periods = tutorPeriods;
            
            try
            {
                _context.Tutors.Add(confirmedApplication);
                tutorApplication.Status = "Approved";
                _context.SaveChanges();
                return RedirectToAction("TutorApplication");
            }

                catch (DbUpdateException ex)
            {
                InformationViewModel info = new InformationViewModel();
                if (_context.Tutors!=null&& _context.Tutors.Count()>0 && _context.Tutors.Where(t => t.Email == tutorApplication.Email)!=null&& _context.Tutors.Where(t => t.Email == tutorApplication.Email).Count()>0)
                {
                    info.Issue= "A tutor with this email already exists.";

                }
                else
                {
                    info.Issue = "This application is not valid";
                }
                
                return View("Information",info);
            }
        }

        [HttpPost]
        public IActionResult RejectSelected(int[] ids)
        {
            foreach (int id in ids)
            {
                DenyTutorApplication(id);
            }
            return Ok();
        }

        public IActionResult DenyTutorApplication(int id)
        {
            var tutorApplication = _context.TutorApplications
                .FirstOrDefault(m => m.Id == id);
            InformationViewModel info = new InformationViewModel();
            if (tutorApplication == null)
            {
                return NotFound();
            }
            else if (!tutorApplication.Status.Equals("Pending"))
            {
                info.Issue = "This application is approved/denied before!";
                return View("Information", info);
            }
            else if (_context.Tutors != null && _context.Tutors.Count() > 0 && _context.Tutors.Where(t => t.Email == tutorApplication.Email) != null && _context.Tutors.Where(t => t.Email == tutorApplication.Email).Count() > 0)
            {
                info.Issue = "A tutor with this email already exists.";
                return View("Information", info);
            }
            try
            {
                tutorApplication.Status = "Denied";
                _context.SaveChanges();
                return RedirectToAction("TutorApplication");
            }
            catch (DbUpdateException ex)
            {
                
                if (_context.Tutors != null && _context.Tutors.Count() > 0 && _context.Tutors.Where(t => t.Email == tutorApplication.Email) != null && _context.Tutors.Where(t => t.Email == tutorApplication.Email).Count() > 0)
                {
                    info.Issue = "A tutor with this email already exists.";

                }
                else
                {
                    info.Issue = "This application is not valid";
                }

                return View("Information", info);
            }
        }

        public static string GetUserEmail(IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.FirstOrDefault((Claim f) => f.Type == "email")?.Value;
        }

        public IActionResult Export()
        {
            List<TutorApplicationExportModel> tutorApplications = this._context.TutorApplications.Select(m => new TutorApplicationExportModel
            {
                Name = m.Name,
                Email = m.Email,
                WeeklyHour = m.WeeklyHour,
                Courses = m.CourseIds != null ? string.Join(" ", _context.Courses.Where(c => m.CourseIds.Contains(c.Id)).Select(c => c.Code)) : null,
                GPA = m.GPA.ToString(),
                Periods = m.PeriodIds != null ? string.Join(" ", _context.Periods.Where(c => m.PeriodIds.Contains(c.Id)).Select(a => string.Join(" ", a.Day, string.Join("-", a.StartHour, a.EndHour)))) : null,
                Status = m.Status
            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: tutorApplications);
            return File(stream.ToArray(), "text/plain", "TutorApplications.csv");
        }
    }
}

