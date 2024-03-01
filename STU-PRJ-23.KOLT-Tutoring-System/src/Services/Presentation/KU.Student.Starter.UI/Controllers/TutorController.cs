using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Security.Claims;
using System.Security.Principal;
using HeadTutor = KU.Student.Starter.UI.Models.HeadTutor;
using PeriodTutor = KU.Student.Starter.UI.Models.PeriodTutor;

namespace KU.Student.Starter.UI.Controllers
{
    public class TutorController : Controller
    {
        private readonly TutorDataContext _context;
        private ExportHelper exportHelper = new ExportHelper();

        public TutorController(TutorDataContext context)
        {
            _context = context;
        }
        // GET: Tutor/Tutor
        public IActionResult Tutor(string searchBy, string searchString)
        {


            var tutors = this._context.Tutors.Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(m => m.Periods).ThenInclude(p => p.Period).OrderBy(t => t.Name).ToList().Select(m => new TutorViewModel
        {
            Id = m.Id,
            Name = m.Name,
            Email = m.Email,
            WeeklyHour = m.WeeklyHour,
            Courses = string.Join(',',m.Courses.Select(a => a.Course.Code)),
            Periods = string.Join(',',m.Periods.Select(a => string.Join(", ",a.Period.Day,string.Join("-",a.Period.StartHour,a.Period.EndHour))))

            });



            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Name"))
                {                    
                    tutors = tutors.Where(s => s.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("Email"))
                {

                    tutors = tutors.Where(s => s.Email!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("Courses"))
                {

                   tutors = tutors.Where(s => s.Courses!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("Periods"))
                {

                    tutors = tutors.Where(s => RemoveWhiteSpaces(s.Periods)!.Contains(RemoveWhiteSpaces(searchString), StringComparison.OrdinalIgnoreCase)).ToList();
                }
                else if (searchBy.Equals("WeeklyHour"))
                {

                    tutors = tutors.Where(s => s.WeeklyHour!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }

            }
            return View(tutors);

        }
        private string RemoveWhiteSpaces(string input)
        {
            return new string(input.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        [HttpGet]
        public IActionResult AddTutor()
        {

            var viewModel = new TutorViewModel();
            viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
            viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
           
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult AddTutor(TutorViewModel model)
        {

            Tutor tutor = new Tutor();
            List<CourseTutor> tutorCourses = new List<CourseTutor>();
            List<PeriodTutor> tutorPeriods = new List<PeriodTutor>();
            //Tutor Id is a foreign key for both tutorcoursetable and periodtutor table.
            //When we create tutor we assign it directly to periods and courses.
            //This means we create table content for those tables, with foreign key tutor.Id.
            //But we do not have tutor Id at creation we were doing it automatically, it is a PK.
            //Since this tutor Id being not assigned and being foreign key at the same time results in an internal error
            //decided to manually increase id of tutor at creation.
            //But this is not a good way of solving the issue. Better solution will be done later. 
            tutor.Id = _context.Tutors.Count()>0? _context.Tutors.OrderBy(t=>t.Id).ToList().Last().Id+ 1:1;
            tutor.Name = model.Name;
            tutor.WeeklyHour = model.WeeklyHour;
            tutor.Email = model.Email;
            if (model.CourseIds != null && model.CourseIds.Length > 0)
            {
                foreach(var courseId in model.CourseIds)
                {
                    tutorCourses.Add(new CourseTutor { CourseId = courseId, TutorId = model.Id });
                }
                tutor.Courses = tutorCourses;
            }

            if (model.PeriodIds != null && model.PeriodIds.Length > 0)
            {
                foreach (var periodId in model.PeriodIds)
                {
                    tutorPeriods.Add(new PeriodTutor { PeriodId = periodId, TutorId = model.Id });
                }
                tutor.Periods = tutorPeriods;
            }

            try
            {
                ModelState["Name"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                ModelState["Courses"].Errors.Clear();
                ModelState["Periods"].Errors.Clear();
                ModelState["WeeklyHour"].Errors.Clear();


                if (model.CourseIds == null || model.CourseIds.Length == 0 || model.PeriodIds == null || model.PeriodIds.Length == 0 || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                {
                    throw new Exception("error");
                }

                _context.Tutors.Add(tutor);
                _context.SaveChanges();
                return RedirectToAction("Tutor");
            }
            catch (DbUpdateException ex)
            {
                model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "A tutor with this email already exists.");
        
                }

                return View(model);
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

                    if (string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                    {
                        ModelState.AddModelError("WeeklyHour", "The Weekly Hour field is required.");
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
        public IActionResult UpdateTutor(int? id)
        {

            var viewModel = new TutorViewModel();
            List<int> courseIds = new List<int>();
            List<int> periodIds = new List<int>();

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var tutor = _context.Tutors.Include("Courses").Include("Periods").FirstOrDefault(x => x.Id == id.Value);
                tutor.Courses.ToList().ForEach(Results => courseIds.Add(Results.CourseId));
                tutor.Periods.ToList().ForEach(Results => periodIds.Add(Results.PeriodId));
                viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString()}).ToList();
                viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text =  x.Day + ", "+ x.StartHour +"-"+ x.EndHour, Value = x.Id.ToString() }).ToList();
                viewModel.Id = tutor.Id;
                viewModel.Name = tutor.Name;
                viewModel.Email = tutor.Email;  
                viewModel.WeeklyHour = tutor.WeeklyHour;
                viewModel.CourseIds = courseIds.ToArray();
                viewModel.PeriodIds = periodIds.ToArray();
            }

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult UpdateTutor(TutorViewModel model)
        {

            try
            {

                ModelState["Name"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                ModelState["Courses"].Errors.Clear();
                ModelState["Periods"].Errors.Clear();
                ModelState["WeeklyHour"].Errors.Clear();

                if (model.CourseIds == null || model.CourseIds.Length == 0 || model.PeriodIds == null || model.PeriodIds.Length == 0 || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                {
                    throw new Exception("error");
                }

                // Check if the email address is already taken by another tutor
                var existingTutor = _context.Tutors.FirstOrDefault(t => t.Email == model.Email && t.Id != model.Id);
                if (existingTutor != null)
                {
                    ModelState.AddModelError("Email", "A tutor with this email already exists.");
                    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                    return View(model);
                }

                Tutor tutor = new Tutor();
                List<CourseTutor> tutorCourses = new List<CourseTutor>();
                List<PeriodTutor> tutorPeriods = new List<PeriodTutor>();
                List<PeriodCubicle> tutorCubicles = new List<PeriodCubicle>();

                if (model.Id != null)
                {
                    tutor = _context.Tutors.Include("Courses").Include("Periods").FirstOrDefault(x => x.Id == model.Id);
                    tutor.Courses.ToList().ForEach(Results => tutorCourses.Add(Results));
                    tutor.Periods.ToList().ForEach(Results =>
                    {
                        if (!model.PeriodIds.Contains(Results.PeriodId))
                        {
                            tutorPeriods.Add(Results); //store deleted periods to remove them from database.
                                                       // also remove these from period cubicles
                            _context.PeriodCubicles.Where(p => p.PeriodTutorId == Results.PeriodTutorId).ToList().ForEach(pc =>
                            {
                                tutorCubicles.Add(pc);
                            }
                                );

                        }
                    });
                    _context.TutorCourses.RemoveRange(tutorCourses);
                    _context.TutorPeriods.RemoveRange(tutorPeriods);
                    _context.PeriodCubicles.RemoveRange(tutorCubicles);
                    _context.SaveChanges();
                    tutor.Name = model.Name;
                    tutor.Email = model.Email;
                    tutor.WeeklyHour = model.WeeklyHour;

                    if (model.CourseIds != null && model.CourseIds.Length > 0)
                    {
                        tutorCourses = new List<CourseTutor>();
                        foreach (var courseId in model.CourseIds)
                        {
                            tutorCourses.Add(new CourseTutor { CourseId = courseId, TutorId = model.Id });
                        }
                        tutor.Courses = tutorCourses;
                    }

                    if (model.PeriodIds != null && model.PeriodIds.Length > 0)
                    {
                        tutorPeriods = new List<PeriodTutor>();
                        foreach (var periodId in model.PeriodIds)
                        {
                            var existingPeriodTutor = tutor.Periods.FirstOrDefault(pt => pt.PeriodId == periodId);
                            // Check if period already exists in tutorPeriods list
                            if (existingPeriodTutor == null)
                            {
                                tutorPeriods.Add(new PeriodTutor { PeriodId = periodId, TutorId = model.Id });
                            }
                            else
                            {
                                tutorPeriods.Add(existingPeriodTutor);
                            }
                        }
                        tutor.Periods = tutorPeriods;
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Tutor");

            }
            catch (DbUpdateException ex)
            {
                model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "A tutor with this email already exists.");

                }
                return View(model);
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

                    if (string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                    {
                        ModelState.AddModelError("WeeklyHour", "The Weekly Hour field is required.");
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
        public IActionResult DeleteTutor(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            // var tutor = _context.Tutors
            //   .FirstOrDefault(m => m.Id == id);
            var tutor = _context.Tutors.Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(t => t.Periods).ThenInclude(pt => pt.Period).FirstOrDefault(m => m.Id == id);
            if (tutor == null)
            {
                return NotFound();
            }

            return View(tutor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
           // var tutorCourses = _context.TutorCourses.Where(x => x.TutorId == id).ToList();      
            var tutor = _context.Tutors.Find(id);
            var pcs = _context.PeriodCubicles.ToList();
            var itemsToRemove = new List<PeriodCubicle>();

            var headtutorCon = _context.HeadTutorConnections.ToList();
            var headtutors = _context.HeadTutors.ToList();
            List<HeadTutorConnection> wildeletehtc = new List<HeadTutorConnection>();
            List<HeadTutor> wildeleteht = new List<HeadTutor>();
            if (headtutorCon!=null&& headtutorCon.Count() > 0)
            {
                bool htremved = false;
                foreach (var deleteht in headtutorCon.Where(x => headtutors.First(y => y.Id == x.HeadTutorId).HeadTutorsTutorId==id).ToList())
                {
                    if (!htremved)
                    {
                        wildeleteht.Add(headtutors.First(x => x.Id == deleteht.HeadTutorId));
                        htremved = true;
                    }
                    wildeletehtc.Add(deleteht);
                }
                foreach (var htc in headtutorCon)
                {
                    if (htc.TutorIds.Contains(id))
                    {
                        if (htc.TutorIds.Length == 1)
                        {
                            wildeletehtc.Add(htc);
                            wildeleteht.Add(headtutors.First(x => x.Id == htc.HeadTutorId));
                        }
                        else
                        {
                            htc.TutorIds.ToList().Remove(id);
                        }
                    }
                }
                if (wildeletehtc != null)
                {
                    _context.HeadTutorConnections.RemoveRange(wildeletehtc);
                }
                if (wildeleteht != null)
                {
                    _context.HeadTutors.RemoveRange(wildeleteht);
                }
            }
            foreach (var pc in pcs)
            {
                var pt = _context.TutorPeriods.FirstOrDefault(tp => tp.PeriodTutorId == pc.PeriodTutorId && tp.TutorId == id);
                if (pt != null)
                {
                    itemsToRemove.Add(pc);
                }
            }

            foreach (var pcToRemove in itemsToRemove)
            {
                _context.PeriodCubicles.Remove(pcToRemove);
            }


          /*  foreach (var pc in pcs)
            {
                var pt = _context.TutorPeriods.Where(tp => tp.PeriodTutorId == pc.PeriodTutorId).ToList() ;
                if (pt.Count>0&&pt.First().TutorId == id) 
                {
                    _context.PeriodCubicles.Remove(pc);
                }
            }*/
            if (tutor != null)
            {
                _context.Tutors.Remove(tutor);
                
            }
            _context.SaveChanges();
            return RedirectToAction("Tutor");
        }

        public IActionResult GetTutor(int? id)
        {
            var tutor = _context.Tutors.Find(id);
            return View("GetTutor", tutor);
        }
        public IActionResult Export()
        {
            List<TutorExportModel> tutors = this._context.Tutors.Select(m => new TutorExportModel
            {
                Name = m.Name,
                Email = m.Email,
                WeeklyHour = int.Parse(m.WeeklyHour),
                Courses = string.Join(' ', m.Courses.Select(a => a.Course.Code)),
                Periods = string.Join(' ', m.Periods.Select(a => string.Join(" ", a.Period.Day, string.Join("-", a.Period.StartHour, a.Period.EndHour))))

            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: tutors);
            return File(stream.ToArray(), "text/plain", "Tutors.csv");
        }

        public IActionResult IndividualTutorPage()
        {
            string userEmail = GetUserEmail(User.Identity);
            var tutor = _context.Tutors.Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(t => t.Periods).ThenInclude(pt => pt.Period).FirstOrDefault(t => t.Email == userEmail);
            return View(tutor);
        }

        public static string GetUserEmail(IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.FirstOrDefault((Claim f) => f.Type == "email")?.Value;
        }

        [HttpGet]
        public IActionResult UpdateIndividualTutor(int? id)
        {

            var viewModel = new TutorViewModel();
            List<int> courseIds = new List<int>();
            List<int> periodIds = new List<int>();

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var tutor = _context.Tutors.Include("Courses").Include("Periods").FirstOrDefault(x => x.Id == id.Value);
                tutor.Courses.ToList().ForEach(Results => courseIds.Add(Results.CourseId));
                tutor.Periods.ToList().ForEach(Results => periodIds.Add(Results.PeriodId));
                viewModel.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                viewModel.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                viewModel.Id = tutor.Id;
                viewModel.Name = tutor.Name;
                viewModel.Email = tutor.Email;
                viewModel.WeeklyHour = tutor.WeeklyHour;
                viewModel.CourseIds = courseIds.ToArray();
                viewModel.PeriodIds = periodIds.ToArray();
            }

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult UpdateIndividualTutor(TutorViewModel model)
        {

            try
            {

                ModelState["Name"].Errors.Clear();
                ModelState["Email"].Errors.Clear();
                ModelState["Courses"].Errors.Clear();
                ModelState["Periods"].Errors.Clear();
                ModelState["WeeklyHour"].Errors.Clear();

                if (model.CourseIds == null || model.CourseIds.Length == 0 || model.PeriodIds == null || model.PeriodIds.Length == 0 || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                {
                    throw new Exception("error");
                }

                // Check if the email address is already taken by another tutor
                var existingTutor = _context.Tutors.FirstOrDefault(t => t.Email == model.Email && t.Id != model.Id);
                if (existingTutor != null)
                {
                    ModelState.AddModelError("Email", "A tutor with this email already exists.");
                    model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                    model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();
                    return View(model);
                }

                Tutor tutor = new Tutor();
                List<CourseTutor> tutorCourses = new List<CourseTutor>();
                List<PeriodTutor> tutorPeriods = new List<PeriodTutor>();
                List<PeriodCubicle> tutorCubicles = new List<PeriodCubicle>();

                if (model.Id != null)
                {
                    tutor = _context.Tutors.Include("Courses").Include("Periods").FirstOrDefault(x => x.Id == model.Id);
                    tutor.Courses.ToList().ForEach(Results => tutorCourses.Add(Results));
                    tutor.Periods.ToList().ForEach(Results =>
                    {
                        if (!model.PeriodIds.Contains(Results.PeriodId))
                        {
                            tutorPeriods.Add(Results); //store deleted periods to remove them from database.
                                                       // also remove these from period cubicles
                            _context.PeriodCubicles.Where(p => p.PeriodTutorId == Results.PeriodTutorId).ToList().ForEach(pc =>
                            {
                                tutorCubicles.Add(pc);
                            }
                                );

                        }
                    });
                    _context.TutorCourses.RemoveRange(tutorCourses);
                    _context.TutorPeriods.RemoveRange(tutorPeriods);
                    _context.PeriodCubicles.RemoveRange(tutorCubicles);
                    _context.SaveChanges();
                    tutor.Name = model.Name;
                    tutor.Email = model.Email;
                    tutor.WeeklyHour = model.WeeklyHour;

                    if (model.CourseIds != null && model.CourseIds.Length > 0)
                    {
                        tutorCourses = new List<CourseTutor>();
                        foreach (var courseId in model.CourseIds)
                        {
                            tutorCourses.Add(new CourseTutor { CourseId = courseId, TutorId = model.Id });
                        }
                        tutor.Courses = tutorCourses;
                    }

                    if (model.PeriodIds != null && model.PeriodIds.Length > 0)
                    {
                        tutorPeriods = new List<PeriodTutor>();
                        foreach (var periodId in model.PeriodIds)
                        {
                            var existingPeriodTutor = tutor.Periods.FirstOrDefault(pt => pt.PeriodId == periodId);
                            // Check if period already exists in tutorPeriods list
                            if (existingPeriodTutor == null)
                            {
                                tutorPeriods.Add(new PeriodTutor { PeriodId = periodId, TutorId = model.Id });
                            }
                            else
                            {
                                tutorPeriods.Add(existingPeriodTutor);
                            }
                        }
                        tutor.Periods = tutorPeriods;
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("IndividualTutorPage");

            }
            catch (DbUpdateException ex)
            {
                model.CourseOptions = _context.Courses.Select(x => new SelectListItem { Text = x.Code, Value = x.Id.ToString() }).ToList();
                model.PeriodOptions = _context.Periods.Select(x => new SelectListItem { Text = x.Day + ", " + x.StartHour + "-" + x.EndHour, Value = x.Id.ToString() }).ToList();

                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Email", "A tutor with this email already exists.");

                }
                return View(model);
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

                    if (string.IsNullOrEmpty(model.WeeklyHour) || model.WeeklyHour == "0")
                    {
                        ModelState.AddModelError("WeeklyHour", "The Weekly Hour field is required.");
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
    }
}
