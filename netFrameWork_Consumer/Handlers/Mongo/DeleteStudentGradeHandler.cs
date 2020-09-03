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
    public class DeleteStudentGradeHandler : IMessageHandler<DeleteStudentGradeMessage>
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public DeleteStudentGradeHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(DeleteStudentGradeMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;

            if (test == "ofebres-cordero")
            {
                var student = _studentRepository.Read(message.Id);
                var studentGrades = student.Grades.Where(_ => _.Subject != message.Subject);
                var grade = new Dictionary<string, object>
                {
                    {
                        "Grades", studentGrades
                    }
                };
                var replyMessage = new DeleteStudentGradeReplyMessage(message.CorrelationId);
                _studentRepository.Update(message.Id, grade);
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
            }

            Context.Reply(new DeleteStudentGradeReplyMessage(message.CorrelationId));
        }

        public IConsumeContext Context { get; set; }
    }
}