using SQLite;
using System;

namespace c971_project.Models
{

    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }

        [Indexed]
        public int StudentId { get; set; }  // Foreign key to Student
        public string Name { get; set; } = string.Empty;
        public int TermNum { get; set; } = 1; // Default term number
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


}
