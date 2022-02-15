using System;

namespace CoreProject.Models
{
    public class StudentCourseTimeModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string ClassName { get; set; }

        public StudentCourseTimeModel(DateTime startdate, DateTime enddate, string classname)
        {
            StartDate = startdate;
            EndDate = enddate;
            ClassName = classname;
            DayOfWeek = startdate.DayOfWeek;
        }

        //傳入當前日期，判斷範圍時段內內，同樣星期時間與否有課
        public bool Check(DateTime date)
        {

            if (date.DayOfWeek != DayOfWeek)
                return false;
            if (date >= StartDate && date <= EndDate)
                return true;
            return false;
        }
    }
}
