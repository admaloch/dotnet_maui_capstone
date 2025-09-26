using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;


namespace c971_project.Models
{


    public class Note : ObservableValidator
    {
        [PrimaryKey, AutoIncrement]
        public int NoteId { get; set; }

        public int CourseId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required.")]
        [MaxLength(5000, ErrorMessage = "Note body cannot exceed 5000 characters.")]
        public string Body { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}


