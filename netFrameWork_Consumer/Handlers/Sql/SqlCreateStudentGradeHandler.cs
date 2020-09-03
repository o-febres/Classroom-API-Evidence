using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Ruffer.Security.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Repositories;

namespace Consumer_dotFramework.Handlers.Sql
{
    public class SqlCreateStudentGradeHandler : IMessageHandler<CreateStudentGradeMessage>
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlCreateStudentGradeHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(CreateStudentGradeMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                message.Grade.Id = _repository.ListGrades().Select(_ => _.Id).Max() + 1;
                message.Grade.StudentId = message.Id;
                var replyMessage = new CreateStudentGradeReplyMessage(message.CorrelationId);
                _repository.CreateGrade(message.Grade);
                Context.Reply(replyMessage);
                Console.WriteLine($"Grade created");
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new CreateStudentGradeReplyMessage(message.CorrelationId));
            }

            
        }

        public IConsumeContext Context { get; set; }
    }
}