using SQLite;
using System;

namespace c971_project.Models
{


    public class Instructor
    {
        [PrimaryKey, AutoIncrement]
        public int InstructorId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }


}
