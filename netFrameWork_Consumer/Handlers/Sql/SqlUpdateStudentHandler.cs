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
        SqlUpdateStudentHandler : IMessageHandler<UpdateStudentMessage> // Class must inherit : Message - creates new exchange and attaches queue
    {
        private readonly ISqlRepository<IStudent> _repository;
        private readonly IClaimsHelper _claimsHelper;

        public SqlUpdateStudentHandler(ISqlRepository<IStudent> studentrepository, IClaimsHelper claimsHelper)
        {
            _repository = studentrepository;
            _claimsHelper = claimsHelper;
        }
        public void Execute(UpdateStudentMessage message)
        {
            var username = _claimsHelper.GetClaims().UserClaims.Username;
            if (username == "ofebres-cordero")
            {
               var student =  _repository.Read(message.StudentId);
                var propertiesOnStudent = typeof(Student).GetProperties();
                foreach (var propertyInfo in propertiesOnStudent)
                {
                    var existingValue = propertyInfo.GetValue(student);
                    var newValue = propertyInfo.GetValue(message.UpdatedStudent);

                    if (newValue != existingValue && newValue != null) propertyInfo.SetValue(student, newValue);
                }
                
                var replyMessage = new UpdateStudentReplyMessage(message.CorrelationId);
                student.Id = message.StudentId;
                _repository.UpdateStudent(student);
                
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