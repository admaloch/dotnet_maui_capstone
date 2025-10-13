using Firebase.Database;
using Firebase.Database.Query;
using c971_project.Models;
using Newtonsoft.Json;

namespace c971_project.Services.Firebase
{
    public interface IFirestoreDataService
    {
        // STUDENT METHODS
        Task<Student> GetStudentAsync(string studentId);
        Task SaveStudentAsync(Student student);

        // TERM METHODS  
        Task<List<Term>> GetTermsByUserIdAsync(string userId);
        Task<Term> GetTermAsync(string termId);
        Task SaveTermAsync(Term term);
        Task DeleteTermAsync(string termId);

        // COURSE METHODS
        Task<List<Course>> GetCoursesByTermIdAsync(string termId);
        Task<Course> GetCourseAsync(string courseId);
        Task<Course> GetCourseByCourseNumAsync(string courseNum);
        Task SaveCourseAsync(Course course);
        Task DeleteCourseAsync(string courseId);

        // INSTRUCTOR METHODS
        Task<Instructor> GetInstructorAsync(string instructorId);
        Task<Instructor> GetInstructorByEmailAsync(string email);
        Task SaveInstructorAsync(Instructor instructor);

        // ASSESSMENT METHODS
        Task<List<Assessment>> GetAssessmentsByCourseIdAsync(string courseId);
        Task<Assessment> GetAssessmentAsync(string assessmentId);
        Task SaveAssessmentAsync(Assessment assessment);
        Task DeleteAssessmentAsync(string assessmentId);
        Task DeleteAssessmentsByCourseIdAsync(string courseId);

        // NOTE METHODS
        Task<List<Note>> GetNotesByCourseIdAsync(string courseId);
        Task<Note> GetNoteAsync(string noteId);
        Task SaveNoteAsync(Note note);
        Task DeleteNoteAsync(string noteId);
        Task DeleteNotesByCourseIdAsync(string courseId);
    }

    public class FirestoreDataService : IFirestoreDataService
    {
        private readonly FirebaseClient _firebaseClient;

        // Use your Realtime Database URL from Firebase Console
        private const string FirebaseUrl = "https://wgu-cloud-planner-default-rtdb.firebaseio.com/";

        public FirestoreDataService()
        {
            _firebaseClient = new FirebaseClient(FirebaseUrl);
        }

        // STUDENT METHODS
        public async Task<Student> GetStudentAsync(string studentId)
        {
            var result = await _firebaseClient
                .Child("students")
                .Child(studentId)
                .OnceSingleAsync<Student>();
            return result;
        }

        public async Task SaveStudentAsync(Student student)
        {
            await _firebaseClient
                .Child("students")
                .Child(student.Id)
                .PutAsync(student);
        }

        // TERM METHODS
        public async Task<List<Term>> GetTermsByUserIdAsync(string userId)
        {
            var terms = await _firebaseClient
                .Child("terms")
                .OrderBy("UserId")
                .EqualTo(userId)
                .OnceAsync<Term>();

            return terms.Select(item => item.Object).ToList();
        }

        public async Task<Term> GetTermAsync(string termId)
        {
            var term = await _firebaseClient
                .Child("terms")
                .Child(termId)
                .OnceSingleAsync<Term>();
            return term;
        }

        public async Task SaveTermAsync(Term term)
        {
            await _firebaseClient
                .Child("terms")
                .Child(term.Id)
                .PutAsync(term);
        }

        public async Task DeleteTermAsync(string termId)
        {
            await _firebaseClient
                .Child("terms")
                .Child(termId)
                .DeleteAsync();
        }

        // COURSE METHODS
        public async Task<List<Course>> GetCoursesByTermIdAsync(string termId)
        {
            var courses = await _firebaseClient
                .Child("courses")
                .OrderBy("TermId")
                .EqualTo(termId)
                .OnceAsync<Course>();

            return courses.Select(item => item.Object).ToList();
        }

