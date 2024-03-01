using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace KU.Student.Starter.UI.Controllers
{
    public class PeriodController : Controller
    {
        private readonly Models.TutorDataContext context;
        private ExportHelper exportHelper = new ExportHelper();


        public PeriodController(Models.TutorDataContext context)
        {
            this.context = context;

        }

        public IActionResult Period(string searchBy, string searchString)
        {
            var periods = this.context.Periods.ToList().OrderBy(p =>
            {
                switch (p.Day)
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
            }).ThenBy(c => c.StartHour).ThenBy(c => c.EndHour).ToList().Select(m => new PeriodViewModel
            {
                Id = m.Id,
                Day = m.Day,
                StartHour=m.StartHour,
                EndHour=m.EndHour

            });

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("Day"))
                {
                    periods = periods.Where(s => s.Day!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                if (searchBy.Equals("Start Hour"))
                {
                    periods = periods.Where(s => s.StartHour!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                if (searchBy.Equals("End Hour"))
                {
                    periods = periods.Where(s => s.EndHour!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
            }
            return View(periods);
        }

        [HttpGet]
        public IActionResult AddPeriod()
        {
            var viewModel = new PeriodViewModel();
            viewModel.DayOptions = new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "All Days", Value = "All days" },
                                        new SelectListItem { Text = "Monday", Value = "Monday" },
                                        new SelectListItem { Text = "Tuesday", Value = "Tuesday" },
                                        new SelectListItem { Text = "Wednesday", Value = "Wednesday" },
                                        new SelectListItem { Text = "Thursday", Value = "Thursday" },
                                        new SelectListItem { Text = "Friday", Value = "Friday" },
                                        new SelectListItem { Text = "Saturday", Value = "Saturday" },
                                        new SelectListItem { Text = "Sunday", Value = "Sunday" }
                                    };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddPeriod(Period m)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new PeriodViewModel();
                viewModel.Day = m.Day;
                viewModel.StartHour = m.StartHour;
                viewModel.EndHour = m.EndHour;
                return View(viewModel);
            }

            try
            {
                TimeOnly startTime = new TimeOnly(int.Parse(m.StartHour.Split(":")[0]), int.Parse(m.StartHour.Split(":")[1]));
                TimeOnly endTime = new TimeOnly(int.Parse(m.EndHour.Split(":")[0]), int.Parse(m.EndHour.Split(":")[1]));
                if (startTime < endTime)
                {
                    if (m.Day == "All days")
                    {
                        List<string> dayOfWeek = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
                        foreach (var item in dayOfWeek)
                        {
                            Period alldays = new Period();
                            alldays.Day = item;
                            alldays.StartHour = m.StartHour;
                            alldays.EndHour = m.EndHour;
                            context.Periods.Add(alldays);
                        }
                    }
                    else
                    {
                        context.Periods.Add(m);
                    }
                    context.SaveChanges();
                    return RedirectToAction("Period");
                }
                else
                {
                    ModelState.AddModelError("StartHour", "Start hour of the period should be earlier than end hour.");
                    return View();
                }

            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Day", "There is already a period with this day, start and end time.");
                    ModelState.AddModelError("StartHour", "There is already a period with this day, start and end time.");
                    ModelState.AddModelError("EndHour", "There is already a period with this day, start and end time.");
                    var viewModel = new PeriodViewModel();
                    viewModel.Day = m.Day;
                    viewModel.StartHour = m.StartHour;
                    viewModel.EndHour = m.EndHour;
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

        public IActionResult DeletePeriod(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var period = context.Periods
                .FirstOrDefault(m => m.Id == id);
            if (period == null)
            {
                return NotFound();
            }

            return View(period);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var pcs = context.PeriodCubicles.ToList();
            foreach (var pc in pcs)
            {
                var pt = context.TutorPeriods.Where(tp => tp.PeriodTutorId == pc.PeriodTutorId).ToList();
                if (pt.Count > 0 && pt.First().PeriodId == id)
                {
                    context.PeriodCubicles.Remove(pc);
                }
            }

            var period = context.Periods.Find(id);
            if (period != null)
            {
                context.Periods.Remove(period);
            }

            context.SaveChanges();
            return RedirectToAction("Period");
        }

        public IActionResult GetPeriod(int? id)
        {

            var period = context.Periods.Find(id);
            return View("GetPeriod", period);
        }

        public IActionResult EditPeriod(Period c)
        {

            var period = context.Periods.Find(c.Id);
            period.Day = c.Day;
            period.StartHour = c.StartHour;
            period.EndHour = c.EndHour;


            if (!ModelState.IsValid)
            {
                return View("GetPeriod", period);
            }


            try
            {
              TimeOnly startTime = new TimeOnly(int.Parse(c.StartHour.Split(":")[0]), int.Parse(c.StartHour.Split(":")[1]));
              TimeOnly endTime = new TimeOnly(int.Parse(c.EndHour.Split(":")[0]), int.Parse(c.EndHour.Split(":")[1]));
              if (startTime > endTime)
              {
                    ModelState.AddModelError("StartHour", "Start hour of the period should be earlier than end hour.");
                    return View("GetPeriod", period);
              }
            
                period.Day = c.Day;
                period.StartHour = c.StartHour;
                period.EndHour = c.EndHour;
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception was caused by a unique constraint violation
                if (ex.InnerException is PostgresException pgex && pgex.SqlState == "23505")
                {
                    ModelState.AddModelError("Day", "There is already a period with this day, start and end time.");
                    ModelState.AddModelError("StartHour", "There is already a period with this day, start and end time.");
                    ModelState.AddModelError("EndHour", "There is already a period with this day, start and end time.");
                    return View("GetPeriod", period);
                }
                else
                {
                    throw; // Rethrow the exception if it wasn't caused by a unique constraint violation
                }

            }
            
            return RedirectToAction("Period");
        }
        public IActionResult Export()
        {

            List<PeriodViewModel> periods = this.context.Periods.Select(m => new PeriodViewModel
            {
                Id = m.Id,
                Day = m.Day,
                StartHour = m.StartHour,
                EndHour = m.EndHour

            }).ToList();

            MemoryStream stream = exportHelper.Export(genericList: periods);
            return File(stream.ToArray(), "text/plain", "Periods.csv");
        }
    }
}

