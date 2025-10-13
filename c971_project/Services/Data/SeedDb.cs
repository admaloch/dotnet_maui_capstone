//using SQLite;
//using c971_project.Models;
//using System.Diagnostics;

//namespace c971_project.Services.Data
//{
//    public class SeedDb
//    {
//        public async static Task SeedDataAsync(SQLiteAsyncConnection connection)
//        {
//            var existingStudent = await connection.Table<Student>().FirstOrDefaultAsync();
//            if (existingStudent != null)
//            {
//                Debug.WriteLine("Students found!!");
//                return; 
//            }


//            Debug.WriteLine("No students found. Seeding database...");
//            // 1. Student
//            var student = new Student
//            {
//                Name = "Brock Johnson",
//                StudentIdNumber = "03829483938",
//                Email = "brockjohnson03@fakeemail.com",
//                Status = "Currently Enrolled",
//                Major = "Computer Science"
//            };

//            await connection.InsertAsync(student);
//            Debug.WriteLine($"Inserted student: {student.Name}, ID: {student.StudentId}");

//            // 2. Terms
//            var term1 = new Term
//            {
//                Name = "Fall 2025",
//                StartDate = DateTime.Now.AddMonths(2),
//                EndDate = DateTime.Now.AddMonths(8)
//            };
//            await connection.InsertAsync(term1);
//            Debug.WriteLine($"Inserted term: {term1.Name}");

//            // 3. Instructors
//            var instructor1 = new Instructor
//            {
//                Name = "Anika Patel",
//                Email = "anika.patel@strimeuniversity.edu",
//                Phone = "555-123-4567"  // Note the format: 555-123-4567
//            };
//            await connection.InsertAsync(instructor1);
//            Debug.WriteLine($"Inserted instructor: {instructor1.Name}");

//            // 4. Courses
//            var course1 = new Course
//            {
//                Name = "Intro to Programming",
//                CourseNum = "CS101",
//                CuNum = 3,
//                InstructorId = instructor1.InstructorId,
//                TermId = term1.TermId,
//                StartDate = term1.StartDate,
//                EndDate = term1.StartDate.AddMonths(3),

//                NotifyStartDate = true,
//                NotifyEndDate = true
//            };
//            await connection.InsertAsync(course1);
//            Debug.WriteLine($"Inserted coursess: {course1.CourseId}");



//            // 6. Assessments
//            var assess1 = new Assessment
//            {
//                CourseId = course1.CourseId,
//                Name = "Programming Exam 1",
//                Type = "Objective",
//                Status = "In progress",
//                StartDate = course1.StartDate.AddDays(30),
//                EndDate = course1.StartDate.AddDays(30),
//                // ADD TIME PROPERTIES:
//                StartTime = new TimeSpan(14, 0, 0),  // 2:00 pM
//                EndTime = new TimeSpan(16, 0, 0),   // 4:00 PM
//                NotifyStartDate = true,
//                NotifyEndDate = true
//            };

//            var assess2 = new Assessment
//            {
//                CourseId = course1.CourseId,
//                Name = "Demonstrate Programming Fundamentals",
//                Type = "Performance",  // ❌ CHANGE FROM "Practical" TO "Performance"
//                Status = "Not started",
//                StartDate = course1.StartDate.AddDays(30),
//                EndDate = course1.EndDate.AddDays(-3),
//                // ADD TIME PROPERTIES:
//                StartTime = new TimeSpan(0, 0, 0),  // 12:00 AM
//                EndTime = new TimeSpan(0, 0, 0),   // 12:00 aM
//                NotifyStartDate = true,
//                NotifyEndDate = true
//            };
//            await connection.InsertAsync(assess1);
//            await connection.InsertAsync(assess2);

//            Debug.WriteLine($"Inserted assessments: {assess1.AssessmentId} -- {assess2.AssessmentId}");

//            // 7. Notes
//            var note1 = new Note
//            {
//                CourseId = course1.CourseId,
//                Title = "Programming Exam Study Notes",
//                Body = "Key topics to study:\n- Variables and data types\n- Control structures (if/else, loops)\n- Functions and methods\n- Basic algorithms\n\nPractice with the quizzes and review past assignments.",
//                DateAdded = DateTime.Now.AddDays(-5),
//                LastUpdated = DateTime.Now.AddDays(-1)
//            };

//            var note2 = new Note
//            {
//                CourseId = course1.CourseId,
//                Title = "Assessment Preparation Tips",
//                Body = "For the performance assessment:\n1. Review the rubric carefully\n2. Practice with sample problems\n3. Test your code thoroughly\n4. Document your thought process\n\nRemember to manage your time effectively!",
//                DateAdded = DateTime.Now.AddDays(-3),
//                LastUpdated = DateTime.Now.AddDays(-1)
//            };
//            await connection.InsertAsync(note1);
//            await connection.InsertAsync(note2);

//            Debug.WriteLine($"Inserted notes: {note1.NoteId} -- {note2.NoteId}");
//        }

//    }
//}
