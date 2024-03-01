using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using String = System.String;


namespace KU.Student.Starter.UI.Models
{
    public class CourseDataSource
    {
        public static String data = string.Empty;
        static List<TempCourse> courses = new();
        private static string term = string.Empty;

        public static void GetCourses(string term)
        {
            RunAsync(term: term).Wait();
        }

        static async Task RunAsync(string term)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("http://open-api.ku.edu.tr/course/vpaa/" + term);
                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();

                    if (data == null)
                    {
                        Console.WriteLine("No data!");
                    }
                }
            }
        }
        public static List<TempCourse> GetCourseList()
        {

            var jarray = JArray.Parse(data);
            foreach (var item in jarray)
            {
                TempCourse course = new TempCourse(subject: item["subject"]?.ToString() ?? "Nan",
                                           catalogNumber: item["catalogNbr"]?.ToString() ?? "Nan",
                                           instructorFirstName: item["firstName"]?.ToString() ?? "Nan",
                                           instructorLastName: item["lastName"]?.ToString() ?? "Nan",
                                           meetingDays: item["meetingDays"]?.ToString() ?? "Nan",
                                           meetingTimeStart: item["meetingTimeStart"]?.ToString() ?? "Nan",
                                           meetingTimeEnd: item["meetingTimeEnd"]?.ToString() ?? "Nan",
                                           courseTitle: item["courseTitle"]?.ToString() ?? "Nan");
                courses.Add(course);
            }
            return courses;
        }
    }
}

