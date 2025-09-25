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
                EndDate = term1.StartDate.AddMonths(2),
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            await connection.InsertAsync(course1);
            Debug.WriteLine($"Inserted courses: {course1.CourseId}");



            // 6. Assessments
            var assess1 = new Assessment
            {
                CourseId = course1.CourseId,
                Name = "Programming Exam 1",
                Type = "Objective",
                Status = "In progress",
                StartDate = term1.StartDate.AddDays(60),
                EndDate = term1.StartDate.AddDays(60),
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            var assess2 = new Assessment
            {
                CourseId = course1.CourseId,
                Name = "Demonstrate Programming Fundamentals",
                Type = "Practical",
                Status = "Not started",
                StartDate = term1.StartDate,
                EndDate = term1.StartDate.AddDays(30),
                NotifyStartDate = true,
                NotifyEndDate = true
            };
            await connection.InsertAsync(assess1);
            await connection.InsertAsync(assess2);

            Debug.WriteLine($"Inserted assessments: {assess1.AssessmentId} -- {assess2.AssessmentId}");

            // 7. Notes
            var note1 = new Note
            {
                CourseId = course1.CourseId,
                Title = "programming notes for exam",
                Body = "Programming is the process of giving a computer instructions so it can perform specific tasks. These instructions are written in languages that both humans and computers can understand, such as Python, JavaScript, or C++. At its core, programming is about problem-solving—breaking down complex problems into smaller, logical steps that a machine can follow.\r\n\r\nA program is made up of statements, variables, and control structures. Variables are used to store data, while control structures such as loops and conditionals help decide what the program should do in different situations. For example, a loop can repeat a set of instructions multiple times, and an if-statement can let the program choose between two or more paths.\r\n\r\nLearning programming often starts with writing simple programs like printing text on the screen or doing basic math. From there, beginners move on to building small applications, experimenting with user input, and understanding how data flows through the program. With practice, these small exercises build the foundation for solving larger, real-world problems using code."
            };
            var note2 = new Note
            {
                CourseId = course1.CourseId,
                Title = "Assessment Tip",
                Body = "For this practical assessment, students are required to demonstrate their understanding of basic programming concepts by writing a short program. The task will begin with creating a simple program that accepts user input, stores it in a variable, and prints a customized message back to the screen. This checks whether the student can declare variables, use input/output functions, and handle strings.Next, the student must demonstrate control structures by creating a loop. For example, the program could ask the user for a number and then use a loop to count from 1 up to that number, displaying each value along the way. This shows an understanding of repetition and iteration, which are essential for solving problems that involve repeated steps. Students may use either a for loop or a while loop, depending on the language of choice.\r\n\r\nFinally, the assessment requires the inclusion of a simple decision-making structure. The program should include at least one if statement that evaluates a condition, such as checking whether a number entered is even or odd, and then printing the result. By combining input, variables, loops, and conditionals, the assessment ensures that students can apply multiple foundational concepts together in one working program."
            };
            await connection.InsertAsync(note1);
            await connection.InsertAsync(note2);

            Debug.WriteLine($"Inserted notes: {note1.NoteId} -- {note2.NoteId}");
        }

    }
}
