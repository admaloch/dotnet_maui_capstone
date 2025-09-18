using SQLite;
using System;

namespace c971_project.Models
{


    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int NoteId { get; set; }
        public int CourseId { get; set; } // nullable for course-level notes
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
