using System;
namespace KU.Student.Starter.UI.Models
{
    public class TempCourse
    {
        public String Subject { get; set; } = string.Empty;
        public String CatalogNumber { get; set; } = string.Empty;
        public String InstructorFirstName { get; set; } = string.Empty;
        public String InstructorLastName { get; set; } = string.Empty;
        public String MeetingDays { get; set; } = string.Empty;
        public String MeetingTimeStart { get; set; } = string.Empty;
        public String MeetingTimeEnd { get; set; } = string.Empty;
        public String CourseTitle { get; set; } = string.Empty;




        public TempCourse(String subject, String catalogNumber, String instructorFirstName, String instructorLastName, String meetingDays, String meetingTimeStart, String meetingTimeEnd, String courseTitle)
        {
            this.Subject = subject;
            this.CatalogNumber = catalogNumber;
            this.InstructorFirstName = instructorFirstName;
            this.InstructorLastName = instructorLastName;
            this.MeetingDays = meetingDays;
            this.MeetingTimeStart = meetingTimeStart;
            this.MeetingTimeEnd = meetingTimeEnd;
            this.CourseTitle = courseTitle;
        }
    }
}

