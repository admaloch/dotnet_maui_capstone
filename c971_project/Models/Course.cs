using SQLite;
using System;

namespace c971_project.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CourseNum { get; set; } = string.Empty;
        public int CuNum { get; set; } = 3; // default credit units
        [Indexed]
        public int InstructorId { get; set; } // Foreign key to Instructor
    }
}
