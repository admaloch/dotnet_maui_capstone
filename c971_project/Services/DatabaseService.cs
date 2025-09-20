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

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

            // Use async connection
            _connection = new SQLiteAsyncConnection(dbPath);

            // Create tables (async, but safe to Wait() here since it's in constructor)
            _connection.CreateTableAsync<Student>().Wait();
            _connection.CreateTableAsync<Term>().Wait();
            _connection.CreateTableAsync<Instructor>().Wait();
            _connection.CreateTableAsync<Course>().Wait();
            _connection.CreateTableAsync<TermCourse>().Wait();
            _connection.CreateTableAsync<Assessment>().Wait();
            _connection.CreateTableAsync<Note>().Wait();


            //DeleteDatabase();

            //SeedDataAsync();

        }


        private async void SeedDataAsync()
        {


            var existingStudent = await _connection.Table<Student>().FirstOrDefaultAsync();
            if (existingStudent != null)
            {
                Debug.WriteLine("Students found!!");
                return; // Already seeded
            }


            Debug.WriteLine("No students found. Seeding database...");
            // 1. Student
            var student = new Student
            {
                Name = "Brock Johnson",
                StudentId = "03829483938",
                Email = "brockjohnson03@fakeemail.com",
                Status = "Currently Enrolled",
                Major = "Computer Science"
            };

            await _connection.InsertAsync(student);
            Debug.WriteLine($"Inserted student: {student.Name}, ID: {student.StudentId}");

            // 2. Terms
            var term1 = new Term
            {
                Name = "Spring 2024",
                TermNum = 1,
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now
            };
            await _connection.InsertAsync(term1);
            Debug.WriteLine($"Inserted term: {term1.Name}");

            // 3. Instructors
            var instructor1 = new Instructor
            {
                Name = "Anika Patel",
                Email = "anika.patel@strimeuniversity.edu",
                Phone = "555-123-4567"  // Note the format: 555-123-4567
            };
            await _connection.InsertAsync(instructor1);
            Debug.WriteLine($"Inserted instructor: {instructor1.Name}");

            // 4. Courses
            var course1 = new Course
            {
                Name = "Intro to Programming",
                CourseNum = "CS101",
                CuNum = 3,
                InstructorId = instructor1.InstructorId,
                TermId = term1.TermId,
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddMonths(2)
            };
            await _connection.InsertAsync(course1);
            Debug.WriteLine($"Inserted courses: {course1.CourseId}");

           

            // 6. Assessments
            var assess1 = new Assessment
            {
                CourseId = course1.CourseId,
                Name = "Programming Exam 1",
                Type = "Objective",
                Status = "Preparing",
                StartDate = term1.StartDate.AddDays(60),
                EndDate = term1.StartDate.AddDays(60)
            };
            var assess2 = new Assessment
            {
                CourseId = course1.CourseId,
                Name = "Demonstrate Programming Fundamentals",
                Type = "Practical",
                Status = "Preparing",
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddDays(30)
            };
            await _connection.InsertAsync(assess1);
            await _connection.InsertAsync(assess2);

            Debug.WriteLine($"Inserted assessments: {assess1.AssessmentId} -- {assess2.AssessmentId}");

            // 7. Notes
            var note1 = new Note
            {
                CourseId = course1.CourseId,
                Title = "Exam Tip",
                Body = "Review chapters 1-3 thoroughly."
            };
            var note2 = new Note
            {
                CourseId = course1.CourseId,
                Title = "Assessment Tip",
                Body = "The assessment requires demonstrating core programming concepts: 1) Data types and variables 2) Operators and expressions 3) Control structures (conditionals and loops) 4) Basic input/output 5) Problem-solving approach. Prepare by writing small programs that showcase each concept. Example: a program that takes user input, processes it using loops and conditionals, and produces formatted output. Focus on clean code and proper syntax."
            };
            await _connection.InsertAsync(note1);
            await _connection.InsertAsync(note2);

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
        public async Task<Student> GetCurrentStudentAsync()
        {
            var students = await _connection.Table<Student>().ToListAsync();
            return students.FirstOrDefault(); // Returns null if no student
        }

        public async Task<Student> GetStudentByIdAsync(string studentId)
        {

            return await _connection.Table<Student>()
                                    .Where(t => t.StudentId == studentId)
                                    .FirstOrDefaultAsync();
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

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _connection.Table<Course>()
                                    .Where(t => t.CourseId == courseId)
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
        public async Task<Instructor> GetInstructorByIdAsync(int instructorId)
        {
            return await _connection.Table<Instructor>()
                                    .Where(t => t.InstructorId == instructorId)
                                    .FirstOrDefaultAsync();
        }

        public async Task<int> DeleteCourseAsync(Course course)
        {
            return await _connection.DeleteAsync(course);
        }

        public Task<List<Assessment>> GetAssessmentsByCourseIdAsync(int courseId)
        {
            return _connection.Table<Assessment>()
                      .Where(c => c.CourseId == courseId)
                      .ToListAsync();
        }
        public async Task<Assessment> GetAssessmentByIdAsync(int assessmentId)
        {
            return await _connection.Table<Assessment>()
                                    .Where(t => t.AssessmentId == assessmentId)
                                    .FirstOrDefaultAsync();
        }

        public Task<List<Note>> GetNotesByCourseIdAsync(int courseId)
        {
            return _connection.Table<Note>()
                      .Where(c => c.CourseId == courseId)
                      .ToListAsync();
        }

        public async Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return await _connection.DeleteAsync(assessment);
        }

        public async Task<int> DeleteNoteAsync(Note Note)
        {
            return await _connection.DeleteAsync(Note);
        }
        public async Task<Note> GetNoteByIdAsync(int noteId)
        {
            return await _connection.Table<Note>()
                                    .Where(t => t.NoteId == noteId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.AssessmentId == 0)
                return await _connection.InsertAsync(assessment);
            else
                return await _connection.UpdateAsync(assessment);
        }

        public async Task<int> SaveNoteAsync(Note note)
        {
            if (note.NoteId == 0)
                return await _connection.InsertAsync(note);
            else
                return await _connection.UpdateAsync(note);
        }

    }
}
