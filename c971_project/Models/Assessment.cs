using SQLite;
using System;

namespace c971_project.Models
{


    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public int CourseId { get; set; }    // FK to Course
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Objective"; // Objective or Performance
        public string Status { get; set; } = "Planned"; // Default
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }


}
