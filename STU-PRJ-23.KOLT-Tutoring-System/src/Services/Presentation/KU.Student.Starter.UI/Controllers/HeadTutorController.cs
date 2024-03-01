using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Principal;

namespace KU.Student.Starter.UI.Controllers
{
    public class HeadTutorController : Controller
    {
        private readonly TutorDataContext context;
        private ExportHelper exportHelper = new ExportHelper();

        public HeadTutorController(TutorDataContext _context)
        {
            context = _context;
        }
        // GET: Tutor/Tutor
        public IActionResult HeadTutor(string searchBy, string searchString)
        {

            var headTutors = this.context.HeadTutors.ToList().Select(m => new HeadTutorViewModel
            {
                Id = m.Id,
                CourseId = m.CourseId,
                CourseCode = context.Courses.First(a => a.Id == m.CourseId).Code,
                HeadTutorsTutorId = m.HeadTutorsTutorId,
                HeadTutor = context.Tutors.First(ht => ht.Id == m.HeadTutorsTutorId),
                TutorIds = m.TutorIds,
                Tutors = string.Join(", ", context.Tutors.Where(ht => m.TutorIds.Contains(ht.Id)).Select(t => t.Name))
            }).OrderBy(c => c.CourseCode).ToList();

            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Course Code"))
                {
                    headTutors = headTutors.Where(s => s.CourseCode.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("Head Tutor"))
                {
                    headTutors = headTutors.Where(s => s.HeadTutor.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                /*else if (searchBy.Equals("Tutor"))
                {
                    headTutors = headTutors.Where(s => s....Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }*/
            }

            return View(headTutors);
        }

        [HttpGet]
        public IActionResult AssignHeadTutor()
        {

            var viewModel = new HeadTutorViewModel();
            viewModel.CourseOptions = context.TutorCourses.Select(c=>c.Course).Distinct().Select(x => new SelectListItem { Text = x.Name + ", " + x.Code, Value = x.Id.ToString() }).ToList();
            viewModel.TutorOptions = context.Tutors.Select(x => new SelectListItem { Text = x.Name + ", " + x.Email+", "+string.Join(", ",x.Courses.Select(c=>c.Course.Code)), Value = x.Id.ToString() }).ToList();
           
            return View(viewModel);

        }

        [HttpPost]
        public IActionResult AssignHeadTutor(HeadTutorViewModel model)
        {
            HeadTutor htutor = new HeadTutor();
            List<HeadTutorConnection> tutors = new List<HeadTutorConnection>();
            htutor.HeadTutorsTutorId = model.HeadTutorsTutorId;
            htutor.CourseId=model.CourseId;
            if (model.TutorIds != null && model.TutorIds.Length > 0)
            {

                tutors.Add(new HeadTutorConnection { TutorIds = model.TutorIds, HeadTutorId = model.Id });

                htutor.Tutors = tutors;
            }
            htutor.TutorIds = model.TutorIds;
            try
            {
                ModelState["CourseId"].Errors.Clear();
                ModelState["HeadTutorsTutorId"].Errors.Clear();
                ModelState["Tutors"].Errors.Clear();

                if (model.CourseId == 0 || model.HeadTutorsTutorId == 0 || model.TutorIds == null || model.TutorIds.Length == 0)
                {
                    throw new Exception("error");
                }
                context.HeadTutors.Add(htutor);
                context.SaveChanges();
                return RedirectToAction("HeadTutor");
            }
            catch (Exception e)
            {
                model.CourseOptions = context.TutorCourses.Select(c => c.Course).Distinct().Select(x => new SelectListItem { Text = x.Name + ", " + x.Code, Value = x.Id.ToString() }).ToList();
                model.TutorOptions = context.Tutors.Select(x => new SelectListItem { Text = x.Name + ", " + x.Email + ", " + string.Join(", ", x.Courses.Select(c => c.Course.Code)), Value = x.Id.ToString() }).ToList();



                if (e.Message == "error")
                {
                    if (model.CourseId == 0)
                    {

                        ModelState.AddModelError("CourseId", "The Course field is required.");
                    }

                    if (model.HeadTutorsTutorId == 0)
                    {
                        ModelState.AddModelError("HeadTutorsTutorId", "The Head Tutor field is required.");
                    }

                    if (model.TutorIds == null || model.TutorIds.Length == 0)
                    {
                        ModelState.AddModelError("Tutors", "The Tutors field is required.");
                    }
                }

                return View(model);
            }
            
        }

        [HttpGet]
        public IActionResult UpdateHeadTutor(int? id)
        {

            var viewModel = new HeadTutorViewModel();
            List<int> tutorIds = new List<int>();

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var htutor = context.HeadTutors.FirstOrDefault(x => x.Id == id.Value);
                viewModel.TutorOptions = context.Tutors.Where(x => x.Id != htutor.HeadTutorsTutorId).Where(t=>t.Courses.Select(tt=>tt.CourseId).Contains(htutor.CourseId)).Select(x => new SelectListItem { Text = x.Name + ", " + x.Email + ", " + string.Join(", ", x.Courses.Select(c => c.Course.Code)), Value = x.Id.ToString() }).ToList();
                viewModel.Id = htutor.Id;
                viewModel.HeadTutorsTutorId = htutor.HeadTutorsTutorId;
                viewModel.HeadTutor = context.Tutors.First(ht => ht.Id == htutor.HeadTutorsTutorId);
                viewModel.TutorIds = htutor.TutorIds;
                viewModel.Tutors = string.Join(", ", context.Tutors.Where(ht => htutor.TutorIds.Contains(ht.Id)).Select(t => t.Name));
                viewModel.CourseId =htutor.CourseId;
                viewModel.CourseCode = context.Courses.First(x => x.Id == htutor.CourseId).Code;
            }

            return View(viewModel);

        }
        [HttpPost]
        public IActionResult UpdateHeadTutor(HeadTutorViewModel model)
        {
            if (model.Id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid input.");
            }
            HeadTutor htutor = context.HeadTutors.Find(model.Id);
            List<HeadTutorConnection> tutorConnections = new List<HeadTutorConnection>();
            context.HeadTutorConnections.Where(htc => htc.HeadTutorId == model.Id).ToList().ForEach(r => { context.HeadTutorConnections.Remove(r); });
            if (model.TutorIds != null && model.TutorIds.Length > 0)
            {
                tutorConnections = new List<HeadTutorConnection>
                    {
                        new HeadTutorConnection { TutorIds = model.TutorIds, HeadTutorId = model.HeadTutorsTutorId }

                    };
                context.HeadTutorConnections.AddRange(tutorConnections);
                htutor.Tutors = tutorConnections;
                htutor.TutorIds = model.TutorIds;
            }
            
            context.SaveChanges();
            return RedirectToAction("HeadTutor");
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
        public IActionResult DeleteHeadTutor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var htutor = context.HeadTutors
                .FirstOrDefault(m => m.Id == id);
            if (htutor == null)
            {
                return NotFound();
            }
            var headTutorDelete = this.context.HeadTutors.First(m => m.Id == id);
            HeadTutorViewModel headTutorDeleteView = new HeadTutorViewModel();
            headTutorDeleteView.Id = headTutorDelete.Id;
            headTutorDeleteView.CourseCode  = context.Courses.Find(headTutorDelete.CourseId).Code;
            headTutorDeleteView.HeadTutorsTutorId = headTutorDelete.HeadTutorsTutorId;
            headTutorDeleteView.HeadTutor = context.Tutors.First(ht => ht.Id == headTutorDelete.HeadTutorsTutorId);
            headTutorDeleteView.TutorIds = headTutorDelete.TutorIds;
            headTutorDeleteView.Tutors = string.Join(", ", context.Tutors.Where(ht => headTutorDelete.TutorIds.Contains(ht.Id)).Select(t => t.Name));
            return View(headTutorDeleteView);
        }
        public IActionResult GetFilteredTutors(int courseId)
        {
        
            // Assuming you have a database context called 'context' and a 'Tutors' DbSet

            var tutors = context.Tutors
    .Where(t => t.Courses.Any(c => c.CourseId == courseId))
    .Select(x => new SelectListItem { Text = x.Name + ", " + x.Email + ", " + string.Join(", ", x.Courses.Select(c => c.Course.Code)),Value = x.Id.ToString() })
    .ToList();

            return Json(tutors);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var headTutor = context.HeadTutors.Find(id);
            var connections=context.HeadTutorConnections.Where(c => c.HeadTutorId == id).ToList();
            //var subtutors=context.Tutors.Where(t=>t.HeadTutor.Id == id).ToList();
             foreach (var con in connections)
             {
                context.HeadTutorConnections.Remove(con);
             }

             if (headTutor != null)
             {
                 context.HeadTutors.Remove(headTutor);

             }
             context.SaveChanges();
            
            return RedirectToAction("HeadTutor");  
        }

        public IActionResult AssignedTutors()
        {

            
            var mail= GetUserEmail(User.Identity);
            bool cansee = false;
            List<int>TutorIds = new List<int>();
            var headTutorsTutorIds = context.HeadTutors.Select(ht => ht.HeadTutorsTutorId).ToList();
            Tutor you = new Tutor();
            foreach (var tutor in context.Tutors)
            {
                if (tutor != null)
                {
                    
                    bool c= headTutorsTutorIds.Contains(tutor.Id);
                    if (tutor.Email == mail&&c)
                    {
                        cansee = true;
                        you = tutor;
                    }
                }
            }
            
            if (!cansee||you==null)
            {
                IEnumerable < TutorViewModel> empty= new List<TutorViewModel>();
                return View(empty);
            }
            TutorIds = context.HeadTutors.First(ht => ht.HeadTutorsTutorId == you.Id).TutorIds.ToList();
            var tutors = context.Tutors.Where(t=>TutorIds.Contains(t.Id)).Include(m => m.Courses).ThenInclude(tc => tc.Course).Include(m => m.Periods).ThenInclude(p => p.Period).ToList().Select(m => new TutorViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                WeeklyHour = m.WeeklyHour,
                Courses = string.Join(',', m.Courses.Select(a => a.Course.Code)),
                Periods = string.Join(',', m.Periods.Select(a => string.Join(", ", a.Period.Day, string.Join("-", a.Period.StartHour, a.Period.EndHour))))

            });
         
            return View(tutors);

        }

        public IActionResult Export()
        {

            List<HeadTutorExportModel> headTutors = this.context.HeadTutors.Select(m => new HeadTutorExportModel
            {

                Course = context.Courses.First(a => a.Id == m.CourseId).Code,
                HeadTutor = context.Tutors.First(ht => ht.Id == m.HeadTutorsTutorId).Name,
                Tutors = string.Join(" ", context.Tutors.Where(ht => m.TutorIds.Contains(ht.Id)).Select(t => t.Name))
            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: headTutors);
            return File(stream.ToArray(), "text/plain", "HeadTutors.csv");
        }

        public static string GetUserEmail(IIdentity identity)
        {
            return ((ClaimsIdentity)identity).Claims.FirstOrDefault((Claim f) => f.Type == "email")?.Value;
        }
    }
}
