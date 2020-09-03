using System;
using System.Threading;
using Ruffer.Security.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Repositories;

namespace Consumer_dotFramework.Handlers.Sql
{
    public class
        SqlDeleteStudentHandler : IMessageHandler<DeleteStudentMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlDeleteStudentHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(DeleteStudentMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                _repository.DeleteStudent(message.Id);
                var replyMessage = new DeleteStudentReplyMessage(message.CorrelationId);
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new DeleteStudentReplyMessage(message.CorrelationId));
            }

        }

        public IConsumeContext Context { get; set; }
    }
}