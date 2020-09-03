using System;
using System.Linq;
using System.Threading;
using Ruffer.Security.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Repositories;

namespace Consumer_dotFramework.Handlers.Sql
{
    public class SqlCreateStudentHandler : IMessageHandler<CreateStudentMessage>
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlCreateStudentHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(CreateStudentMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                var replyMessage = new CreateStudentReplyMessage(message.CorrelationId);
                message.Student.Id = _repository.List().Select(_ => _.Id).Max() + 1;
                _repository.CreateStudent(message.Student);
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new CreateStudentReplyMessage(message.CorrelationId));
            }
        }
        public IConsumeContext Context { get; set; }
    }
}