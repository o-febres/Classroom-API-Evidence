using System;
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
        SqlGetStudentIdHandler : IMessageHandler<GetStudentIdMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlGetStudentIdHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }

        public void Execute(GetStudentIdMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                var replyMessage = new GetStudentIdReplyMessage(message.CorrelationId)
                {
                    Student = (Student) _repository.Read(message.Id)
                };
                Context.Reply(replyMessage);
            }
            else
            {
                Console.WriteLine("Request Failed - Invalid Token");
                Context.Reply(new GetStudentIdReplyMessage(message.CorrelationId));
            }
        }
        public IConsumeContext Context { get; set; }
    }
}