        public async Task<Course> GetCourseAsync(string courseId)
        {
            var course = await _firebaseClient
                .Child("courses")
                .Child(courseId)
                .OnceSingleAsync<Course>();
            return course;
        }

        public async Task<Course> GetCourseByCourseNumAsync(string courseNum)
        {
            var courses = await _firebaseClient
                .Child("courses")
                .OrderBy("CourseNum")
                .EqualTo(courseNum)
                .OnceAsync<Course>();

            return courses.FirstOrDefault()?.Object;
        }

        public async Task SaveCourseAsync(Course course)
        {
            await _firebaseClient
                .Child("courses")
                .Child(course.Id)
                .PutAsync(course);
        }

        public async Task DeleteCourseAsync(string courseId)
        {
            await _firebaseClient
                .Child("courses")
                .Child(courseId)
                .DeleteAsync();
        }

        // INSTRUCTOR METHODS
        public async Task<Instructor> GetInstructorAsync(string instructorId)
        {
            var instructor = await _firebaseClient
                .Child("instructors")
                .Child(instructorId)
                .OnceSingleAsync<Instructor>();
            return instructor;
        }

        public async Task<Instructor> GetInstructorByEmailAsync(string email)
        {
            var instructors = await _firebaseClient
                .Child("instructors")
                .OrderBy("Email")
                .EqualTo(email)
                .OnceAsync<Instructor>();

            return instructors.FirstOrDefault()?.Object;
        }

        public async Task SaveInstructorAsync(Instructor instructor)
        {
            await _firebaseClient
                .Child("instructors")
                .Child(instructor.Id)
                .PutAsync(instructor);
        }

        // ASSESSMENT METHODS
        public async Task<List<Assessment>> GetAssessmentsByCourseIdAsync(string courseId)
        {
            var assessments = await _firebaseClient
                .Child("assessments")
                .OrderBy("CourseId")
                .EqualTo(courseId)
                .OnceAsync<Assessment>();

            return assessments.Select(item => item.Object).ToList();
        }

        public async Task<Assessment> GetAssessmentAsync(string assessmentId)
        {
            var assessment = await _firebaseClient
                .Child("assessments")
                .Child(assessmentId)
                .OnceSingleAsync<Assessment>();
            return assessment;
        }

        public async Task SaveAssessmentAsync(Assessment assessment)
        {
            await _firebaseClient
                .Child("assessments")
                .Child(assessment.Id)
                .PutAsync(assessment);
        }

        public async Task DeleteAssessmentAsync(string assessmentId)
        {
            await _firebaseClient
                .Child("assessments")
                .Child(assessmentId)
                .DeleteAsync();
        }

        public async Task DeleteAssessmentsByCourseIdAsync(string courseId)
        {
            var assessments = await GetAssessmentsByCourseIdAsync(courseId);
            foreach (var assessment in assessments)
            {
                await DeleteAssessmentAsync(assessment.Id);
            }
        }

        // NOTE METHODS
        public async Task<List<Note>> GetNotesByCourseIdAsync(string courseId)
        {
            var notes = await _firebaseClient
                .Child("notes")
                .OrderBy("CourseId")
                .EqualTo(courseId)
                .OnceAsync<Note>();

            return notes.Select(item => item.Object).ToList();
        }

        public async Task<Note> GetNoteAsync(string noteId)
        {
            var note = await _firebaseClient
                .Child("notes")
                .Child(noteId)
                .OnceSingleAsync<Note>();
            return note;
        }

        public async Task SaveNoteAsync(Note note)
        {
            await _firebaseClient
                .Child("notes")
                .Child(note.Id)
                .PutAsync(note);
        }

        public async Task DeleteNoteAsync(string noteId)
        {
            await _firebaseClient
                .Child("notes")
                .Child(noteId)
                .DeleteAsync();
        }

        public async Task DeleteNotesByCourseIdAsync(string courseId)
        {
            var notes = await GetNotesByCourseIdAsync(courseId);
            foreach (var note in notes)
            {
                await DeleteNoteAsync(note.Id);
            }
        }
    }
}