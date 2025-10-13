using Google.Cloud.Firestore;
using c971_project.Models;

namespace c971_project.Services.Firebase
{
    public interface IFirestoreDataService
    {
        // STUDENT METHODS
        Task<Student> GetStudentAsync(string studentId);
        Task SaveStudentAsync(Student student);

        // TERM METHODS  
        Task<List<Term>> GetTermsAsync(string userId);
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
        private readonly FirestoreDb _firestoreDb;
        private const string ProjectId = "wgu-cloud-planner"; // Your firebase Project ID from console

        public FirestoreDataService()
        {
            _firestoreDb = FirestoreDb.Create(ProjectId);
        }

        // STUDENT METHODS
        public async Task<Student> GetStudentAsync(string studentId)
        {
            var snapshot = await _firestoreDb.Collection("students").Document(studentId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Student>() : null;
        }

        public async Task SaveStudentAsync(Student student)
        {
            await _firestoreDb.Collection("students").Document(student.Id).SetAsync(student);
        }

        // TERM METHODS
        public async Task<List<Term>> GetTermsAsync(string userId)
        {
            var query = _firestoreDb.Collection("terms").WhereEqualTo("UserId", userId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Term>()).ToList();
        }

        public async Task<Term> GetTermAsync(string termId)
        {
            var snapshot = await _firestoreDb.Collection("terms").Document(termId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Term>() : null;
        }

        public async Task SaveTermAsync(Term term)
        {
            await _firestoreDb.Collection("terms").Document(term.Id).SetAsync(term);
        }

        public async Task DeleteTermAsync(string termId)
        {
            await _firestoreDb.Collection("terms").Document(termId).DeleteAsync();
        }

        // COURSE METHODS
        public async Task<List<Course>> GetCoursesByTermIdAsync(string termId)
        {
            var query = _firestoreDb.Collection("courses").WhereEqualTo("TermId", termId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Course>()).ToList();
        }

        public async Task<Course> GetCourseAsync(string courseId)
        {
            var snapshot = await _firestoreDb.Collection("courses").Document(courseId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Course>() : null;
        }

        public async Task<Course> GetCourseByCourseNumAsync(string courseNum)
        {
            var query = _firestoreDb.Collection("courses").WhereEqualTo("CourseNum", courseNum);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Course>();
        }

        public async Task SaveCourseAsync(Course course)
        {
            await _firestoreDb.Collection("courses").Document(course.Id).SetAsync(course);
        }

        public async Task DeleteCourseAsync(string courseId)
        {
            await _firestoreDb.Collection("courses").Document(courseId).DeleteAsync();
        }

        // INSTRUCTOR METHODS
        public async Task<Instructor> GetInstructorAsync(string instructorId)
        {
            var snapshot = await _firestoreDb.Collection("instructors").Document(instructorId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Instructor>() : null;
        }

        public async Task<Instructor> GetInstructorByEmailAsync(string email)
        {
            var query = _firestoreDb.Collection("instructors").WhereEqualTo("Email", email);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Instructor>();
        }

        public async Task SaveInstructorAsync(Instructor instructor)
        {
            await _firestoreDb.Collection("instructors").Document(instructor.Id).SetAsync(instructor);
        }

        // ASSESSMENT METHODS
        public async Task<List<Assessment>> GetAssessmentsByCourseIdAsync(string courseId)
        {
            var query = _firestoreDb.Collection("assessments").WhereEqualTo("CourseId", courseId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Assessment>()).ToList();
        }

        public async Task<Assessment> GetAssessmentAsync(string assessmentId)
        {
            var snapshot = await _firestoreDb.Collection("assessments").Document(assessmentId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Assessment>() : null;
        }

        public async Task SaveAssessmentAsync(Assessment assessment)
        {
            await _firestoreDb.Collection("assessments").Document(assessment.Id).SetAsync(assessment);
        }

        public async Task DeleteAssessmentAsync(string assessmentId)
        {
            await _firestoreDb.Collection("assessments").Document(assessmentId).DeleteAsync();
        }

        public async Task DeleteAssessmentsByCourseIdAsync(string courseId)
        {
            var assessments = await GetAssessmentsByCourseIdAsync(courseId);
            var batch = _firestoreDb.StartBatch();

            foreach (var assessment in assessments)
            {
                batch.Delete(_firestoreDb.Collection("assessments").Document(assessment.Id));
            }

            await batch.CommitAsync();
        }

        // NOTE METHODS
        public async Task<List<Note>> GetNotesByCourseIdAsync(string courseId)
        {
            var query = _firestoreDb.Collection("notes").WhereEqualTo("CourseId", courseId);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Note>()).ToList();
        }

        public async Task<Note> GetNoteAsync(string noteId)
        {
            var snapshot = await _firestoreDb.Collection("notes").Document(noteId).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Note>() : null;
        }

        public async Task SaveNoteAsync(Note note)
        {
            await _firestoreDb.Collection("notes").Document(note.Id).SetAsync(note);
        }

        public async Task DeleteNoteAsync(string noteId)
        {
            await _firestoreDb.Collection("notes").Document(noteId).DeleteAsync();
        }

        public async Task DeleteNotesByCourseIdAsync(string courseId)
        {
            var notes = await GetNotesByCourseIdAsync(courseId);
            var batch = _firestoreDb.StartBatch();

            foreach (var note in notes)
            {
                batch.Delete(_firestoreDb.Collection("notes").Document(note.Id));
            }

            await batch.CommitAsync();
        }
    }
}