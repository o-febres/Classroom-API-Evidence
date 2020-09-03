using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class CreateStudentReplyMessage : Message
    {
        public CreateStudentReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}