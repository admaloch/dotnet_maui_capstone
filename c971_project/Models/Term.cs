using SQLite;
using System;

namespace c971_project.Models
{

    using CommunityToolkit.Mvvm.ComponentModel;
    using SQLite;
    using System;

    public partial class Term : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }

        [Indexed]
        public string StudentId { get; set; } = string.Empty;  // Foreign key to Student

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private int _termNum = 1;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;
    }



}
