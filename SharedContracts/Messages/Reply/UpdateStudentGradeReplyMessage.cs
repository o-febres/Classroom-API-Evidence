using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class UpdateStudentGradeReplyMessage : Message
    {
        public UpdateStudentGradeReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}