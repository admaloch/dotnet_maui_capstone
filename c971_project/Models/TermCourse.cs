using SQLite;
using System;

namespace c971_project.Models
{


    public class TermCourse
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Single PK; can replace with composite PK if desired

        public int TermId { get; set; }      // FK to Term
        public int CourseId { get; set; }    // FK to Course

        public string Status { get; set; } = "In Progress"; // Default
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }


}
