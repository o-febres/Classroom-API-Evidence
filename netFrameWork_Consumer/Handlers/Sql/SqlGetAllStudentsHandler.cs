using System;
using System.Linq;
using System.Threading;
using Ruffer.Security.Filters;
using ServiceConnect.Interfaces;
using SharedContracts.Interfaces;
using SharedContracts.Messages;
using SharedContracts.Messages.Reply;
using SharedContracts.Models;
using SharedContracts.Repositories;

namespace Consumer_dotFramework.Handlers.Sql
{
    public class
        SqlGetAllStudentsHandler : IMessageHandler<GetAllStudentsMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlGetAllStudentsHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(GetAllStudentsMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                var replyMessage = new GetAllStudentsReplyMessage(message.CorrelationId)
                {
                    Students = _repository.List().Cast<Student>().ToList()
                };
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new GetAllStudentsReplyMessage(message.CorrelationId));
            }
        }

        public IConsumeContext Context { get; set; }
    }
}