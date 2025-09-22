using SQLite;
using c971_project.Models;
using System.Diagnostics;

namespace c971_project.Services
{
    public class SeedDb
    {


        public async static void SeedDataAsync(SQLiteAsyncConnection connection)
        {


            var existingStudent = await connection.Table<Student>().FirstOrDefaultAsync();
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

            await connection.InsertAsync(student);
            Debug.WriteLine($"Inserted student: {student.Name}, ID: {student.StudentId}");

            // 2. Terms
            var term1 = new Term
            {
                Name = "Spring 2024",
                TermNum = 1,
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now
            };
            await connection.InsertAsync(term1);
            Debug.WriteLine($"Inserted term: {term1.Name}");

            // 3. Instructors
            var instructor1 = new Instructor
            {
                Name = "Anika Patel",
                Email = "anika.patel@strimeuniversity.edu",
                Phone = "555-123-4567"  // Note the format: 555-123-4567
            };
            await connection.InsertAsync(instructor1);
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
            await connection.InsertAsync(course1);
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
            await connection.InsertAsync(assess1);
            await connection.InsertAsync(assess2);

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
            await connection.InsertAsync(note1);
            await connection.InsertAsync(note2);

            Debug.WriteLine($"Inserted notes: {note1.NoteId} -- {note2.NoteId}");
        }





    }
}
