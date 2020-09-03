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
    public class SqlDeleteStudentGradeHandler : IMessageHandler<DeleteStudentGradeMessage>
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlDeleteStudentGradeHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(DeleteStudentGradeMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                _repository.DeleteGrade(message.Subject);

                var replyMessage = new DeleteStudentGradeReplyMessage(message.CorrelationId);
                
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new DeleteStudentGradeReplyMessage(message.CorrelationId));
            }

        }

        public IConsumeContext Context { get; set; }
    }
}