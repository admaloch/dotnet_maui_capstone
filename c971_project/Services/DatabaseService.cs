using SQLite;
using c971_project.Models;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;


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
                Console.WriteLine($"Database path: {dbPath}");

            }
        }

    
        private void SeedData()
        {
            // 1. Student
            var student = new Student
            {
                Name = "Brock Johnson",
                StudentId = "03829483938",
                Email = "brockjohnson03@fakeemail.com",
                Status = "Currently Enrolled",
                Major = "Computer Science"
            };
            _connection.Insert(student);
            Debug.WriteLine($"Inserted student: {student.Name}, ID: {student.StudentId}");

            // 2. Terms
            var term1 = new Term
            {
                StudentId = student.StudentId,
                Name = "Spring 2024",
                TermNum = 1,
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now
            };
            _connection.Insert(term1);
            Debug.WriteLine($"Inserted term: {term1.Name}");

            // 3. Instructors
            var instructor1 = new Instructor{
                Name = "Anika Patel",
                Email = "anika.patel@strimeuniversity.edu",
                Phone = "555-123-4567"  // Note the format: 555-123-4567
            };
            _connection.Insert(instructor1);
            Debug.WriteLine($"Inserted instructor: {instructor1.Name}");

            // 4. Courses
            var course1 = new Course
            {
                Name = "Intro to Programming",
                CourseNum = "CS101",
                CuNum = 3,
                InstructorId = instructor1.InstructorId
            }; 
            _connection.Insert(course1);
            Debug.WriteLine($"Inserted courses: {course1.CourseId}");

            // 5. TermCourse associations
            var termCourse1 = new TermCourse
            {
                TermId = term1.TermId,
                CourseId = course1.CourseId,
                Status = "Currently Enrolled",
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddMonths(2)
            }; 
            _connection.Insert(termCourse1);
            Debug.WriteLine($"Inserted term-course associations: {termCourse1.Id}");

            // 6. Assessments
            var assess1 = new Assessment
            {
                TermId = term1.TermId,
                CourseId = course1.CourseId,
                StudentId = student.StudentId,
                Name = "Programming Exam 1",
                Type = "Objective",
                Status = "Preparing",
                StartDate = term1.StartDate.AddDays(60),
                EndDate = term1.StartDate.AddDays(60)
            };
            var assess2 = new Assessment
            {
                TermId = term1.TermId,
                CourseId = course1.CourseId,
                StudentId = student.StudentId,
                Name = "Demonstrate Programming Fundamentals",
                Type = "Practical",
                Status = "Preparing",
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddDays(30)
            };
            _connection.Insert(assess1);
            _connection.Insert(assess2);

            Debug.WriteLine($"Inserted assessments: {assess1.AssessmentId} -- {assess2.AssessmentId}");

            // 7. Notes
            var note1 = new Note
            {
                AssessmentId = assess1.AssessmentId,
                Title = "Exam Tip",
                Body = "Review chapters 1-3 thoroughly."
            };
            var note2 = new Note
            {
                AssessmentId = assess2.AssessmentId,
                Title = "Assessment Tip",
                Body = "The assessment requires demonstrating core programming concepts: 1) Data types and variables 2) Operators and expressions 3) Control structures (conditionals and loops) 4) Basic input/output 5) Problem-solving approach. Prepare by writing small programs that showcase each concept. Example: a program that takes user input, processes it using loops and conditionals, and produces formatted output. Focus on clean code and proper syntax."
            };
            _connection.Insert(note1);
            _connection.Insert(note2);

            Debug.WriteLine($"Inserted notes: {note1.NoteId} -- {note2.NoteId}");
        }
        private void DeleteDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Debug.WriteLine("Database deleted.");
            }
            else
            {
                Debug.WriteLine("No database file found to delete.");
            }
        }
        public async Task<List<Student>> GetStudentsAsync()
        {
            return await Task.Run(() => _connection.Table<Student>().ToList());
        }

        public async Task<List<Term>> GetTermsByStudentIdAsync(string studentId)
        {
            return await Task.Run(() =>
                _connection.Table<Term>()
                          .Where(t => t.StudentId == studentId)
                          .ToList()
            );
        }

        public async Task<List<Course>> GetCoursesAsync()
        {
            return await Task.Run(() => _connection.Table<Course>().ToList());
        }
        // Keep your synchronous methods too for now
        public List<Student> GetStudents()
        {
            return _connection.Table<Student>().ToList();
        }

        public async Task<int> InsertStudentAsync(Student student)
        {
            return await Task.Run(() => _connection.Insert(student));
        }

        public async Task<int> UpdateStudentAsync(Student student)
        {
            return await Task.Run(() => _connection.Update(student));
        }

        public SQLiteConnection Connection => _connection;
    }
}
