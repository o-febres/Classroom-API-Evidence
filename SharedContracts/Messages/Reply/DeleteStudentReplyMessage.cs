using System;
using ServiceConnect.Interfaces;

namespace SharedContracts.Messages.Reply
{
    public class DeleteStudentReplyMessage : Message
    {
        public DeleteStudentReplyMessage(Guid correlationId) : base(correlationId)
        {
        }
    }
}