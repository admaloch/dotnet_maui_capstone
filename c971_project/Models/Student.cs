using SQLite;
using System;

namespace c971_project.Models
{
    public class Student
    {
        [PrimaryKey] // No AutoIncrement for strings
        public string StudentId { get; set; } = string.Empty; // Cha ged to string! 
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Currently Enrolled";
        public string Major { get; set; } = string.Empty;
    }


}
