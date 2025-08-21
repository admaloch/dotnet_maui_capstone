using SQLite;
using System;

namespace c971_project.Models
{
    public class Student
    {
        [PrimaryKey, AutoIncrement]
        public int StudentId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "Currently Enrolled"; // Default
        public string Major { get; set; } = string.Empty;
    }


}
