using KU.Student.Starter.UI.Models;
using Microsoft.AspNetCore.Mvc;


namespace KU.Student.Starter.UI.Controllers
{
    public class EditScheduleTextsController : Controller
    {
        private readonly Models.TutorDataContext context;
        private ExportHelper exportHelper = new ExportHelper();

        public EditScheduleTextsController(Models.TutorDataContext context)
        {
            this.context = context;

        }

        [HttpGet]
        public IActionResult EditScheduleTexts()
        {

            if (!context.EditScheduleTexts.Any())
            {
                return View("AddScheduleTexts");
            }

            EditScheduleTexts currentScheduleTexts = context.EditScheduleTexts.First();
            return View(currentScheduleTexts);
        }

        [HttpPost]
        public IActionResult EditScheduleTexts(EditScheduleTexts edt)
        {
            if (!ModelState.IsValid)
            {
                return View("EditScheduleTexts", edt);
            }

            EditScheduleTexts currentScheduleTexts = context.EditScheduleTexts.First();

            currentScheduleTexts.MainTitle = edt.MainTitle;
            currentScheduleTexts.SubTitle = edt.SubTitle;
            currentScheduleTexts.BelowTable = edt.BelowTable;
            currentScheduleTexts.PageTitle = edt.PageTitle;
            currentScheduleTexts.GoToTop = edt.GoToTop;
            currentScheduleTexts.NotPublishedText = edt.NotPublishedText;

            context.SaveChanges();

            return RedirectToAction("ScheduleTexts");
        }

        [HttpGet]
        public IActionResult AddScheduleTexts()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddScheduleTexts(EditScheduleTexts e)
        {
            context.EditScheduleTexts.Add(e);
            context.SaveChanges();
            return RedirectToAction("ScheduleTexts");
        }

        public IActionResult ScheduleTexts()
        {
            if (!context.EditScheduleTexts.Any())
            {
                return View(null);
            }

            EditScheduleTexts currentScheduleTexts = context.EditScheduleTexts.First();
            return View(currentScheduleTexts);
        }
    }
}

