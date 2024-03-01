using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace KU.Student.Starter.UI.Controllers
{
    public class SetCubiclesController : Controller
    {
        private readonly Models.TutorDataContext context;
        public SetCubiclesController(Models.TutorDataContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Daily(string day)
        {
            return View(DailyPlan(day));
        }
        // GET: /SetCubicles/Weekly
        [HttpGet]
        public IActionResult Weekly()
        {
            WeeklyViewModel w = new WeeklyViewModel();
            w.Periods= new List<Period>();
            w.MondayHours = new Dictionary<string,int>();
            w.TuesdayHours = new Dictionary<string, int>();
            w.WednesdayHours = new Dictionary<string, int>();
            w.ThursdayHours = new Dictionary<string, int>();
            w.FridayHours = new Dictionary<string, int>();
            string time = "";
            foreach(var pct in context.PeriodCubicles.ToList())
            {
                if (!PeriodTutorChecker(pct))
                {
                    continue;
                }
                pct.PeriodTutor = context.TutorPeriods.First(pt => pt.PeriodTutorId == pct.PeriodTutorId);
                pct.PeriodTutor.Tutor = context.Tutors.First(t => t.Id == pct.PeriodTutor.TutorId);
                pct.PeriodTutor.Period = context.Periods.First(p => p.Id == pct.PeriodTutor.PeriodId);
                w.Periods.Add(pct.PeriodTutor.Period);
                time = string.Join("-", pct.PeriodTutor.Period.StartHour, pct.PeriodTutor.Period.EndHour);
                switch (pct.PeriodTutor.Period.Day)
                {
                    case "Monday":
                        if (!w.MondayHours.ContainsKey(time)){
                            w.MondayHours.Add(time,1);
                        }
                        else
                        {
                            w.MondayHours[time]++;
                        }
                        break;
                    case "Tuesday":
                        if (!w.TuesdayHours.ContainsKey(time))
                        {
                            w.TuesdayHours.Add(time, 1);
                        }
                        else
                        {
                            w.TuesdayHours[time]++;
                        }
                        break;
                    case "Wednesday":
                        if (!w.WednesdayHours.ContainsKey(time))
                        {
                            w.WednesdayHours.Add(time, 1);
                        }
                        else
                        {
                            w.WednesdayHours[time]++;
                        }
                        break;
                    case "Thursday":
                        if (!w.ThursdayHours.ContainsKey(time))
                        {
                            w.ThursdayHours.Add(time, 1);
                        }
                        else
                        {
                            w.ThursdayHours[time]++;
                        }
                        break;
                    case "Friday":
                        if (!w.FridayHours.ContainsKey(time))
                        {
                            w.FridayHours.Add(time, 1);
                        }
                        else
                        {
                            w.FridayHours[time]++;
                        }
                        break;
                    default:
                        break; // any other value will be sorted last
                }

            }
            return View(w);
        }
        [HttpGet]
        public IActionResult Combined()
        {
            CombinedViewModel cm = new CombinedViewModel();
            cm.Monday = DailyPlan("Monday");
            cm.Tuesday = DailyPlan("Tuesday");
            cm.Wednesday = DailyPlan("Wednesday");
            cm.Thursday = DailyPlan("Thursday");
            cm.Friday = DailyPlan("Friday");
            return View(cm);
        }
        [HttpGet]
        public IActionResult CombinedStudent()
        { 
            CombinedViewModel cm = new CombinedViewModel();
            cm.Monday = DailyPlanOnView("Monday");
            cm.Tuesday = DailyPlanOnView("Tuesday");
            cm.Wednesday = DailyPlanOnView("Wednesday");
            cm.Thursday = DailyPlanOnView("Thursday");
            cm.Friday = DailyPlanOnView("Friday");
            return PartialView(cm);
        }
        [HttpGet]
        public IActionResult CombinedCoursely()
        {
            CombinedCourseViewModel ccvm= new CombinedCourseViewModel();
            ccvm.Courselies = new List<CouselyScheduleTable>();
            List<int> checkedCourseIds =new  List<int>();
            context.TutorCourses.ToList().ForEach(r =>
            {
                if(checkedCourseIds==null|| !checkedCourseIds.Contains(r.CourseId))
                {
                    ccvm.Courselies.Add(CourselyPlan(r.CourseId));
                    checkedCourseIds.Add(r.CourseId);
                }
                
            });
            if (ccvm.Courselies != null && ccvm.Courselies.Count > 0)
            {
                ccvm.SortedCourselies = ccvm.Courselies.OrderBy(c => c.CourseCode).ToList();
            }
            else
            {
                ccvm.SortedCourselies = new List<CouselyScheduleTable>();
            }
            EditScheduleTexts current = new EditScheduleTexts();
            if (context.EditScheduleTexts!=null&& context.EditScheduleTexts.Count() > 0)
            {
                current= context.EditScheduleTexts.ToList().Last();
                
            }
            ccvm.Texts = current;
            bool publish = false;
            if (context.Configurations != null && context.Configurations.Count() > 0)
            {
                publish = context.Configurations.ToList().Last().PublishSchedule;

            }
            ccvm.Publish = publish;
            return PartialView(ccvm);
        }
        /// <summary>
        /// This method assigns cubicles to tutors who chose a session 
        /// </summary>
        /// <param name="day"></param>
        /// <returns>A special modal just for the usage on view</returns>
        /// What needs to be added: Tutor might choose 3 periods but will only teach for one of the selections, how do we implement this?
        public SessionCubicleTableViewModel DailyPlan(string day)
        {
            var periodTutorCubicles = context.PeriodCubicles.ToList();
            var ptcPerTutId = periodTutorCubicles.Select(p => p.PeriodTutorId).ToList();
            var cubicles = context.Cubicles.ToList();
            var sessions = context.Periods.Where(p => p.Day == day).ToList();
            var tutorPeriods = context.TutorPeriods.ToList();
            var tutorCourse = context.TutorCourses.ToList();
            var courses = context.Courses.ToList();

            bool assigned = false;
            bool available = true;
            foreach (var tps in tutorPeriods) //Tutor periodlardan cubicle atanmamış olanlara gidiyoruz
            {
                assigned = false;
                periodTutorCubicles = context.PeriodCubicles.ToList();
                ptcPerTutId = periodTutorCubicles.Select(p => p.PeriodTutorId).ToList();

                tps.Period = context.Periods.First(p => p.Id == tps.PeriodId);
                tps.Tutor = context.Tutors.First(p => p.Id == tps.TutorId);
                if (!ptcPerTutId.Contains(tps.PeriodTutorId)) //eğer cubicle atanmadıysa
                {
                    PeriodCubicle newie = new PeriodCubicle();
                    newie.PeriodTutor = tps;
                    newie.PeriodTutorId = tps.PeriodTutorId;

                    foreach (var cub in cubicles) //eşlenmiş cubiclerde geziyoruz
                    {
                        available = true;
                        foreach (var ptc in periodTutorCubicles) //her bir cubicle için
                        {
                            if (!PeriodTutorChecker(ptc))
                            {
                                continue;
                            }
                            if (ptc.CubicleId == cub.Id && ptc.PeriodTutor.PeriodId == tps.PeriodId) //eşlenmiş cubic id si bizim cubiğe eşitse ve periodu da eşitse
                                {
                                    available = false;
                                }
                                              
                        }
                        if (available)
                        {
                            newie.Cubicle = cub;
                            newie.CubicleId = cub.Id;
                            assigned = true;
                            break;
                        }

                    }
                    if (!assigned)
                    {
                        newie.Cubicle = cubicles.First();
                        newie.CubicleId = cubicles.First().Id;
                        assigned = true;
                    }
                    context.PeriodCubicles.Add(newie);
                    context.SaveChanges();
                }
            }
            periodTutorCubicles = context.PeriodCubicles.ToList();
            SessionCubicleTableViewModel combined = new SessionCubicleTableViewModel();
            combined.Cubicles = cubicles;
            combined.Periods = sessions;
            combined.PeriodCubicles = periodTutorCubicles;
            combined.TutorCourses = tutorCourse;
            combined.Courses = courses;
            combined.Day = day;
            return combined;
        }
        public SessionCubicleTableViewModel DailyPlanOnView(string day)
        {
            var periodTutorCubicles = context.PeriodCubicles.ToList();
            var cubicles = context.Cubicles.ToList();
            var sessions = context.Periods.Where(p => p.Day == day).ToList();
            var tutorCourse = context.TutorCourses.ToList();
            var courses = context.Courses.ToList();
            foreach(var pct in periodTutorCubicles)
            {
                if (!PeriodTutorChecker(pct))
                {
                    continue;
                }
                   
                 pct.PeriodTutor=context.TutorPeriods.First(pt=>pt.PeriodTutorId==pct.PeriodTutorId);
                 pct.PeriodTutor.Tutor = context.Tutors.First(t => t.Id == pct.PeriodTutor.TutorId);
                 pct.PeriodTutor.Period = context.Periods.First(p => p.Id == pct.PeriodTutor.PeriodId);
            }
            SessionCubicleTableViewModel combined = new SessionCubicleTableViewModel();
            combined.Cubicles = cubicles;
            combined.Periods = sessions;
            combined.PeriodCubicles = periodTutorCubicles;
            combined.TutorCourses = tutorCourse;
            combined.Courses = courses;
            combined.Day = day;
            return combined;
        }
        public CouselyScheduleTable CourselyPlan(int courseId)
        {
            CouselyScheduleTable viewModel= new CouselyScheduleTable();
            RowsCouselyScheduleTable row = new RowsCouselyScheduleTable();
            var course = context.Courses.Find(courseId);
            viewModel.CourseId=courseId;
            viewModel.CourseCode = course.Code;
            viewModel.Rows = new List<RowsCouselyScheduleTable>();
            var periodTutorCubicles = context.PeriodCubicles.ToList();
            var tutorsIdsOfCourse = context.TutorCourses.Where(tc=>tc.CourseId==courseId).Select(tc=>tc.TutorId).ToList();
            foreach (var pct in periodTutorCubicles)
            {
                if (!PeriodTutorChecker(pct))
                {
                    continue;
                }
                pct.PeriodTutor = context.TutorPeriods.First(pt => pt.PeriodTutorId == pct.PeriodTutorId);
                pct.PeriodTutor.Tutor = context.Tutors.First(t => t.Id == pct.PeriodTutor.TutorId);
                pct.PeriodTutor.Period = context.Periods.First(t => t.Id == pct.PeriodTutor.PeriodId);
                pct.Cubicle= context.Cubicles.First(c=>c.Id==pct.CubicleId);
                if (tutorsIdsOfCourse.Contains(pct.PeriodTutor.TutorId))
                {
                    row.Day=pct.PeriodTutor.Period.Day;
                    row.StartHour = pct.PeriodTutor.Period.StartHour;
                    row.Time = string.Join("-", pct.PeriodTutor.Period.StartHour, pct.PeriodTutor.Period.EndHour);
                    row.Place = pct.Cubicle.CubiclePlace;
                    row.Tutor = pct.PeriodTutor.Tutor.Name;
                    row.CubicleNo = pct.Cubicle.CubicleNumber;

                    viewModel.Rows.Add(row);

                    row =  new RowsCouselyScheduleTable();
                }
                
            }
            if (viewModel.Rows != null && viewModel.Rows.Count > 0)
            {
                viewModel.SortedRows=viewModel.Rows.OrderBy(row =>
                {
                    switch (row.Day)
                    {
                        case "Monday":
                            return 1;
                        case "Tuesday":
                            return 2;
                        case "Wednesday":
                            return 3;
                        case "Thursday":
                            return 4;
                        case "Friday":
                            return 5;
                        default:
                            return int.MaxValue; // any other value will be sorted last
                    }
                }).ThenBy(row => row.StartHour).ThenBy(row => row.CubicleNo).ToList();
            }
            else
            {
                viewModel.SortedRows = new List<RowsCouselyScheduleTable>();
            }
            return viewModel;
        }

        [HttpGet]
        public ActionResult SwipeCubicles(int pcId, int cubicleId, int periodId)
        {
            if (pcId == null || cubicleId == null || periodId == null) {
                ModelState.AddModelError(string.Empty, "Invalid parameters.");
                return View();

                ///////////////////////
                // return StatusCode(StatusCodes.Status400BadRequest, "Invalid parameters.");
                //////////
              //  ViewBag.ErrorMessage = "Invalid parameters.";
                //return View("Error");

            }
            PeriodCubicle swiped = context.PeriodCubicles.FirstOrDefault(s => s.PeriodCubicleId == pcId);
            Cubicle newloc = context.Cubicles.First(p => p.Id == cubicleId);
            if (swiped == null || newloc == null) {

                return StatusCode(StatusCodes.Status400BadRequest, "Invalid parameters.");
                /////
                ///ViewBag.ErrorMessage = "Invalid parameters.";
                // return View("Error");
                ///
                //ModelState.AddModelError(string.Empty, "Invalid parameters.");
                //return View();

            }
            PeriodTutor currentPeriodTutor = context.TutorPeriods.First(p => p.PeriodTutorId == swiped.PeriodTutorId);

            if (currentPeriodTutor.PeriodId != periodId)
            {
                PeriodTutor newPeriodTutor = new PeriodTutor();
                newPeriodTutor.TutorId = currentPeriodTutor.TutorId;
                newPeriodTutor.PeriodId = periodId;
                context.TutorPeriods.Remove(currentPeriodTutor);
                context.TutorPeriods.Add(newPeriodTutor);
                currentPeriodTutor = newPeriodTutor;
            }

            PeriodCubicle newie = new PeriodCubicle();
            newie.PeriodTutorId = currentPeriodTutor.PeriodTutorId;
            newie.PeriodTutor = currentPeriodTutor;
            newie.Cubicle = newloc;
            newie.CubicleId = newloc.Id;
            context.PeriodCubicles.Remove(swiped);
            context.PeriodCubicles.Add(newie);
            context.SaveChanges();

            return Ok();
        }

        private bool PeriodTutorChecker(PeriodCubicle pc)
        {

            if(context.TutorPeriods==null|| context.TutorPeriods.Count() <= 0)
            {
                context.PeriodCubicles.Remove(pc);
                context.SaveChanges();
                return false;
            }
            if (!context.TutorPeriods.Where(tp=>tp.PeriodTutorId == pc.PeriodTutorId).Any())
            {
                context.PeriodCubicles.Remove(pc);
                context.SaveChanges();
                return false;
            }
            return true;
        }
    }
}