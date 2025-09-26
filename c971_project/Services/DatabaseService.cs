

using SQLite;
using c971_project.Models;
using c971_project.Services;

using System.IO;
using System.Diagnostics;
using System.ComponentModel;


namespace c971_project.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _connection;
        private readonly Task _initializationTask;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
            _connection = new SQLiteAsyncConnection(dbPath);

            // Start initialization task
            _initializationTask = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // Create tables sequentially to avoid conflicts
                await _connection.CreateTableAsync<Student>();
                await _connection.CreateTableAsync<Term>();
                await _connection.CreateTableAsync<Instructor>();
                await _connection.CreateTableAsync<Course>();
                await _connection.CreateTableAsync<TermCourse>();
                await _connection.CreateTableAsync<Assessment>();
                await _connection.CreateTableAsync<Note>();

                // Initialize data after tables are created
                await InitializeDataAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization failed: {ex.Message}");
                throw; // Re-throw to make initialization task fail
            }
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                // Check if database already has data
                var termCount = await _connection.Table<Term>().CountAsync();

                if (termCount == 0)
                {
                    Debug.WriteLine("Database is empty - seeding sample data...");
                    await SeedDb.SeedDataAsync(_connection);
                    Debug.WriteLine("Sample data seeded successfully!");
                }
                else
                {
                    Debug.WriteLine($"Database already has {termCount} terms - skipping seed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error seeding data: {ex.Message}");
                // Don't throw here - we want the app to continue even if seeding fails
            }
        }

        // Public method to await initialization completion
        public Task EnsureInitialized() => _initializationTask;


        //STUDENT 
        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            await EnsureInitialized();
            return await _connection.Table<Student>()
                                    .Where(t => t.StudentId == studentId)
                                    .FirstOrDefaultAsync();
        }

        public async Task<Student> GetCurrentStudentAsync()
        {
            await EnsureInitialized();
            var students = await _connection.Table<Student>().ToListAsync();
            return students.FirstOrDefault(); // Returns null if no student
        }

        public async Task<int> SaveStudentAsync(Student student)
        {
            await EnsureInitialized();
            if (string.IsNullOrEmpty(student.StudentId))
                return await _connection.InsertAsync(student);
            else
                return await _connection.UpdateAsync(student);
        }

        //TERM 
        public async Task<List<Term>> GetTermsAsync()
        {
            await EnsureInitialized();
            return await _connection.Table<Term>().ToListAsync();
        }

        public async Task<Term> GetTermByIdAsync(int termId)
        {
            await EnsureInitialized();

            return await _connection.Table<Term>()
                                    .Where(t => t.TermId == termId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<int> SaveTermAsync(Term term)
        {
            await EnsureInitialized();
            if (term.TermId == 0)
                return await _connection.InsertAsync(term);
            else
                return await _connection.UpdateAsync(term);
        }

        public async Task<int> DeleteTermAsync(Term term)
        {
            await EnsureInitialized();
            return await _connection.DeleteAsync(term);
        }

        //COURSES 
        public async Task<List<Course>> GetCoursesByTermIdAsync(int termId)
        {
            await EnsureInitialized();
            return await _connection.Table<Course>()
                      .Where(c => c.TermId == termId)
                      .ToListAsync();
        }
        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            await EnsureInitialized();
            return await _connection.Table<Course>()
                                    .Where(t => t.CourseId == courseId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<Course> GetCourseByCourseNumAsync(string courseNum)
        {
            await EnsureInitialized();
            return await _connection.Table<Course>()
                      .Where(c => c.CourseNum == courseNum)
                      .FirstOrDefaultAsync();
        }
        public async Task<int> SaveCourseAsync(Course course)
        {
            await EnsureInitialized();
            if (course.CourseId == 0)
                return await _connection.InsertAsync(course);
            else
                return await _connection.UpdateAsync(course);
        }
        public async Task<int> DeleteCourseAsync(Course course)
        {
            await EnsureInitialized();
            return await _connection.DeleteAsync(course);
        }

        //INSTRUCTOR
        public async Task<Instructor> GetInstructorByIdAsync(int instructorId)
        {
            await EnsureInitialized();
            return await _connection.Table<Instructor>()
                                    .Where(t => t.InstructorId == instructorId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<Instructor> GetInstructorByEmailAsync(string email)
        {
            await EnsureInitialized();
            return await _connection.Table<Instructor>()
                      .Where(i => i.Email == email)
                      .FirstOrDefaultAsync();
        }
        public async Task<int> SaveInstructorAsync(Instructor instructor)
        {
            await EnsureInitialized();
            if (instructor.InstructorId == 0)
                return await _connection.InsertAsync(instructor);
            else
                return await _connection.UpdateAsync(instructor);
        }

        //ASSESSMENTS
        public async Task<List<Assessment>> GetAssessmentsByCourseIdAsync(int courseId)
        {
            await EnsureInitialized();
            return await _connection.Table<Assessment>()
                      .Where(c => c.CourseId == courseId)
                      .ToListAsync();
        }
        public async Task<Assessment> GetAssessmentByIdAsync(int assessmentId)
        {
            await EnsureInitialized();
            return await _connection.Table<Assessment>()
                                    .Where(t => t.AssessmentId == assessmentId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            await EnsureInitialized();
            if (assessment.AssessmentId == 0)
                return await _connection.InsertAsync(assessment);
            else
                return await _connection.UpdateAsync(assessment);
        }
        public async Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            await EnsureInitialized();
            return await _connection.DeleteAsync(assessment);
        }
        public async Task<int> DeleteAssessmentsByCourseIdAsync(int courseId)
        {
            await EnsureInitialized();
            return await _connection.ExecuteAsync(
                "DELETE FROM Assessment WHERE CourseId = ?",
                courseId);
        }

        //NOTES
        public async Task<List<Note>> GetNotesByCourseIdAsync(int courseId)
        {
            await EnsureInitialized();
            return await _connection.Table<Note>()
                      .Where(c => c.CourseId == courseId)
                      .ToListAsync();
        }
        public async Task<Note> GetNoteByIdAsync(int noteId)
        {
            await EnsureInitialized();
            return await _connection.Table<Note>()
                                    .Where(t => t.NoteId == noteId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<int> SaveNoteAsync(Note note)
        {
            await EnsureInitialized();
            if (note.NoteId == 0)
                return await _connection.InsertAsync(note);
            else
                return await _connection.UpdateAsync(note);
        }

        public async Task<int> DeleteNoteAsync(Note Note)
        {
            await EnsureInitialized();
            return await _connection.DeleteAsync(Note);
        }
        public async Task<int> DeleteNotesByCourseIdAsync(int courseId)
        {
            await EnsureInitialized();
            return await _connection.ExecuteAsync(
                "DELETE FROM Note WHERE CourseId = ?",
                courseId);
        }
    }
}
