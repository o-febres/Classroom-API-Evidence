using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class CreateStudentGradeReplyMessage : Message
    {
        public CreateStudentGradeReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}