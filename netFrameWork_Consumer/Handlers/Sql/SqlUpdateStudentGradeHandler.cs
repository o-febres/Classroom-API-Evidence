using System;
using System.Collections.Generic;
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
    public class SqlUpdateStudentGradeHandler : IMessageHandler<UpdateStudentGradeMessage>
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlUpdateStudentGradeHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }
        public void Execute(UpdateStudentGradeMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
                var student = _repository.Read(message.StudentId); // pulls the appropriate student - given ID
                var origGrade = student.Grades.FirstOrDefault(_ => _.Subject == message.SubjectName);
                var newSubject = message.Grade.Subject;
                var newScore = message.Grade.Score;

                if (newSubject != String.Empty)
                    origGrade.Subject = newSubject;
                if (newScore != 0)
                    origGrade.Score = newScore;
                
                
                _repository.UpdateGrade(message.StudentId, origGrade); 
                var replyMessage = new UpdateStudentGradeReplyMessage(message.CorrelationId);
                //_studentRepository.Update(message.StudentId, message.UpdatedFields);

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