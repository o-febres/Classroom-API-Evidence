using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class DeleteStudentGradeReplyMessage : Message
    {
        public DeleteStudentGradeReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}