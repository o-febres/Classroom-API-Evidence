using System;
using System.Threading;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    /*public class
        GetStudentIdHandler : IMessageHandler<GetStudentIdMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public GetStudentIdHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(GetStudentIdMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var replyMessage = new GetStudentIdReplyMessage(message.CorrelationId)
                {
                    Student = (Student) _studentRepository.Read(message.Id)
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
    }*/
}