using System;
using System.Linq;
using System.Threading;
using MongoDB.Driver;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;

namespace Consumer_dotFramework.Handlers
{
    public class CreateStudentHandler : IMessageHandler<CreateStudentMessage>
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public CreateStudentHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(CreateStudentMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var id = _studentRepository.List(FilterDefinition<Student>.Empty).Select(_ => _.Id).Max() + 1;
                message.Student.Id = id;
                _studentRepository.Create(message.Student);
                var replymessage = new CreateStudentReplyMessage(message.CorrelationId);

                Context.Reply(replymessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
            }

            Context.Reply(new CreateStudentReplyMessage(message.CorrelationId));
        }

        public IConsumeContext Context { get; set; }
    }
}