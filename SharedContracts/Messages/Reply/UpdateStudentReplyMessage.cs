using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class UpdateStudentReplyMessage : Message
    {
        public UpdateStudentReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}