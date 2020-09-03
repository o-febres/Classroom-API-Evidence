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
        UpdateStudentHandler : IMessageHandler<UpdateStudentMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly IRepository<Student, IStudent> _studentRepository;

        public UpdateStudentHandler(IRepository<Student, IStudent> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public void Execute(UpdateStudentMessage message)
        {
            var test = Thread.CurrentPrincipal.Identity.Name;
            if (test == "ofebres-cordero")
            {
                var replyMessage = new UpdateStudentReplyMessage(message.CorrelationId);
                _studentRepository.Update(message.StudentId,
                    message.UpdatedFields); // updates the student given a particular ID and fields to be updated

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