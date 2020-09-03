using System;
using System.Collections.Generic;
using System.Threading;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class CreateStudentGradeHandler : IMessageHandler<CreateStudentGradeMessage>
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public CreateStudentGradeHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(CreateStudentGradeMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var student = _studentRepository.Read(message.Id);
                var studentGrades = student.Grades;
                if (studentGrades.Count == 0) studentGrades = new List<Grade>();
                studentGrades.Add(message.Grade);
                var grade = new Dictionary<string, object> {{"Grades", studentGrades}};
                var replyMessage = new CreateStudentGradeReplyMessage(message.CorrelationId);
                _studentRepository.Update(message.Id, grade);
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
            }

            Context.Reply(new CreateStudentGradeReplyMessage(message.CorrelationId));
        }

        public IConsumeContext Context { get; set; }
    }
}