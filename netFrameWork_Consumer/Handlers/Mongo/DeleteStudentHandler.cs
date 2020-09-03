using System;
using System.Threading;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class
        DeleteStudentHandler : IMessageHandler<DeleteStudentMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public DeleteStudentHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(DeleteStudentMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                _studentRepository.Delete(message.Id);
                var replyMessage = new DeleteStudentReplyMessage(message.CorrelationId);
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
            }

            Context.Reply(new DeleteStudentReplyMessage(message.CorrelationId));
        }

        public IConsumeContext Context { get; set; }
    }
}