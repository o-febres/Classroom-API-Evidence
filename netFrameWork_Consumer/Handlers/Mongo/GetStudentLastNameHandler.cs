using System;
using System.Linq;
using System.Threading;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class
        GetStudentLastNameHandler : IMessageHandler<GetStudentLastnameMessage
        > // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public GetStudentLastNameHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(GetStudentLastnameMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var replyMessage = new GetStudentLastNameReplyMessage(message.CorrelationId)
                {
                    Student = (Student) _studentRepository.List().FirstOrDefault(_ => _.LastName == message.LastName)
                };

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