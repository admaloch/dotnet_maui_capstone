

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

        private String dbLocation = "app.db";

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, dbLocation);

            // Use async connection
            _connection = new SQLiteAsyncConnection(dbPath);

            //DeleteDb.DeleteDatabaseAsync(dbLocation);

            // Create tables (async, but safe to Wait() here since it's in constructor)
            _connection.CreateTableAsync<Student>().Wait();
            _connection.CreateTableAsync<Term>().Wait();
            _connection.CreateTableAsync<Instructor>().Wait();
            _connection.CreateTableAsync<Course>().Wait();
            _connection.CreateTableAsync<TermCourse>().Wait();
            _connection.CreateTableAsync<Assessment>().Wait();
            _connection.CreateTableAsync<Note>().Wait();


            //SeedDb.SeedDataAsync(_connection);

        }


        //STUDENT 
        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            return await _connection.Table<Student>()
                                    .Where(t => t.StudentId == studentId)
                                    .FirstOrDefaultAsync();
        }

        public async Task<Student> GetCurrentStudentAsync()
        {
            var students = await _connection.Table<Student>().ToListAsync();
            return students.FirstOrDefault(); // Returns null if no student
        }

        public async Task<int> SaveStudentAsync(Student student)
        {
            if (string.IsNullOrEmpty(student.StudentId))
                return await _connection.InsertAsync(student);
            else
                return await _connection.UpdateAsync(student);
        }

        //TERM 
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

        //COURSES 
        public Task<List<Course>> GetCoursesByTermIdAsync(int termId)
        {
            return _connection.Table<Course>()
                      .Where(c => c.TermId == termId)
                      .ToListAsync();
        }
        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _connection.Table<Course>()
                                    .Where(t => t.CourseId == courseId)
                                    .FirstOrDefaultAsync();
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
        public async Task<int> DeleteCourseAsync(Course course)
        {
            return await _connection.DeleteAsync(course);
        }

        //INSTRUCTOR
        public async Task<Instructor> GetInstructorByIdAsync(int instructorId)
        {
            return await _connection.Table<Instructor>()
                                    .Where(t => t.InstructorId == instructorId)
                                    .FirstOrDefaultAsync();
        }
        public Task<Instructor> GetInstructorByEmailAsync(string email)
        {
            return _connection.Table<Instructor>()
                      .Where(i => i.Email == email)
                      .FirstOrDefaultAsync();
        }
        public async Task<int> SaveInstructorAsync(Instructor instructor)
        {
            if (instructor.InstructorId == 0)
                return await _connection.InsertAsync(instructor);
            else
                return await _connection.UpdateAsync(instructor);
        }

        //ASSESSMENTS
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
        public async Task<int> SaveAssessmentAsync(Assessment assessment)
        {
            if (assessment.AssessmentId == 0)
                return await _connection.InsertAsync(assessment);
            else
                return await _connection.UpdateAsync(assessment);
        }
        public async Task<int> DeleteAssessmentAsync(Assessment assessment)
        {
            return await _connection.DeleteAsync(assessment);
        }
        public async Task<int> DeleteAssessmentsByCourseIdAsync(int courseId)
        {
            return await _connection.ExecuteAsync(
                "DELETE FROM Assessment WHERE CourseId = ?",
                courseId);
        }

        //NOTES
        public Task<List<Note>> GetNotesByCourseIdAsync(int courseId)
        {
            return _connection.Table<Note>()
                      .Where(c => c.CourseId == courseId)
                      .ToListAsync();
        }
        public async Task<Note> GetNoteByIdAsync(int noteId)
        {
            return await _connection.Table<Note>()
                                    .Where(t => t.NoteId == noteId)
                                    .FirstOrDefaultAsync();
        }
        public async Task<int> SaveNoteAsync(Note note)
        {
            if (note.NoteId == 0)
                return await _connection.InsertAsync(note);
            else
                return await _connection.UpdateAsync(note);
        }

        public async Task<int> DeleteNoteAsync(Note Note)
        {
            return await _connection.DeleteAsync(Note);
        }
        public async Task<int> DeleteNotesByCourseIdAsync(int courseId)
        {
            return await _connection.ExecuteAsync(
                "DELETE FROM Note WHERE CourseId = ?",
                courseId);
        }
    }
}
