using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class UpdateStudentGradeHandler : IMessageHandler<UpdateStudentGradeMessage>
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public UpdateStudentGradeHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(UpdateStudentGradeMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var student = _studentRepository.Read(message.StudentId); // pulls the appropriate student - given ID
                student.Grades = student.Grades
                    .Select(t =>
                        (t.Subject ?? string.Empty) == message.SubjectName
                            ? message.Grade
                            : t) // says for every grade with the same subjectname, replace the entire grade with new grade
                    .ToList();

                var grade = new Dictionary<string, object>
                {
                    {
                        "Grades", student.Grades
                    }
                };
                _studentRepository.Update(message.StudentId, grade);
                var replyMessage = new UpdateStudentGradeReplyMessage(message.CorrelationId);
                //_studentRepository.Update(message.StudentId, message.UpdatedFields);

                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
            }

            Context.Reply(new GetAllStudentsReplyMessage(message.CorrelationId));
        }

        public IConsumeContext Context { get; set; }
    }
}