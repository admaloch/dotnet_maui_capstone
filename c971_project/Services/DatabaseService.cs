using SQLite;
using c971_project.Models;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;


namespace c971_project.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _connection;

        private DatabaseService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        // Async factory method
        public static async Task<DatabaseService> CreateAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            var connection = new SQLiteAsyncConnection(dbPath);

            // Create tables asynchronously
            await connection.CreateTableAsync<Student>();
            await connection.CreateTableAsync<Term>();
            await connection.CreateTableAsync<Instructor>();
            await connection.CreateTableAsync<Course>();
            await connection.CreateTableAsync<TermCourse>();
            await connection.CreateTableAsync<Assessment>();
            await connection.CreateTableAsync<Note>();

            // Seed the database
            DeleteDatabase();
            SeedData();
            Debug.WriteLine("Database initialized and seeded (sync).");
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

        private void SeedData()
        {
            // Check if already seeded
            var existingStudent = _connection.Table<Student>().FirstOrDefault();
            if (existingStudent != null)
            {
                Debug.WriteLine("Students found!!");
                return; // Already seeded
            }

            Debug.WriteLine("No students found. Seeding database...");

            var student = new Student
            {
                Name = "Brock Johnson",
                StudentId = "03829483938",
                Email = "brockjohnson03@fakeemail.com",
                Status = "Currently Enrolled",
                Major = "Computer Science"
            };
            _connection.Insert(student);
            Debug.WriteLine($"Inserted student: {student.Name}");

            var term1 = new Term
            {
                Name = "Spring 2024",
                TermNum = 1,
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now
            };
            _connection.Insert(term1);

            var instructor1 = new Instructor
            {
                Name = "Anika Patel",
                Email = "anika.patel@strimeuniversity.edu",
                Phone = "555-123-4567"
            };
            _connection.Insert(instructor1);

            var course1 = new Course
            {
                Name = "Intro to Programming",
                CourseNum = "CS101",
                CuNum = 3,
                InstructorId = instructor1.InstructorId,
                TermId = term1.TermId,
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddMonths(2),
                DateAdded = DateTime.Now
            };
            _connection.Insert(course1);

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
                Body = "The assessment requires demonstrating core programming concepts..."
            };
            _connection.Insert(note1);
            _connection.Insert(note2);

            Debug.WriteLine("Database seeding complete (sync).");
        }
        public async Task<Student> GetCurrentStudentAsync()
        {
            var students = await _connection.Table<Student>().ToListAsync();
            return students.FirstOrDefault(); // Returns null if no student
        }

        // Save or update student
        public async Task<int> SaveStudentAsync(Student student)
        {
            if (string.IsNullOrEmpty(student.StudentId))
                return await _connection.InsertAsync(student);
            else
                return await _connection.UpdateAsync(student);
        }

        public async Task<int> SaveTermAsync(Term term)
        {
            if (term.TermId == 0)
                return await _connection.InsertAsync(term);
            else
                return await _connection.UpdateAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            return await _connection.DeleteAsync(term);
        }


        // Example: Get all terms
        public async Task<List<Term>> GetTermsAsync()
        {
            return await _connection.Table<Term>().ToListAsync();
        }

        public async Task<Term> GetTermByIdAsync(int termId)
        {
            if (_connection == null)
                throw new InvalidOperationException("Database connection is not initialized.");

            return await _connection.Table<Term>()
                                    .Where(t => t.TermId == termId)
                                    .FirstOrDefaultAsync();
        }
        // Example: Get all courses
        public Task<List<Course>> GetCoursesByTermIdAsync(int termId)
        {
            return _connection.Table<Course>()
                      .Where(c => c.TermId == termId)
                      .ToListAsync();
        }

        public Task<Course> GetCourseByCourseNumAsync(string courseNum)
        {
            return _connection.Table<Course>()
                      .Where(c => c.CourseNum == courseNum)
                      .FirstOrDefaultAsync();
        }

        public async Task<int> SaveCourseAsync(Course course)
        {
            if (course.CourseId == 0)
                return await _connection.InsertAsync(course);
            else
                return await _connection.UpdateAsync(course);
        }

        public async Task<int> SaveInstructorAsync(Instructor instructor)
        {
            if (instructor.InstructorId == 0)
                return await _connection.InsertAsync(instructor);
            else
                return await _connection.UpdateAsync(instructor);
        }

        public Task<Instructor> GetInstructorByEmailAsync(string email)
        {
            return _connection.Table<Instructor>()
                      .Where(i => i.Email == email)
                      .FirstOrDefaultAsync();
        }

    }
}
