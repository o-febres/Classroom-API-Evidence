using System;
using ServiceConnect.Interfaces;
using SharedContracts.Models;

namespace SharedContracts.Messages.Reply
{
    public class GetStudentLastNameReplyMessage : Message
    {
        public GetStudentLastNameReplyMessage(Guid correlationId) : base(correlationId)
        {
        }

        public Student Student { get; set; }
    }
}