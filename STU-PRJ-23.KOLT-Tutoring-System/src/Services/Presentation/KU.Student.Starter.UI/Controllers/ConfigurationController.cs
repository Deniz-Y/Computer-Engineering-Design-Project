using KU.Student.Starter.UI.Models;
using KU.Student.Starter.UI.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace KU.Student.Starter.UI.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly TutorDataContext context;
        private ExportHelper exportHelper = new ExportHelper();

        public ConfigurationController(TutorDataContext context)
        {
            this.context = context;

        }

        public IActionResult Configuration()
        {
            Configuration currenConfiguration;
            
            if (!context.Configurations.Any())
            {
                return View(null);
            }

            currenConfiguration = context.Configurations.OrderBy(c => c.Id).Last();
            return View(currenConfiguration);
        }

        public IActionResult AddConfiguration(Configuration c)
        {
            if(User.Identity != null)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
                c.UserName = claimsIdentity.Claims.FirstOrDefault((Claim f) => f.Type == "name").Value;
            }
            c.EditDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH: mm");

            if (!ModelState.IsValid)
            {
                return View("Configuration",c);
            }

            context.Configurations.Add(c);
            context.SaveChanges();
            return RedirectToAction("Configuration");
        }

        public IActionResult ConfigurationHistory(string searchBy, string searchString)
        {
            var configuraions = this.context.Configurations.ToList().Select(c => new ConfigurationViewModel
            {
                Id = c.Id,
                PublishSchedule = c.PublishSchedule,
                Date = c.Date,
                Time = c.Time,
                NumberOfCubicles = c.NumberOfCubicles,
                ScheduleSplitCount = c.ScheduleSplitCount,
                UserName = c.UserName,
                EditDateTime = c.EditDateTime
            });

            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchString))
            {
                if (searchBy.Equals("UserName"))
                {
                    configuraions = configuraions.Where(c => c.UserName!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();

                }
                else if (searchBy.Equals("EditDateTime"))
                {
                    configuraions = configuraions.Where(c => c.EditDateTime!.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            return View(configuraions);
        }

        public IActionResult ViewConfiguration(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuration = context.Configurations.FirstOrDefault(m => m.Id == id);
            if (configuration == null)
            {
                return NotFound();
            }
            return View(configuration);
        }

        public IActionResult EditConfiguration()
        {
            Configuration currenConfiguration;

            if (context.Configurations.Any())
            {
                currenConfiguration = context.Configurations.OrderBy(c => c.Id).Last();
            }
            else
            {
                currenConfiguration = new Configuration();
            }
            return View(currenConfiguration);
        }

        public IActionResult Export()
        {

            List<ConfigurationViewModel> configuraions = this.context.Configurations.Select(c => new ConfigurationViewModel
            {
                Id = c.Id,
                PublishSchedule = c.PublishSchedule,
                Date = c.Date,
                Time = c.Time,
                NumberOfCubicles = c.NumberOfCubicles,
                ScheduleSplitCount = c.ScheduleSplitCount,
                UserName = c.UserName,
                EditDateTime = c.EditDateTime.Replace(',',' ')
            }).ToList();


            MemoryStream stream = exportHelper.Export(genericList: configuraions);
            return File(stream.ToArray(), "text/plain", "ConfigurationsHistory.csv");
        }
    }
}

