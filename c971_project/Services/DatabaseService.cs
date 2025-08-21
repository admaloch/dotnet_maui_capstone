using SQLite;
using c971_project.Models;
using System.IO;
using System.Diagnostics;


namespace c971_project.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _connection;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _connection = new SQLiteConnection(dbPath);

            // Create tables
            _connection.CreateTable<Student>();
            _connection.CreateTable<Term>();
            _connection.CreateTable<Instructor>();
            _connection.CreateTable<Course>();
            _connection.CreateTable<TermCourse>();
            _connection.CreateTable<Assessment>();
            _connection.CreateTable<Note>();

            if (!_connection.Table<Student>().Any())
            {
                Debug.WriteLine("No students found. Seeding database...");
                SeedData();
            }
            else
            {
                Debug.WriteLine("Students already exist. Skipping seeding.");
            }
        }

        private void SeedData()
        {
            // 1. Student
            var student = new Student
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Status = "Currently Enrolled",
                Major = "Computer Science"
            };
            _connection.Insert(student);
            Debug.WriteLine($"Inserted student: {student.Name}, ID: {student.StudentId}");

            // 2. Terms
            var term1 = new Term { StudentId = student.StudentId, TermNum = 1, StartDate = DateTime.Now.AddMonths(-6), EndDate = DateTime.Now };
            var term2 = new Term { StudentId = student.StudentId, TermNum = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(6) };
            _connection.Insert(term1);
            _connection.Insert(term2);
            Debug.WriteLine($"Inserted terms: {term1.TermId}, {term2.TermId}");

            // 3. Instructors
            var instructor1 = new Instructor { Name = "Dr. Smith", Email = "smith@university.edu", Phone = "555-1234" };
            var instructor2 = new Instructor { Name = "Prof. Johnson", Email = "johnson@university.edu", Phone = "555-5678" };
            _connection.Insert(instructor1);
            _connection.Insert(instructor2);
            Debug.WriteLine($"Inserted instructors: {instructor1.InstructorId}, {instructor2.InstructorId}");

            // 4. Courses
            var course1 = new Course { Name = "Intro to Programming", CourseNum = "CS101", CuNum = 3, InstructorId = instructor1.InstructorId };
            var course2 = new Course { Name = "Data Structures", CourseNum = "CS102", CuNum = 3, InstructorId = instructor2.InstructorId };
            _connection.Insert(course1);
            _connection.Insert(course2);
            Debug.WriteLine($"Inserted courses: {course1.CourseId}, {course2.CourseId}");

            // 5. TermCourse associations
            var tc1 = new TermCourse { TermId = term1.TermId, CourseId = course1.CourseId, Status = "Completed", StartDate = term1.StartDate, EndDate = term1.EndDate };
            var tc2 = new TermCourse { TermId = term2.TermId, CourseId = course2.CourseId, Status = "In Progress", StartDate = term2.StartDate, EndDate = term2.EndDate };
            _connection.Insert(tc1);
            _connection.Insert(tc2);
            Debug.WriteLine($"Inserted term-course associations: {tc1.Id}, {tc2.Id}");

            // 6. Assessments
            var assess1 = new Assessment
            {
                TermId = term1.TermId,
                CourseId = course1.CourseId,
                StudentId = student.StudentId,
                Name = "Programming Exam 1",
                Type = "Objective",
                Status = "Completed",
                StartDate = term1.StartDate.AddDays(30),
                EndDate = term1.StartDate.AddDays(30)
            };
            _connection.Insert(assess1);
            Debug.WriteLine($"Inserted assessment: {assess1.AssessmentId}");

            // 7. Notes
            var note1 = new Note
            {
                AssessmentId = assess1.AssessmentId,
                Title = "Exam Tip",
                Body = "Review chapters 1-3 thoroughly."
            };
            _connection.Insert(note1);
            Debug.WriteLine($"Inserted note: {note1.NoteId}");
        }
        public List<Student> GetStudents()
        {
            return _connection.Table<Student>().ToList();
        }

        public SQLiteConnection Connection => _connection;
    }
}
